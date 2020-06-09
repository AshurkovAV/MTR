using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    public class ActsViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IMessageService _messageService;
        private readonly IReportService _reportService;

        #region IContextCommandContainer

        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }

        #endregion

        #region IHash

        public string Hash
        {
            get { return typeof(ActsViewModel).FullName + _scope.ToString(); }
        }

        #endregion

        private readonly int? _accountId;
        private readonly int _scope;

        private ActModel _currentRow;

        private bool _isEditActMeeOpen;
        private MeeActViewModel _meeActModel;
        private bool _isEditActEqmaOpen;
        private EqmaActViewModel _eqmaActModel;

        private PLinqActsList _actsListSource;



        public bool IsEditActMeeOpen
        {
            get { return _isEditActMeeOpen; }
            set { _isEditActMeeOpen = value; RaisePropertyChanged(() => IsEditActMeeOpen); }
        }

        public MeeActViewModel MeeActModel
        {
            get { return _meeActModel; }
            set { _meeActModel = value; RaisePropertyChanged(() => MeeActModel); }
        }

        public bool IsEditActEqmaOpen
        {
            get { return _isEditActEqmaOpen; }
            set { _isEditActEqmaOpen = value; RaisePropertyChanged(() => IsEditActEqmaOpen); }
        }

        public EqmaActViewModel EqmaActModel
        {
            get { return _eqmaActModel; }
            set { _eqmaActModel = value; RaisePropertyChanged(() => EqmaActModel); }
        }

        public ActModel CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        public PLinqActsList ActsListSource
        {
            get { return _actsListSource; }
            set { _actsListSource = value; RaisePropertyChanged(() => ActsListSource); }
        }

        public ActsViewModel(
            IMedicineRepository repository,
            INotifyManager notifyManager,
            IMessageService messageService,
            IReportService reportService,
            int scope,
            int? accountId)
        {
            _accountId = accountId;
            _scope = scope;

            _repository = repository;
            _notifyManager = notifyManager;
            _messageService = messageService;
            _reportService = reportService;

            Initialize();
        }

        private void Initialize()
        {
            PageName = "Акты экспертиз";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "edit",
                    Caption = "Редактировать",
                    Command = new RelayCommand(Edit, CanEdit),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png"
                },
                new ContextCommand
                {
                    Id = "reload",
                    Caption = "Обновить",
                    Command = new RelayCommand(Reload),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png"
                },
                new ContextCommand
                {
                    Id = "print",
                    Caption = "Печать акта",
                    Command = new RelayCommand(Print, CanPrint),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/printer.png",
                    SmallGlyph = "../Resources/Icons/printer.png"
                },
                new ContextCommand
                {
                    Id = "print",
                    Caption = "Печать всех актов",
                    Command = new RelayCommand(PrintAll),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/printer.png",
                    SmallGlyph = "../Resources/Icons/printer.png"
                },
                new ContextCommand
                {
                    Id = "delete",
                    Caption = "Удалить акт",
                    Command = new RelayCommand(Delete, CanDelete),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                }
            };
            Reload();
        }

        private bool CanEdit()
        {
            return CurrentRow.IsNotNull();
        }

        private bool CanPrint()
        {
            return CurrentRow.IsNotNull();
        }

        private void PrintAll()
        {
            var actIds = new List<object>();
            var allReportsIds = _actsListSource.GetAsQueryable().Select(p => p.Id);
            foreach (var id in allReportsIds)
            {
                switch (CurrentRow.Type)
                {
                    case RefusalType.MEE:
                        var reportsMee = _repository.GetPreparedReportByExternalIdScopeSubId(id, Constants.ScopeActMee, CurrentRow.AccountId);
                        if (reportsMee.Success)
                        {
                            actIds.AddRange(reportsMee.Data.Select(p => p.PreparedReportId as object));
                        }
                        break;
                    case RefusalType.EQMA:
                        var reportsEqma = _repository.GetPreparedReportByExternalIdScopeSubId(id, Constants.ScopeActEqma, CurrentRow.AccountId);
                        if (reportsEqma.Success)
                        {
                            actIds.AddRange(reportsEqma.Data.Select(p => p.PreparedReportId as object));
                        }
                        break;
                    default:
                        return;
                }
            }

            _reportService.PrintReports(actIds, true);
        }

        private void Print()
        {
            switch (CurrentRow.Type)
            {
                case RefusalType.MEE:
                    _reportService.PrintReports(CurrentRow.Id, Constants.ScopeActMee, CurrentRow.AccountId, true);
                    break;
                case RefusalType.EQMA:
                    _reportService.PrintReports(CurrentRow.Id, Constants.ScopeActEqma, CurrentRow.AccountId, true);
                    break;
                default:
                    return;
            }
            
        }

        private bool CanDelete()
        {
            return CurrentRow.IsNotNull();
        }

        private void Delete()
        {
            if (_messageService.AskQuestion("Вы действительно хотите удалить акт?", "Внимание"))
            {
                TransactionResult deleteResult;
                switch (CurrentRow.Type)
                {
                    case RefusalType.MEE:
                        deleteResult = _repository.DeleteMeeAct(CurrentRow.Id);
                        break;
                    case RefusalType.EQMA:
                        deleteResult = _repository.DeleteEqmaAct(CurrentRow.Id);
                        break;
                    default:
                        return;
                }

                if (deleteResult.Success)
                {
                    Reload();
                    _notifyManager.ShowNotify("Акт успешно удален");
                }
            }
        }

        private void Reload()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (_accountId.HasValue)
                {
                    ActsListSource = scope.Resolve<PLinqActsList>(new NamedParameter("scope", _scope), new NamedParameter("accountId", _accountId));
                }
                else
                {
                    ActsListSource = scope.Resolve<PLinqActsList>(new NamedParameter("scope", _scope));
                }
            }
        }

        private void Edit()
        {
            switch (CurrentRow.Type)
            {
                case RefusalType.MEE:
                    IsEditActMeeOpen = true;
                    using (var scope = Di.I.BeginLifetimeScope())
                    {
                        var model = scope.Resolve<MeeActViewModel>(
                            new NamedParameter("accountId", CurrentRow.AccountId),
                            new NamedParameter("scope", _scope),
                            new NamedParameter("actId", CurrentRow.Id));
                        model.OkCallback = () =>
                        {
                            _notifyManager.ShowNotify("Данные акта МЭЭ успешно обновлены.");
                            IsEditActMeeOpen = false;
                        };
                        MeeActModel = model;
                    }
                    break;
                case RefusalType.EQMA:
                    IsEditActEqmaOpen = true;
                    using (var scope = Di.I.BeginLifetimeScope())
                    {
                        var model = scope.Resolve<EqmaActViewModel>(
                            new NamedParameter("accountId", CurrentRow.AccountId),
                            new NamedParameter("scope", _scope),
                            new NamedParameter("actId", CurrentRow.Id));
                        model.OkCallback = () =>
                        {
                            _notifyManager.ShowNotify("Данные акта ЭКМП успешно обновлены.");
                            IsEditActEqmaOpen = false;
                        };
                        EqmaActModel = model;
                    }
                    break;
                default:
                    return;

            }
            
        }
    }
}
