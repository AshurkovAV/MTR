using System;
using System.Collections.ObjectModel;
using Autofac;
using BLToolkit.Mapping;
using Core.Extensions;
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

namespace Medical.AppLayer.Report.ViewModels
{
    public class ReportViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;

        private FactReport _currentRow;
        private PLinqReportList _dataList;
        private bool _isNewReportOpen;
        private PropertyGridViewModel<EditReportViewModel> _newReportModel;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(ReportViewModel).FullName; }
        }
        #endregion

        public ReportViewModel(PLinqReportList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
        {
            DataList = list;
            _messageService = messageService;
            _repository = repository;
            _notifyManager = notifyManager;
            Initialize();
        }

        private void Initialize()
        {
            PageName = "Отчеты";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "newReport",
                    Caption = "Новый отчет",
                    Command = new RelayCommand(AddReport, CanAddReport),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/document-new.png",
                    SmallGlyph = "../Resources/Icons/document-new.png"
                },
                new ContextCommand
                {
                    Id = "editReport",
                    Caption = "Редактировать отчет",
                    Command = new RelayCommand(EditReport, CanEditReport),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png"
                },
                new ContextCommand
                {
                    Id = "deleteReport",
                    Caption = "Удалить отчет",
                    Command = new RelayCommand(DeleteReport, CanDeleteReport),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                }
            };
        }
        public PLinqReportList DataList
        {
            get { return _dataList; }
            set
            {
                if (_dataList == value) return;
                _dataList = value;
                RaisePropertyChanged("DataList");
            }
        }

        public FactReport CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        public bool IsNewReportOpen
        {
            get { return _isNewReportOpen; }
            set { _isNewReportOpen = value; RaisePropertyChanged(()=>IsNewReportOpen); }
        }

        public PropertyGridViewModel<EditReportViewModel> NewReportModel
        {
            get { return _newReportModel; }
            set { _newReportModel = value; RaisePropertyChanged(()=>NewReportModel); }
        }

        private bool CanAddReport()
        {
            return true;
        }

        private void AddReport()
        {
            try
            {
                IsNewReportOpen = true;
                var model = new PropertyGridViewModel<EditReportViewModel>(new EditReportViewModel());
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactReport>(model.SelectedObject.Classifier);

                    var result = _repository.InsertWithIdentity(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Отчет успешно добавлен.");
                        Load();
                        IsNewReportOpen = false;
                    }
                };

                NewReportModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при создании отчета", typeof(ReportViewModel));
            }
        }

        private bool CanEditReport()
        {
            return CurrentRow != null;
        }

        private void EditReport()
        {
            try
            {
                IsNewReportOpen = true;
                var copy = Map.ObjectToObject<FactReport>(CurrentRow);
                var model = new PropertyGridViewModel<EditReportViewModel>(new EditReportViewModel(copy));
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactReport>(model.SelectedObject.Classifier);

                    var result = _repository.Update(tmp);
                    if (result.Success)
                    {
                        _notifyManager.ShowNotify("Отчет успешно обновлен.");
                        Load();
                        IsNewReportOpen = false;
                    }
                };

                NewReportModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при обновлении отчета ID {0}".F(CurrentRow.FactReportID), typeof(ReportViewModel));
            }
        }

        private bool CanDeleteReport()
        {
            return CurrentRow != null;
        }

        private void DeleteReport()
        {
            try
            {
                if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить отчет ID {0}?", "Внимание!",
                    CurrentRow.FactReportID))
                {
                    var result = _repository.DeleteReport(CurrentRow.FactReportID);
                    if (result.Success)
                    {
                        Load();
                        _notifyManager.ShowNotify("Отчет успешно удален.");
                    }
                } 
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Исключение при попытке удаления отчета ID {0}", typeof(ReportViewModel), CurrentRow.FactReportID);
            }
        }

        private void Load()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                DataList = scope.Resolve<PLinqReportList>();
            }
        }
    }
}