using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using Autofac;
using BLToolkit.Mapping;
using Core.Extensions;
using Core.Infrastructure;
using Core.Linq;
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
    public class PreparedReportViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;

        private FactPreparedReport _currentRow;
        private PLinqPreparedReportList _dataList;

        private bool _isEditReportOpen;
        private PropertyGridViewModel<EditPreparedReportViewModel> _editPreparedReportModel;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(PreparedReportViewModel).FullName; }
        }
        #endregion


        public PreparedReportViewModel(PLinqPreparedReportList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
        {
            DataList = list;
            _messageService = messageService;
            _repository = repository;
            _notifyManager = notifyManager;
            Initialize();
        }

        public FactPreparedReport CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value; RaisePropertyChanged(()=>CurrentRow);
            }
        }

        public PLinqPreparedReportList DataList
        {
            get { return _dataList; }
            set
            {
                if (_dataList == value) return;
                _dataList = value;
                RaisePropertyChanged("DataList");
            }
        }
        
        private void Initialize()
        {
            PageName = "Отчеты";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "printPreparedReport",
                    Caption = "Печать отчета",
                    Command = new RelayCommand(PrintDocument, CanPrintDocument),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/document-print.png",
                    SmallGlyph = "../Resources/Icons/document-print.png"
                },
                new ContextCommand
                {
                    Id = "editPreparedReport",
                    Caption = "Редактировать отчет",
                    Command = new RelayCommand(EditDocument, CanEditDocument),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png"
                },
                new ContextCommand
                {
                    Id = "deletePreparedReport",
                    Caption = "Удалить отчет",
                    Command = new RelayCommand(DeleteDocument, CanDeleteDocument),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                }
            };
        }

        public PropertyGridViewModel<EditPreparedReportViewModel> EditPreparedReportModel
        {
            get { return _editPreparedReportModel; }
            set { _editPreparedReportModel = value; RaisePropertyChanged(() => EditPreparedReportModel); }
        }

        public bool IsEditReportOpen
        {
            get { return _isEditReportOpen; }
            set { _isEditReportOpen = value; RaisePropertyChanged(()=>IsEditReportOpen); }
        }

        private bool CanEditDocument()
        {
            return CurrentRow != null;
        }

        private void EditDocument()
        {
            try
            {
                IsEditReportOpen = true;
                var copy = Map.ObjectToObject<FactPreparedReport>(CurrentRow);
                var model = new PropertyGridViewModel<EditPreparedReportViewModel>(new EditPreparedReportViewModel(copy));
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactPreparedReport>(model.SelectedObject.Classifier);

                    var result = _repository.Update(tmp);
                    if (result.Success)
                    {
                        var map = model.SelectedObject.MapObject<FactPreparedReport>(CurrentRow);
                        CurrentRow.Update(map.Affected);
                        _notifyManager.ShowNotify("Данные готового отчета успешно обновлены.");
                        IsEditReportOpen = false;
                    }

                };

                EditPreparedReportModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при редактировании готового отчета ID {0}".F(CurrentRow.PreparedReportId), typeof(ReportViewModel));
            }

        }


        private bool CanPrintDocument()
        {
            return CurrentRow != null;
        }

        private void PrintDocument()
        {
            try
            {
                var report = new FastReport.Report();
                if (CurrentRow.Body == null || CurrentRow.Body.Length == 0)
                {
                    _notifyManager.ShowNotify("Ошибка при загрузке готового отчета.");
                    return;
                }
                report.LoadPrepared(new MemoryStream(CurrentRow.Body));
                report.ShowPrepared();
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при печати готового отчета ID {0}".F(CurrentRow.PreparedReportId), typeof(PreparedReportViewModel));
            }
        }

        private bool CanDeleteDocument()
        {
            return CurrentRow != null;
        }

        private void DeleteDocument()
        {
            try
            {
                if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить готовый отчет ID {0}?", "Внимание!",
                    CurrentRow.PreparedReportId))
                {
                    var result = _repository.DeletePreparedReport(CurrentRow.PreparedReportId);
                    if (result.Success)
                    {
                        Load();
                        _notifyManager.ShowNotify("Отчет успешно удален.");
                    }
                }
            }           
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при попытке удалить готовый отчет ID {0}".F(CurrentRow.PreparedReportId), typeof(PreparedReportViewModel));
            }
        }

        private void Load()
        {
            Expression<Func<FactPreparedReport, bool>> predicate = PredicateBuilder.True<FactPreparedReport>();
            //predicate = predicate.And(p=>p.);
            using (var scope = Di.I.BeginLifetimeScope())
            {
                DataList = scope.Resolve<PLinqPreparedReportList>();
            }
        }
    }
}