using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using BLToolkit.Mapping;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Infrastructure;
using Core.Linq;
using Core.Services;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Examination.ViewModels;
using Medical.AppLayer.Examination.Views;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Processing.ViewModels;
using Medical.AppLayer.Processing.Views;
using Medical.AppLayer.Register.Views;
using Medical.CoreLayer.PropertyGrid;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using Medical.AppLayer.Report.ViewModels;
using Medical.AppLayer.Report.Views;
using Medical.DatabaseCore.Services.Cache;
using MedicalAccountView = DataModel.MedicalAccountView;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace Medical.AppLayer.Register.ViewModels
{
    public class MedicalAccountViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        public static IAppRemoteSettings _remoteSettings;
        private readonly ICacheRepository _cacheRepository;
        private readonly IMedicineRepository _repository;
        private readonly IDockLayoutManager _dockManager;
        private readonly INotifyManager _notifyManager;
        private readonly IOverlayManager _overlayManager;

        private RelayCommand _viewAccountCommand;
        private RelayCommand _editAccountCommand;
        private RelayCommand _createAccountCommand; 
        private RelayCommand _showSummaryCommand;
        private RelayCommand _viewAccountErrorCommand;
        private RelayCommand _viewAccountSrzCommand;
        private RelayCommand _reloadCommand;
        private RelayCommand _viewExchangeCommand;
        private RelayCommand _deleteAccountCommand;
        private RelayCommand _deletePeriodAccountCommand;
        private RelayCommand _runCheckCommand;
        private RelayCommand _checkPolicyCommand;
        private RelayCommand _runProcessingCommand;
        private RelayCommand _createDocumentCommand;
        private RelayCommand _makeTerritoryAccountCommand;
        private RelayCommand _printDocumentCommand;
        private RelayCommand _addMeeCommand;
        private RelayCommand _addEqmaCommand;
        private RelayCommand _viewActCommand;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private MedicalAccountView _currentRow;

        private int? _selectedYear;
        private int? _selectedMonth;
        private int? _selectedMedicalOrganization;

        private PLinqMedicalAccountList _accountListSource;
        private object _eventListSource;

       
        private PropertyGridViewModel<EditMedicalAccountViewModel> _medicalAccount;
        private RunExaminationViewModel _runExaminationModel;
        private MakeTerritoryAccountsViewModel _makeTerritoryAccountsModel;
        private CheckPolicyInTortillaViewModel _tortillaCheckModel;
        
        private RunProcessingViewModel _runProcessingModel;

        private RunReportViewModel _runReportModel;
        private bool _isRunReportOpen;
        private PrintReportViewModel _printReportModel;

        private bool _isMeeOpen;
        private MeeActViewModel _meeModel;
        private bool _isEqmaOpen;
        private EqmaActViewModel _eqmaModel;

        public ObservableCollection<int> YearItemsSource { get; set; }
        public ObservableCollection<CommonTuple> MonthItemsSource { get; set; }
        
        #region IHash
        public string Hash
        {
            get { return typeof(MedicalAccountViewModel).FullName; }
        }
        #endregion

       
        public PrintReportViewModel PrintReportModel
        {
            get { return _printReportModel; }
            set
            {
                _printReportModel = value;
                RaisePropertyChanged(() => PrintReportModel);
            }
        }

        public bool IsRunReportOpen
        {
            get { return _isRunReportOpen; }
            set
            {
                _isRunReportOpen = value;
                RaisePropertyChanged(() => IsRunReportOpen);
            }
        }

        public RunReportViewModel RunReportModel
        {
            get { return _runReportModel; }
            set
            {
                _runReportModel = value;
                RaisePropertyChanged(() => RunReportModel);
            }
        }

        public bool IsMeeOpen
        {
            get { return _isMeeOpen; }
            set
            {
                _isMeeOpen = value;
                RaisePropertyChanged(() => IsMeeOpen);
            }
        }

        public MeeActViewModel MeeModel
        {
            get { return _meeModel; }
            set
            {
                _meeModel = value;
                RaisePropertyChanged(() => MeeModel);
            }
        }

        public bool IsEqmaOpen
        {
            get { return _isEqmaOpen; }
            set
            {
                _isEqmaOpen = value;
                RaisePropertyChanged(() => IsEqmaOpen);
            }
        }

        public EqmaActViewModel EqmaModel
        {
            get { return _eqmaModel; }
            set
            {
                _eqmaModel = value;
                RaisePropertyChanged(() => EqmaModel);
            }
        }

        public ICommand ViewAccountCommand => _viewAccountCommand ??
                                              (_viewAccountCommand = new RelayCommand(ViewAccount, CanViewAccount));
        public ICommand ShowSummaryCommand => _showSummaryCommand ??
                                              (_showSummaryCommand = new RelayCommand(ShowSummary, CanShowSummary));
        public ICommand ViewAccountErrorCommand => _viewAccountErrorCommand ??
                                             (_viewAccountErrorCommand = new RelayCommand(ViewAccountError, CanViewAccountError));

        public ICommand ViewAccountSrzCommand => _viewAccountSrzCommand ??
                                             (_viewAccountSrzCommand = new RelayCommand(ViewAccountSrz, CanViewAccountSrz));
        public ICommand CreateAccountCommand => _createAccountCommand ??
                                             (_createAccountCommand = new RelayCommand(CreateAccount, CanCreateAccount));

        public ICommand EditAccountCommand => _editAccountCommand ??
                                              (_editAccountCommand = new RelayCommand(EditAccount, CanEditAccount));
        public ICommand ReloadCommand => _reloadCommand ??
                                                     (_reloadCommand = new RelayCommand(RefreshAccount));
        public ICommand ViewExchangeCommand => _viewExchangeCommand ??
                                             (_viewExchangeCommand = new RelayCommand(ViewExchange, CanViewExchange));
        public ICommand DeleteAccountCommand => _deleteAccountCommand ??
                                             (_deleteAccountCommand = new RelayCommand(DeleteAcount, CanDeleteAcount));
        public ICommand DeleteByPeriodAccountCommand => _deletePeriodAccountCommand ??
                                             (_deletePeriodAccountCommand = new RelayCommand(DeleteAcountByPeriod, CanDeleteAcount));
        public ICommand RunCheckCommand => _runCheckCommand ??
                                             (_runCheckCommand = new RelayCommand(RunCheck, CanRunCheck));
        public ICommand CheckPolicyCommand => _checkPolicyCommand ??
                                             (_checkPolicyCommand = new RelayCommand(TortillaCheck, CanTortillaCheck));
        public ICommand RunProcessingCommand => _runProcessingCommand ??
                                             (_runProcessingCommand = new RelayCommand(RunProcessing, CanRunProcessing));
        public ICommand MakeTerritoryAccountCommand => _makeTerritoryAccountCommand ??
                                            (_makeTerritoryAccountCommand = new RelayCommand(MakeTerritoryAccount, CanMakeTerritoryAccount)); 
        public ICommand CreateDocumentCommand => _createDocumentCommand ??
                                             (_createDocumentCommand = new RelayCommand(CreateDocuments, CanCreateDocuments));
        public ICommand PrintDocumentCommand => _printDocumentCommand ??
                                             (_printDocumentCommand = new RelayCommand(PrintDocuments, CanPrintDocuments));
        public ICommand AddMeeCommand => _addMeeCommand ??
                                             (_addMeeCommand = new RelayCommand(AddMee, CanAddMee));
        public ICommand AddEqmaCommand => _addEqmaCommand ??
                                             (_addEqmaCommand = new RelayCommand(AddEqma, CanAddEqma));
        public ICommand ViewActCommand => _viewActCommand ??
                                             (_viewActCommand = new RelayCommand(ViewAct, CanViewAct));
        public MedicalAccountViewModel(PLinqMedicalAccountList list,
            IMessageService messageService,
            IMedicineRepository repository,
            IDockLayoutManager dockManager,
            INotifyManager notifyManager,
            IOverlayManager overlayManager)
        {
            AccountListSource = list;
            _messageService = messageService;
            _repository = repository;
            _overlayManager = overlayManager;
            _dockManager = dockManager;
            _notifyManager = notifyManager;
            _remoteSettings = Di.I.Resolve<IAppRemoteSettings>();
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            Initialize();
            if (YearItemsSource.Any())
            {
                SelectedYear = YearItemsSource.Last();
            }

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            //PageName = "Счета МО";
            //ContextCommands = new ObservableCollection<IContextCommand>
            //{
            //    new ContextCommand
            //    {
            //        Id = "viewAccount",
            //        Caption = "Просмотр позиций",
            //        Command = new RelayCommand(ViewAccount, CanViewAccount),
            //        Page = "Просмотр",
            //        LargeGlyph = "../Resources/Icons/eye.png",
            //        SmallGlyph = "../Resources/Icons/eye.png"
            //    },
            //    new ContextCommand
            //    {
            //        Id = "viewAccountFilter",
            //        Caption = "Просмотр (фильтры)",
            //        Page = "Просмотр",
            //        LargeGlyph = "../Resources/Icons/view-filter.png",
            //        SmallGlyph = "../Resources/Icons/view-filter.png",
            //        IsParent = true
            //    },
            //    new ContextCommand
            //    {
            //        Id = "viewOnlyError",
            //        Caption = "Позиции счета(ошибки)",
            //        LargeGlyph = "../Resources/Icons/view-filter-error.png",
            //        SmallGlyph = "../Resources/Icons/view-filter-error.png",
            //        Parent = "viewAccountFilter",
            //        Command = new RelayCommand(ViewAccountError, CanViewAccountError),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "viewOnlySrz",
            //        Caption = "Позиции счета(СРЗ)",
            //        LargeGlyph = "../Resources/Icons/view-filter-people.png",
            //        SmallGlyph = "../Resources/Icons/view-filter-people.png",
            //        Parent = "viewAccountFilter",
            //        Command = new RelayCommand(ViewAccountSrz, CanViewAccountSrz),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "editAccount",
            //        Caption = "Редактировать",
            //        Page = "Редактирование",
            //        LargeGlyph = "../Resources/Icons/edit.png",
            //        SmallGlyph = "../Resources/Icons/edit.png",
            //        Command = new RelayCommand(EditAccount, CanEditAccount),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "reloadAccount",
            //        Caption = "Обновить",
            //        Page = "Действия",
            //        LargeGlyph = "../Resources/Icons/view-refresh.png",
            //        SmallGlyph = "../Resources/Icons/view-refresh.png",
            //        Command = new RelayCommand(RefreshAccount),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "viewRelations",
            //        Caption = "Просмотр связанных данных",
            //        Page = "Действия",
            //        LargeGlyph = "../Resources/Icons/run-build-install.png",
            //        SmallGlyph = "../Resources/Icons/run-build-install.png",
            //        IsParent = true
            //    },
            //    new ContextCommand
            //    {
            //        Id = "actionsViewExchange",
            //        Caption = "Просмотр журнала информационного взаимодействия",
            //        LargeGlyph = "../Resources/Icons/text-x-changelog.png",
            //        SmallGlyph = "../Resources/Icons/text-x-changelog.png",
            //        Parent = "viewRelations",
            //        Command = new RelayCommand(ViewExchange, CanViewExchange),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "deleteAccount",
            //        Caption = "Удалить",
            //        Page = "Редактирование",
            //        LargeGlyph = "../Resources/Icons/edit-delete.png",
            //        SmallGlyph = "../Resources/Icons/edit-delete.png",
            //        Command = new RelayCommand(DeleteAcount, CanDeleteAcount),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "summary",
            //        Caption = "Саммари",
            //        LargeGlyph = "../Resources/Icons/text-x-texinfo.png",
            //        SmallGlyph = "../Resources/Icons/text-x-texinfo.png",
            //        Command = new RelayCommand(ShowSummary, CanShowSummary),
            //        Page = "Статистика",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "runCheck",
            //        Caption = "Запуск экспертиз",
            //        LargeGlyph = "../Resources/Icons/edit-clear.png",
            //        SmallGlyph = "../Resources/Icons/edit-clear.png",
            //        Command = new RelayCommand(RunCheck, CanRunCheck),
            //        Page = "Проверки",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "runTortilla",
            //        Caption = "Проверка в Тортилле",
            //        LargeGlyph = "../Resources/Icons/tortoise.png",
            //        SmallGlyph = "../Resources/Icons/tortoise.png",
            //        Command = new RelayCommand(TortillaCheck, CanTortillaCheck),
            //        Page = "Проверки",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "runProcessing",
            //        Caption = "Запуск обработки данных",
            //        LargeGlyph = "../Resources/Icons/system-run.png",
            //        SmallGlyph = "../Resources/Icons/system-run.png",
            //        Command = new RelayCommand(RunProcessing, CanRunProcessing),
            //        Page = "Обработка данных",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "makeTerritoryAccounts",
            //        Caption = "Сформировать счета на территории",
            //        LargeGlyph = "../Resources/Icons/run-build2.png",
            //        SmallGlyph = "../Resources/Icons/run-build2.png",
            //        Command = new RelayCommand(MakeTerritoryAccount, CanMakeTerritoryAccount),
            //        Page = "Межтерры",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "createDocuments",
            //        Caption = "Сформировать документы",
            //        LargeGlyph = "../Resources/Icons/folder-print.png",
            //        SmallGlyph = "../Resources/Icons/folder-print.png",
            //        Command = new RelayCommand(CreateDocuments, CanCreateDocuments),
            //        Page = "Отчеты",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "printDocuments",
            //        Caption = "Печать документов",
            //        LargeGlyph = "../Resources/Icons/printer.png",
            //        SmallGlyph = "../Resources/Icons/printer.png",
            //        Command = new RelayCommand(PrintDocuments, CanPrintDocuments),
            //        Page = "Отчеты",
            //    },
            //    new ContextCommand
            //    {
            //        Id = "acts",
            //        Caption = "Акты",
            //        Page = "Экспертизы",
            //        LargeGlyph = "../Resources/Icons/acts.png",
            //        SmallGlyph = "../Resources/Icons/acts.png",
            //        IsParent = true
            //    },
            //    new ContextCommand
            //    {
            //        Id = "addMeeAct",
            //        Caption = "Добавить акт МЭЭ",
            //        LargeGlyph = "../Resources/Icons/plus.png",
            //        SmallGlyph = "../Resources/Icons/plus.png",
            //        Parent = "acts",
            //        Command = new RelayCommand(AddMee, CanAddMee),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "addEqmaAct",
            //        Caption = "Добавить акт ЭКМП",
            //        LargeGlyph = "../Resources/Icons/plus.png",
            //        SmallGlyph = "../Resources/Icons/plus.png",
            //        Parent = "acts",
            //        Command = new RelayCommand(AddEqma, CanAddEqma),
            //    },
            //    new ContextCommand
            //    {
            //        Id = "viewActs",
            //        Caption = "Просмотр актов МЭЭ/ЭКМП",
            //        LargeGlyph = "../Resources/Icons/text-x-changelog.png",
            //        SmallGlyph = "../Resources/Icons/text-x-changelog.png",
            //        Parent = "acts",
            //        Command = new RelayCommand(ViewAct, CanViewAct),
            //    }
            //};

            var yearResult = _repository.GetAvailableYearsForMedicalAccounts();
            if (yearResult.Success)
            {
                YearItemsSource = new ObservableCollection<int>(yearResult.Data);
            }

            var monthsResult = _repository.GetAvailableMonthsForMedicalAccounts();
            if (monthsResult.Success)
            {
                MonthItemsSource = new ObservableCollection<CommonTuple>(monthsResult.Data);
            }
            UpdateFilter();
        }

        private bool CanPrintDocuments()
        {
            return CurrentRow != null && _repository.IsPreparedReportByExternalIdExists(CurrentRow.MedicalAccountId, Constants.ScopeLocalAccount);
        }

        private void PrintDocuments()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    PrintReportModel = scope.Resolve<PrintReportViewModel>(
                         new NamedParameter("id", CurrentRow.MedicalAccountId),
                         new NamedParameter("scope", Constants.ScopeLocalAccount));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, PrintReportModel, new PrintReportView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При печати документов для счета МО ID {0} произошла ошибка.".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }

        private bool CanCreateDocuments()
        {
            return CurrentRow != null;
        }

        private void CreateDocuments()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    RunReportModel = scope.Resolve<RunReportViewModel>(
                        new NamedParameter("id", CurrentRow.MedicalAccountId),
                        new NamedParameter("scope", Constants.ScopeLocalAccount),
                        new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, RunReportModel, new RunReportView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При формировании документов для счета МО ID {0} произошла ошибка.".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }


        public MedicalAccountView CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);

                _viewAccountCommand.RaiseCanExecuteChanged();
                _editAccountCommand.RaiseCanExecuteChanged();
                _showSummaryCommand.RaiseCanExecuteChanged();
                _viewAccountErrorCommand.RaiseCanExecuteChanged();
                _viewAccountSrzCommand.RaiseCanExecuteChanged();
                _viewExchangeCommand.RaiseCanExecuteChanged();
                _deleteAccountCommand.RaiseCanExecuteChanged();
                _runCheckCommand.RaiseCanExecuteChanged();
                _checkPolicyCommand.RaiseCanExecuteChanged();
                _runProcessingCommand.RaiseCanExecuteChanged();
                _createDocumentCommand.RaiseCanExecuteChanged();
                _printDocumentCommand.RaiseCanExecuteChanged();
                _addMeeCommand.RaiseCanExecuteChanged();
                _addEqmaCommand.RaiseCanExecuteChanged();
                _viewActCommand.RaiseCanExecuteChanged();

                if (_currentRow != null)
                {
                    if (CurrentRow.Version != null && Constants.ZmedicalVersion.Contains((int)CurrentRow.Version))
                    {
                        Expression<Func<ZslEventShortView, bool>> predicate = PredicateBuilder.True<ZslEventShortView>();
                        predicate = predicate.And(p => p.MedicalAccountId == _currentRow.MedicalAccountId);
                        using (var scope = Di.I.BeginLifetimeScope())
                        {
                            EventListSource = scope.Resolve<PLinqZEventList>(new NamedParameter("predicate", predicate));
                        }
                    }
                    else
                    {
                        Expression<Func<EventShortView, bool>> predicate = PredicateBuilder.True<EventShortView>();
                        predicate = predicate.And(p => p.MedicalAccountId == _currentRow.MedicalAccountId);
                        using (var scope = Di.I.BeginLifetimeScope())
                        {
                            EventListSource = scope.Resolve<PLinqEventList>(new NamedParameter("predicate", predicate));
                        }
                    }

                }
            }
        }
       

        public PropertyGridViewModel<EditMedicalAccountViewModel> MedicalAccount
        {
            get { return _medicalAccount; }
            set
            {
                _medicalAccount = value;
                RaisePropertyChanged(() => MedicalAccount);
            }
        }

        public RunExaminationViewModel RunExaminationModel
        {
            get { return _runExaminationModel; }
            set
            {
                _runExaminationModel = value;
                RaisePropertyChanged(() => RunExaminationModel);
            }
        }

        public MakeTerritoryAccountsViewModel MakeTerritoryAccountsModel
        {
            get { return _makeTerritoryAccountsModel; }
            set
            {
                _makeTerritoryAccountsModel = value;
                RaisePropertyChanged(() => MakeTerritoryAccountsModel);
            }
        }
        public CheckPolicyInTortillaViewModel TortillaCheckModel
        {
            get { return _tortillaCheckModel; }
            set
            {
                _tortillaCheckModel = value;
                RaisePropertyChanged(() => TortillaCheckModel);
            }
        }
       
        public RunProcessingViewModel RunProcessingModel
        {
            get { return _runProcessingModel; }
            set
            {
                _runProcessingModel = value;
                RaisePropertyChanged(() => RunProcessingModel);
            }
        }

        public int? SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                RaisePropertyChanged("SelectedYear");
                UpdateFilter();
            }
        }

        public int? SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                _selectedMonth = value;
                RaisePropertyChanged("SelectedMonth");
                UpdateFilter();
            }
        }

        public int? SelectedMedicalOrganization
        {
            get { return _selectedMedicalOrganization; }
            set
            {
                _selectedMedicalOrganization = value;
                RaisePropertyChanged("SelectedMedicalOrganization");
                UpdateFilter();
            }
        }

        //private ObservableCollection<object> source = EventListSource;
        private readonly ObservableCollection<object> selection = new ObservableCollection<object>();
        //public ObservableCollection<object> Source { get { return this.EventListSource; } }
        public ObservableCollection<object> Selection { get { return this.selection; } }


        //private readonly ObservableCollection<object> selection = new ObservableCollection<object>();
        //public ObservableCollection<object> Selection { get { return this.selection; } }

        public object EventListSource
        {
            get { return _eventListSource; }
            set
            {
                if (_eventListSource == value) return;
                _eventListSource = value;
                RaisePropertyChanged("EventListSource");
            }
        }

        public PLinqMedicalAccountList AccountListSource
        {
            get { return _accountListSource; }
            set
            {
                if (_accountListSource == value) return;
                _accountListSource = value;
                RaisePropertyChanged("AccountListSource");
            }
        }

        private bool CanShowSummary()
        {
            return CurrentRow != null && CurrentRow.GetType() != typeof(DevExpress.Data.NotLoadedObject);
        }

        private void ShowSummary()
        {


            try
            {
                var t = _overlayManager.ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);

                t.ContinueWith(p =>
                {
                    var summary = Di.I.Resolve<MedicalAccountSummaryViewModel>(
                        new NamedParameter("accountId", CurrentRow.MedicalAccountId),
                        new NamedParameter("version", CurrentRow.Version));

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, summary,
                            new TerritoryAccountSummaryView());

                        view.OkCallback = () =>
                        {
                            view.Close();
                        };
                        view.ShowDialog();
                    });
                }).ContinueWith(p => _overlayManager.HideOverlay());
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при показе саммари территориального счета ID {0}", typeof(MedicalAccountViewModel), CurrentRow.MedicalAccountId);
            }
           
        }

        private bool CanViewAccountError()
        {
            if (CurrentRow.IsNotNull())
            {
                TransactionResult<bool> countResult;
                if (CurrentRow.Version != null && Constants.ZmedicalVersion.Contains((int)CurrentRow.Version))
                {
                    countResult = _repository.IsErrorForZMedicalAccountExist(CurrentRow.MedicalAccountId);
                }
                else
                {
                    countResult = _repository.IsErrorForMedicalAccountExist(CurrentRow.MedicalAccountId);
                }
                return countResult.Success && countResult.Data;
            }
            return false;
        }

        private void ViewAccountError()
        {
            try
            {
                if (CurrentRow.Version != null && Constants.ZmedicalVersion.Contains((int)CurrentRow.Version))
                {
                    _dockManager.ShowOperator(CurrentRow.MedicalAccountId, OperatorMode.ZLocalError);
                }
                else
                {
                    _dockManager.ShowOperator(CurrentRow.MedicalAccountId, OperatorMode.LocalError);
                }

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии позиций счета МО ID {0}".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }

        private void DeleteAcountByPeriod()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<DeleteByPeriodViewModel>();
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, model, new DeleteByPeriodView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при удалении счетов по периоду", typeof(MedicalAccountViewModel));
            }
        }

        private bool CanViewAccountSrz()
        {
            if (CurrentRow.IsNotNull() && CurrentRow.Status < 3)
            {
                var countResult = _repository.IsSrzQueriesForMedicalAccountExist(CurrentRow.MedicalAccountId);
                return countResult.Success && countResult.Data;
            }
            return false;
        }

        private void ViewAccountSrz()
        {
            try
            {
                _dockManager.ShowOperator(CurrentRow.MedicalAccountId, OperatorMode.LocalSrzQuery);

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии позиций счета МО ID {0}".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }

        private bool CanRunCheck()
        {
            return CurrentRow != null && CurrentRow.Status < 3;
        }

        private void RunCheck()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    RunExaminationModel = scope.Resolve<RunExaminationViewModel>(
                        new NamedParameter("id", CurrentRow.MedicalAccountId),
                        new NamedParameter("scope", Constants.ScopeLocalAccount),
                        new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, RunExaminationModel, new RunExaminationView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При запуске проверок для счета МО ID {0} произошла ошибка.".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }


        public string Error
        {
            get
            {
                RaisePropertyChanged("Visibility");
                return _dataErrorInfoSupport.Error;
            }
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }

        string IDataErrorInfo.this[string memberName]
        {
            get { return _dataErrorInfoSupport[memberName]; }
        }

        #endregion

        private void UpdateFilter()
        {
            Expression<Func<MedicalAccountView, bool>> predicate = PredicateBuilder.True<MedicalAccountView>();
            if (_selectedYear.HasValue)
            {
                predicate = predicate.And(p => p.Date.Value.Year == _selectedYear);
            }

            if (_selectedMonth.HasValue)
            {
                predicate = predicate.And(p => p.Date.Value.Month == _selectedMonth);
            }

            if (_selectedMedicalOrganization.HasValue)
            {
                predicate = predicate.And(p => p.MedicalOrganization == _selectedMedicalOrganization);
            }

            using (var scope = Di.I.BeginLifetimeScope())
            {
                AccountListSource = scope.Resolve<PLinqMedicalAccountList>(new NamedParameter("predicate", predicate));
            }
        }

        private void RefreshAccount()
        {
            UpdateFilter();
        }

        private bool CanDeleteAcount()
        {
            if (CurrentRow.IsNull() || CurrentRow.GetType() == typeof(DevExpress.Data.NotLoadedObject))
            {
                return false;
            }

            var isTerritoryAccountExistsResult = _repository.IsTerritoryAccountExistsForMedicalAccount(CurrentRow.MedicalAccountId);
            if (isTerritoryAccountExistsResult.HasError)
            {
                //TODO log
            }

            return !isTerritoryAccountExistsResult.Data;
        }

        private void DeleteAcount()
        {
            try
            {
                if (_messageService.AskQuestion("Вы действительно хотите удалить счет МО?", "Внимание!"))
                {
                    Task.Factory.StartNew(() => _dockManager.ShowOverlay(Constants.AccountDeleteTitleMsg, Constants.PleaseWaitMsg))
                    .ContinueWith(p =>
                    {

                        if (Constants.Zversion.Contains((int)CurrentRow.Version)) //Удалем счет с законченным случаем
                        {
                            var deleteResult = _repository.DeleteZMedicalAccount(CurrentRow.MedicalAccountId);
                            if (deleteResult.Success)
                            {
                                RefreshAccount();
                                _notifyManager.ShowNotify("Счет МО успешно удален.");
                            }
                            else
                            {
                                //TODO log
                                _messageService.ShowException(deleteResult.LastError, "При попытке удалить счет МО ID {0} произошла ошибка.".F(CurrentRow.MedicalAccountId), typeof(TerritoryAccountViewModel));
                            }
                        }
                        else
                        {
                            var deleteResult = _repository.DeleteMedicalAccount(CurrentRow.MedicalAccountId);
                            if (deleteResult.Success)
                            {
                                RefreshAccount();
                                _notifyManager.ShowNotify("Счет МО успешно удален.");
                            }
                            else
                            {
                                //TODO log
                                _messageService.ShowException(deleteResult.LastError, "При попытке удалить счет МО ID {0} произошла ошибка.".F(CurrentRow.MedicalAccountId), typeof(TerritoryAccountViewModel));
                            }
                        }

                    })
                    .ContinueWith(p => _dockManager.HideOverlay());
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке удалить счет МО ID {0} произошла ошибка.".F(CurrentRow.MedicalAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanViewAccount()
        {
            return CurrentRow != null && CurrentRow.GetType() != typeof(DevExpress.Data.NotLoadedObject);
        }

        private void ViewAccount()
        {
            try
            {
                if (CurrentRow.Version != null && Constants.ZmedicalVersion.Contains((int)CurrentRow.Version))
                {
                    if (((PLinqZEventList) EventListSource).Any)
                    {
                        _dockManager.ShowOperator(CurrentRow.MedicalAccountId, OperatorMode.Zlocal);
                    }
                    else
                    {
                        _notifyManager.ShowNotify("Нет данных для просмотра.");
                    }
                }
                else
                {
                    if (((PLinqEventList) EventListSource).Any)
                        _dockManager.ShowOperator(CurrentRow.MedicalAccountId, OperatorMode.Local);
                    else
                    {
                        _notifyManager.ShowNotify("Нет данных для просмотра.");
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception,
                    "Исключение при открытии позиций счета МО ID {0}".F(CurrentRow.MedicalAccountId),
                    typeof (MedicalAccountViewModel));
            }
        }

        
        private bool CanCreateAccount()
        {
            return CurrentRow != null;
        }

        private void CreateAccount()
        {
            dynamic v = _remoteSettings.Get(AppRemoteSettings.DefaultOmsVersion);
            dynamic clientMo = _remoteSettings.Get(AppRemoteSettings.ClientMo);
            int version = v.version;
            string mcod = clientMo.Mcod;

            var model =
                new PropertyGridViewModel<EditMedicalAccountViewModel>(
                    new EditMedicalAccountViewModel(new FactMedicalAccount
                    {
                        MedicalOrganization = _cacheRepository.Get("F003Cache").GetBack(mcod),
                        Date = DateTime.Now,
                        AccountDate = DateTime.Now,
                        Version = version - 1,
                        MECPenalties = 00,
                        MEEPenalties = 00,
                        EQMAPenalties = 00,
                        Price = 00,
                        AcceptPrice = 00,
                        Status = 1

                    }));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactMedicalAccount>(model.SelectedObject.Classifier);

                var result = _repository.InsertOrUpdateMedicalAccount(tmp);
                if (result.Success)
                {
                    RefreshAccount();
                    _notifyManager.ShowNotify("Счет успешно создан.");
                    view.Close();
                }

            };
            view.ShowDialog();
        }



        private bool CanEditAccount()
        {
            return CurrentRow != null;
        }

        private void EditAccount()
        {
            var copy = Map.ObjectToObject<FactMedicalAccount>(CurrentRow);
            var model = new PropertyGridViewModel<EditMedicalAccountViewModel>(new EditMedicalAccountViewModel(copy));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactMedicalAccount>(model.SelectedObject.Classifier);

                var result = _repository.Update(tmp);
                if (result.Success)
                {
                    var map = model.SelectedObject.MapObject<MedicalAccountView>(CurrentRow);
                    CurrentRow.Update(map.Affected);
                    _notifyManager.ShowNotify("Данные счета успешно обновлены.");
                    view.Close();
                }

            };

            view.ShowDialog();
        }

        private bool CanTortillaCheck()
        {
            return CurrentRow != null && CurrentRow.GetType() != typeof(DevExpress.Data.NotLoadedObject);
        }

        private void TortillaCheck()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    TortillaCheckModel = scope.Resolve<CheckPolicyInTortillaViewModel>(new NamedParameter("id", CurrentRow.MedicalAccountId),
                         new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, TortillaCheckModel, new CheckPolicyInTortillaView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке проверить документы ОМС в Тортиле, счет ID {0}", typeof(MedicalAccountViewModel), CurrentRow.MedicalAccountId);
            }


        }

        private bool CanMakeTerritoryAccount()
        {

            //TODO add check unmake patients/events
            return true;
        }

        private void MakeTerritoryAccount()
        {

            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    MakeTerritoryAccountsModel =
                        scope.Resolve<MakeTerritoryAccountsViewModel>(new NamedParameter("mode", OperatorMode.Zlocal));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, MakeTerritoryAccountsModel, new MakeTerritoryAccountsView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Ошибка при попытке сформировать счета на территории", typeof(MedicalAccountViewModel));
            }
        }

        private bool CanViewExchange()
        {
            return CurrentRow.IsNotNull();
        }

        private void ViewExchange()
        {
            try
            {
                _dockManager.ShowExchangeLocal(CurrentRow.MedicalAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии журнала информационного взаимодействия для счета МО ID {0}".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }

        private bool CanRunProcessing()
        {
            return CurrentRow.IsNotNull();
        }

        private void RunProcessing()
        {

            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    RunProcessingModel = scope.Resolve<RunProcessingViewModel>(
                        new NamedParameter("id", CurrentRow.MedicalAccountId),
                        new NamedParameter("scope", Constants.ScopeLocalAccount),
                        new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, RunProcessingModel, new RunProcessingView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке запуска функция обработки данных, счет ID {0}", typeof(MedicalAccountViewModel), CurrentRow.MedicalAccountId);
            }
        }

        private bool CanAddEqma()
        {
            return CurrentRow.IsNotNull() && _repository.IsSankWithoutActExistsForMedicalAccount(CurrentRow.MedicalAccountId); ;
        }

        private void AddEqma()
        {


            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    EqmaModel = scope.Resolve<EqmaActViewModel>(
                        new NamedParameter("accountId", CurrentRow.MedicalAccountId),
                        new NamedParameter("scope", Constants.ScopeLocalAccount));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, EqmaModel, new EqmaView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке добавления акта ЭКМП", typeof(MedicalAccountViewModel));
            }



            //try
            //{
            //    IsEqmaOpen = true;
            //    using (var scope = Di.I.BeginLifetimeScope())
            //    {
            //        EqmaModel = scope.Resolve<EqmaActViewModel>(
            //            new NamedParameter("accountId", CurrentRow.MedicalAccountId),
            //            new NamedParameter("scope", Constants.ScopeLocalAccount));

            //        EqmaModel.OkCallback = () =>
            //        {
            //            _notifyManager.ShowNotify("Акт ЭКМП успешно добавлен.");
            //            IsEqmaOpen = false;
            //        };

            //        EqmaModel.CancelCallback = () =>
            //        {
            //            IsEqmaOpen = false;
            //        };
            //    }
            //}
            //catch (Exception exception)
            //{
            //    _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке добавления акта ЭКМП", typeof(MedicalAccountViewModel));
            //}
        }

        private bool CanAddMee()
        {
            return CurrentRow.IsNotNull() && _repository.IsMeeWithoutActExistsForMedicalAccount(CurrentRow.MedicalAccountId);
        }

        private void AddMee()
        {
            try
            {
                IsMeeOpen = true;
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    MeeModel = scope.Resolve<MeeActViewModel>(
                        new NamedParameter("accountId", CurrentRow.MedicalAccountId),
                        new NamedParameter("scope", Constants.ScopeLocalAccount));

                    MeeModel.OkCallback = () =>
                    {
                        _notifyManager.ShowNotify("Акт МЭЭ успешно добавлен.");
                        IsMeeOpen = false;
                    };

                    MeeModel.CancelCallback = () =>
                    {
                        IsMeeOpen = false;
                    };
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке добавления акта МЭЭ", typeof(MedicalAccountViewModel));
            }
        }

        private bool CanViewAct()
        {
            return CurrentRow.IsNotNull() && (_repository.IsMeeWithActExistsForMedicalAccount(CurrentRow.MedicalAccountId) || _repository.IsEqmaWithActExistsForMedicalAccount(CurrentRow.MedicalAccountId));
        }

        private void ViewAct()
        {
            try
            {
                _dockManager.ShowActs(Constants.ScopeLocalAccount, CurrentRow.MedicalAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии актов МЭЭ для территориального счета ID {0}".F(CurrentRow.MedicalAccountId), typeof(MedicalAccountViewModel));
            }
        }

    }



    public class DataItem
    {
        public static ObservableCollection<object> GetData(int count = 100)
        {
            ObservableCollection<object> result = new ObservableCollection<object>();
            for (int i = 0; i < count; i++)
                result.Add(new DataItem(i, "Name" + i, DateTime.Now.AddDays(i)));
            return result;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date_Time { get; set; }
        public DataItem() { }
        public DataItem(int id, string name, DateTime date)
        {
            Id = id;
            Name = name;
            Date_Time = date;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}