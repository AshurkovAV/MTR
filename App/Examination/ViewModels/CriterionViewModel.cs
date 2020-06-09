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

namespace Medical.AppLayer.Examination.ViewModels
{
    public class CriterionViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;

        private FactExpertCriterion _currentRow;
        private PLinqExaminationList _dataList;

        private bool _isNewCriterionOpen;
        private PropertyGridViewModel<EditCriterionViewModel> _newCriterionModel;
        private bool _isEditCriterionOpen;
        

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(CriterionViewModel).FullName; }
        }
        #endregion

        public CriterionViewModel(PLinqExaminationList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
        {
            DataList = list;
            _messageService = messageService;
            _repository = repository;
            _notifyManager = notifyManager;
            Initialize();
        }

        private void Initialize()
        {
            PageName = "Критерии экспертизы";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "newCriterion",
                    Caption = "Новый критерий",
                    Command = new RelayCommand(AddCriterion, CanAddCriterion),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/document-new.png",
                    SmallGlyph = "../Resources/Icons/document-new.png"
                },
                new ContextCommand
                {
                    Id = "editCriterion",
                    Caption = "Редактировать критерий",
                    Command = new RelayCommand(EditCriterion, CanEditCriterion),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png"
                },
                new ContextCommand
                {
                    Id = "copyCriterion",
                    Caption = "Копировать критерий",
                    Command = new RelayCommand(CopyCriterion, CanCopyCriterion),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-copy.png",
                    SmallGlyph = "../Resources/Icons/edit-copy.png"
                },
                new ContextCommand
                {
                    Id = "deleteCriterion",
                    Caption = "Удалить критерий",
                    Command = new RelayCommand(DeleteCriterion, CanDeleteCriterion),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                }
            };
        }

        public PLinqExaminationList DataList
        {
            get { return _dataList; }
            set
            {
                if (_dataList == value) return;
                _dataList = value;
                RaisePropertyChanged("DataList");
            }
        }

        public FactExpertCriterion CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        public bool IsNewCriterionOpen
        {
            get { return _isNewCriterionOpen; }
            set { _isNewCriterionOpen = value; RaisePropertyChanged(() => IsNewCriterionOpen); }
        }

        public PropertyGridViewModel<EditCriterionViewModel> NewCriterionModel
        {
            get { return _newCriterionModel; }
            set
            {
                _newCriterionModel = value;
                RaisePropertyChanged(() => NewCriterionModel);
            }
        }

        public bool IsEditCriterionOpen
        {
            get { return _isEditCriterionOpen; }
            set { _isEditCriterionOpen = value; RaisePropertyChanged(() => IsEditCriterionOpen); }
        }

        

        private bool CanAddCriterion()
        {
            return true;
        }

        private void AddCriterion()
        {
            try
            {
                IsNewCriterionOpen = true;
                var model = new PropertyGridViewModel<EditCriterionViewModel>(new EditCriterionViewModel());
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactExpertCriterion>(model.SelectedObject.Classifier);

                    var result = _repository.InsertWithIdentity(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Критерий успешно добавлен.");
                        Load();
                        IsNewCriterionOpen = false;
                    }
                };

                NewCriterionModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при создании критерия", typeof(CriterionViewModel));
            }
        }

        private bool CanEditCriterion()
        {
            return CurrentRow != null;
        }

        private void EditCriterion()
        {
            try
            {
                IsNewCriterionOpen = true;
                var copy = Map.ObjectToObject<FactExpertCriterion>(CurrentRow);
                var model = new PropertyGridViewModel<EditCriterionViewModel>(new EditCriterionViewModel(copy));
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactExpertCriterion>(model.SelectedObject.Classifier);

                    var result = _repository.Update(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Критерий успешно обновлен.");
                        Load();
                        IsNewCriterionOpen = false;
                    }
                };

                NewCriterionModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при обновлении критерия", typeof(CriterionViewModel));
            }
        }

        private bool CanDeleteCriterion()
        {
            return CurrentRow != null;
        }

        private void DeleteCriterion()
        {
            try
            {
                if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить экспертизу ID {0}?", "Внимание!",
                    CurrentRow.FactExpertCriterionID))
                {
                    var result = _repository.DeleteExpertCriterion(CurrentRow.FactExpertCriterionID);
                    if (result.Success)
                    {
                        Load();
                        _notifyManager.ShowNotify("Экспертиза успешно удалена.");
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Исключение при попытке удаления экспертизы ID {0}", typeof(CriterionViewModel), CurrentRow.FactExpertCriterionID);
            }
        }

        private bool CanCopyCriterion()
        {
            return CurrentRow != null;
        }

        private void CopyCriterion()
        {
            try
            {
                IsNewCriterionOpen = true;
                var copy = Map.ObjectToObject<FactExpertCriterion>(CurrentRow);
                copy.FactExpertCriterionID = 0;
                copy.Name += "Копия";
                var model = new PropertyGridViewModel<EditCriterionViewModel>(new EditCriterionViewModel(copy));
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactExpertCriterion>(model.SelectedObject.Classifier);

                    var result = _repository.InsertWithIdentity(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Критерий успешно скопирован.");
                        Load();
                        IsNewCriterionOpen = false;
                    }
                };

                NewCriterionModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при копировании критерия", typeof(CriterionViewModel));
            }
        }

        private void Load()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                DataList = scope.Resolve<PLinqExaminationList>();
            }
        }
    }
}