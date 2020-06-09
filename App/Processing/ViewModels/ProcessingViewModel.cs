using System;
using System.Collections.ObjectModel;
using Autofac;
using BLToolkit.Mapping;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.PropertyGrid;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Processing.ViewModels
{
    public class ProcessingViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IMessageService _messageService;

        #region IContextCommandContainer

        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }

        #endregion

        #region IHash

        public string Hash
        {
            get { return typeof (ProcessingViewModel).FullName; }
        }

        #endregion

        private FactProcessing _currentRow;
        private PLinqProcessingList _userListSource;

        private bool _isEditProcessingOpen;
        private PropertyGridViewModel<EditProcessingViewModel> _processingModel;
        
        public FactProcessing CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        public PLinqProcessingList ProcessingListSource
        {
            get { return _userListSource; }
            set { _userListSource = value; RaisePropertyChanged(() => ProcessingListSource); }
        }

        public ProcessingViewModel(PLinqProcessingList processingList, 
            IMedicineRepository repository, 
            INotifyManager notifyManager,
            IMessageService messageService)
        {
            _repository = repository;
            _notifyManager = notifyManager;
            _messageService = messageService;

            ProcessingListSource = processingList;

            Initialize();
        }

        private void Initialize()
        {
            PageName = "Функции обработки данных";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "newCriterion",
                    Caption = "Новая функция",
                    Command = new RelayCommand(AddProcessing, CanAddProcessing),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/document-new.png",
                    SmallGlyph = "../Resources/Icons/document-new.png"
                },
                new ContextCommand
                {
                    Id = "editCriterion",
                    Caption = "Редактировать",
                    Command = new RelayCommand(EditProcessing, CanEditProcessing),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png"
                },
                new ContextCommand
                {
                    Id = "deleteCriterion",
                    Caption = "Удалить",
                    Command = new RelayCommand(DeleteProcessing, CanDeleteProcessing),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                }
            };
        }

        public PropertyGridViewModel<EditProcessingViewModel> ProcessingModel
        {
            get { return _processingModel; }
            set
            {
                _processingModel = value;
                RaisePropertyChanged(() => ProcessingModel);
            }
        }

        public bool IsEditProcessingOpen
        {
            get { return _isEditProcessingOpen; }
            set { _isEditProcessingOpen = value; RaisePropertyChanged(() => IsEditProcessingOpen); }
        }



        private bool CanAddProcessing()
        {
            return true;
        }

        private void AddProcessing()
        {
            try
            {
                IsEditProcessingOpen = true;
                var model = new PropertyGridViewModel<EditProcessingViewModel>(new EditProcessingViewModel());
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactProcessing>(model.SelectedObject.Classifier);

                    var result = _repository.InsertWithIdentity(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Функция обработки данных успешно добавлена.");
                        Reload();
                        IsEditProcessingOpen = false;
                    }
                };

                ProcessingModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при создании функции обработки данных", typeof(ProcessingViewModel));
            }
        }

        private bool CanEditProcessing()
        {
            return CurrentRow != null;
        }

        private void EditProcessing()
        {
            try
            {
                IsEditProcessingOpen = true;
                var copy = Map.ObjectToObject<FactProcessing>(CurrentRow);
                var model = new PropertyGridViewModel<EditProcessingViewModel>(new EditProcessingViewModel(copy));
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactProcessing>(model.SelectedObject.Classifier);

                    var result = _repository.Update(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Функция обработки данных успешно обновлена.");
                        Reload();
                        IsEditProcessingOpen = false;
                    }
                };

                ProcessingModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при обновлении критерия", typeof(ProcessingViewModel));
            }
        }

        private bool CanDeleteProcessing()
        {
            return CurrentRow != null;
        }

        private void DeleteProcessing()
        {
            try
            {
                if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить функцию обработки данных ID {0}?", "Внимание!",
                    CurrentRow.ProcessingId))
                {
                    var result = _repository.Delete(CurrentRow);
                    if (result.Success)
                    {
                        Reload();
                        _notifyManager.ShowNotify("Функция обработки данных успешно удалена.");
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Исключение при попытке удаления функцию обработки данных ID {0}", typeof(ProcessingViewModel), CurrentRow.ProcessingId);
            }
        }

        private void Reload()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                ProcessingListSource = scope.Resolve<PLinqProcessingList>();
            }
        }
    }
}
