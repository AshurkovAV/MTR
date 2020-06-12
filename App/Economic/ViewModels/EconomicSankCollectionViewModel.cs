using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using BLToolkit.Mapping;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Linq;
using Core.Services;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Medical.AppLayer.Economic.Views;
using Medical.AppLayer.Examination.ViewModels;
using Medical.AppLayer.Examination.Views;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.OperatorModels;
using Medical.AppLayer.Processing.ViewModels;
using Medical.AppLayer.Processing.Views;
using Medical.AppLayer.Register.ViewModels;
using Medical.AppLayer.Register.Views;
using Medical.AppLayer.Report.ViewModels;
using Medical.AppLayer.Report.Views;
using Medical.AppLayer.Services;
using Medical.CoreLayer.PropertyGrid;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using CommonWindow = Medical.AppLayer.Editors.CommonWindow;
using CommonWindowControl = Medical.AppLayer.Editors.CommonWindowControl;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EconomicSankCollectionViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly IDockLayoutManager _dockManager;
        private readonly INotifyManager _notifyManager;
        private readonly IOverlayManager _overlayManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private FactTerritoryAccount _currentRow;

        private object _eventListSource;
        private PLinqAccountList _accountListSource;
     
        private int? _selectedDirection;
        private ZslEventShortView _selecttedEventShortView;
        private string _selectedTerritory;
        private string _selectedYear;
        private int _idAct;

        private RelayCommand _resetTerritoryCommand;
        private RelayCommand _resetDirectionCommand;

        private RelayCommand _viewAccountCommand;
        private RelayCommand _viewAccountListCommand;
        private RelayCommand _viewContextMenuAccountCommand;
        private RelayCommand _viewContextMenuAccountCheckEventCommand; 
        private RelayCommand _refreshEventListCommand;
        private RelayCommand _editAccountCommand;
        private RelayCommand _showSummaryCommand;
        private RelayCommand _viewAccountErrorCommand;
        private RelayCommand _viewAccountSrzCommand;
        private RelayCommand _reloadCommand;
        private RelayCommand _viewExchangeCommand;
        private RelayCommand _deleteAccountCommand;
        private RelayCommand _breakUpAccountCommand;
        private RelayCommand _breakUpByPeriodAccountCommand;
        private RelayCommand<object> _changeStatusCommand;
        private RelayCommand _exportAccountCommand;
        private RelayCommand _runCheckCommand;
        private RelayCommand _checkPolicyCommand;
        private RelayCommand _runProcessingCommand;
        private RelayCommand _createDocumentCommand;
        private RelayCommand _printDocumentCommand;
        private RelayCommand _addMeeCommand;
        private RelayCommand _addEqmaCommand;
        private RelayCommand _viewActCommand;
        private RelayCommand _addPaymentCommand;
        private RelayCommand _viewPaymentCommand;
        private RelayCommand _addRefuseCommand;
        private RelayCommand _viewRefuseCommand;
        private RelayCommand _addSurchargeCommand;
        private RelayCommand _viewSurchargeCommand;
        private RelayCommand _saveFiltrCommand;
        private RelayCommand _exportExcelCommand;

        #region IHash
        public string Hash => typeof(EconomicSankCollectionViewModel).FullName;

        #endregion

        public EconomicSankCollectionViewModel( 
            IMessageService messageService, 
            IMedicineRepository repository, 
            IDockLayoutManager dockManager, 
            INotifyManager notifyManager,
            IOverlayManager overlayManager,
            int idAct)
        {
            _messageService = messageService;
            _repository = repository;
            _dockManager = dockManager;
            _notifyManager = notifyManager;
            _overlayManager = overlayManager;
            _idAct = idAct;
            Initialize();
            SelectedYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            SelectedDirection = 0;
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            Expression<Func<SankShortView, bool>> predicate = PredicateBuilder.True<SankShortView>();
            predicate = predicate.And(p => _idAct != null && p.ActExpertiseId == _idAct);
            using (var scope = Di.I.BeginLifetimeScope())
            {
                EventListSource =
                    scope.Resolve<PLinqSankList>(new NamedParameter("predicate", predicate));
            }
        }

        public ICommand ViewAccountCommand => _viewAccountCommand ??
                                              (_viewAccountCommand = new RelayCommand(ViewAccount, CanViewAccount));

        public ICommand ViewAccountListCommand => _viewAccountListCommand ??
                                              (_viewAccountListCommand = new RelayCommand(ViewAccountList, CanViewAccountList));

        public ICommand ViewContextMenuAccountCommand
        {
            get
            {
                return _viewContextMenuAccountCommand ??
                       (_viewContextMenuAccountCommand = new RelayCommand(OpenAccount, CanOpenAccount));
            }
        }

        public ICommand ViewContextMenuAccountCheckEventCommand
        {
            get
            {
                return _viewContextMenuAccountCheckEventCommand ??
                       (_viewContextMenuAccountCheckEventCommand = new RelayCommand(OpenAccountCheck, CanOpenAccountCheck));
            }
        }

        public ICommand RefreshEventListCommand
        {
            get
            {
                return _refreshEventListCommand??
                       (_refreshEventListCommand = new RelayCommand(RefreshEvent));
            }
        }
        public ICommand EditAccountCommand => _editAccountCommand ??
                                              (_editAccountCommand = new RelayCommand(EditAccount, CanEditAccount));
        public ICommand ShowSummaryCommand => _showSummaryCommand ??
                                              (_showSummaryCommand = new RelayCommand(ShowSummary, CanShowSummary));
        public ICommand ViewAccountErrorCommand => _viewAccountErrorCommand ??
                                             (_viewAccountErrorCommand = new RelayCommand(ViewAccountError, CanViewAccountError));
        public ICommand ViewAccountSrzCommand => _viewAccountSrzCommand ??
                                             (_viewAccountSrzCommand = new RelayCommand(ViewAccountSrz, CanViewAccountSrz));
        public ICommand ReloadCommand => _reloadCommand ??
                                                     (_reloadCommand = new RelayCommand(RefreshAccount));
        public ICommand ViewExchangeCommand => _viewExchangeCommand ??
                                             (_viewExchangeCommand = new RelayCommand(ViewExchange, CanViewExchange));
        public ICommand DeleteAccountCommand => _deleteAccountCommand ??
                                             (_deleteAccountCommand = new RelayCommand(DeleteAcount, CanDeleteAcount));
        public ICommand BreakUpAccountCommand => _breakUpAccountCommand ??
                                             (_breakUpAccountCommand = new RelayCommand(BreakUpAcount, CanBreakUpAcount));
        public ICommand BreakUpByPeriodAccountCommand => _breakUpByPeriodAccountCommand ??
                                             (_breakUpByPeriodAccountCommand = new RelayCommand(BreakUpAcountByPeriod));
        public ICommand ChangeStatusCommand => _changeStatusCommand ??
                                            (_changeStatusCommand = new RelayCommand<object>(ChangeStatus, CanChangeStatus));
        public ICommand ExportAccountCommand => _exportAccountCommand ??
                                             (_exportAccountCommand = new RelayCommand(ExportXml, CanExportXml));
        public ICommand RunCheckCommand => _runCheckCommand ??
                                             (_runCheckCommand = new RelayCommand(RunCheck, CanRunCheck));
        public ICommand CheckPolicyCommand => _checkPolicyCommand ??
                                             (_checkPolicyCommand = new RelayCommand(CheckPolicy, CanCheckPolicy));
        public ICommand RunProcessingCommand => _runProcessingCommand ??
                                             (_runProcessingCommand = new RelayCommand(RunProcessing, CanRunProcessing));
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
        public ICommand AddPaymentCommand => _addPaymentCommand ??
                                             (_addPaymentCommand = new RelayCommand(AddPayment, CanAddPayment));
        public ICommand ViewPaymentCommand => _viewPaymentCommand ??
                                             (_viewPaymentCommand = new RelayCommand(ViewPayments, CanViewPayments));
        public ICommand AddRefuseCommand => _addRefuseCommand ??
                                             (_addRefuseCommand = new RelayCommand(AddRefuse, CanAddRefuse));
        public ICommand ViewRefuseCommand => _viewRefuseCommand ??
                                             (_viewRefuseCommand = new RelayCommand(ViewRefuse, CanViewRefuse));
        public ICommand AddSurchargeCommand => _addSurchargeCommand ??
                                             (_addSurchargeCommand = new RelayCommand(AddSurcharge, CanAddSurcharge));
        public ICommand ViewSurchargeCommand => _viewSurchargeCommand ??
                                             (_viewSurchargeCommand = new RelayCommand(ViewSurcharge, CanViewSurcharge));
        public ICommand SaveFiltrCommand => _saveFiltrCommand ??
                                             (_saveFiltrCommand = new RelayCommand(ViewSurcharge, CanViewSurcharge));

        public ICommand ExportExcelCommand => _exportExcelCommand ??
                                            (_exportExcelCommand = new RelayCommand(ExportExcel));

        private void SaveFiltr()
        {
            try
            {
                _dockManager.ShowEconomicSurcharge(CurrentRow.TerritoryAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии доплат для территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        public ObservableCollection<int> YearItemsSource { get; set; }

        public FactTerritoryAccount CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(()=>CurrentRow);

                _viewAccountCommand.RaiseCanExecuteChanged();
                _editAccountCommand.RaiseCanExecuteChanged();
                _showSummaryCommand.RaiseCanExecuteChanged();
                _viewAccountErrorCommand.RaiseCanExecuteChanged();
                _viewAccountSrzCommand.RaiseCanExecuteChanged();
                _viewExchangeCommand.RaiseCanExecuteChanged();
                _deleteAccountCommand.RaiseCanExecuteChanged();
                _breakUpAccountCommand.RaiseCanExecuteChanged();
                _changeStatusCommand.RaiseCanExecuteChanged();
                _exportAccountCommand.RaiseCanExecuteChanged();
                _runCheckCommand.RaiseCanExecuteChanged();
                _checkPolicyCommand.RaiseCanExecuteChanged();
                _runProcessingCommand.RaiseCanExecuteChanged();
                _createDocumentCommand.RaiseCanExecuteChanged();
                _printDocumentCommand.RaiseCanExecuteChanged();
                _addMeeCommand.RaiseCanExecuteChanged();
                _addEqmaCommand.RaiseCanExecuteChanged();
                _viewActCommand.RaiseCanExecuteChanged();

                RefreshEventList();
            }
        }

        private void RefreshEventList()
        {
            if (_currentRow != null)
            {
                if (Constants.ZterritoryVersionNull.Contains(CurrentRow.Version))
                {
                    //Expression<Func<ZslEventShortView, bool>> predicate = PredicateBuilder.True<ZslEventShortView>();
                    //predicate = predicate.And(p => p.AccountId == _currentRow.TerritoryAccountId);
                    //using (var scope = Di.I.BeginLifetimeScope())
                    //{
                    //    EventListSource =
                    //        scope.Resolve<PLinqZEventExtendedList>(new NamedParameter("predicate", predicate));
                    //}
                }
            }
        }

        public string SelectedTerritory
        {
            get { return _selectedTerritory; }
            set
            {
                _selectedTerritory = value;
                RaisePropertyChanged();
                UpdateFilter();
            }
        }

        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                RaisePropertyChanged();
                UpdateFilter();
            }
        }

        public int? SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                RaisePropertyChanged();
                UpdateFilter();
            }
        }

        public ZslEventShortView SelectedEventShortView
        {
            get { return _selecttedEventShortView;  }
            set
            {
                _selecttedEventShortView = value;
                RaisePropertyChanged();
            }
        }


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

        public PLinqAccountList AccountListSource
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
            return CurrentRow != null;
        }


        private void ShowSummary()
        {
            try
            {
                var t = _overlayManager.ShowOverlay(Constants.OperationBeginTitleMsg ,Constants.PleaseWaitMsg);

                    t.ContinueWith(p =>
                    {
                        var summary = Di.I.Resolve<TerritoryAccountSummaryViewModel>(
                            new NamedParameter("accountId", CurrentRow.TerritoryAccountId),
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
                _messageService.ShowExceptionFormatted(exception, "Ошибка при показе саммари территориального счета ID {0}", typeof(TerritoryAccountViewModel), CurrentRow.TerritoryAccountId);
            }
        }

        private bool CanChangeStatus(object sender)
        {
            return CurrentRow != null;
        }

        private void ChangeStatus(object sender)
        {
            if (CurrentRow != null)
            {
                var status = sender.ToInt32Nullable();
                var result = _repository.ChangeTerritoryAccountStatus(CurrentRow.TerritoryAccountId, status);
                if (result.Success)
                {
                    CurrentRow.Status = status;
                    CurrentRow.UpdateProperty("Status");
                }
                else
                {
                    _messageService.ShowErrorFormatted("При изменении статуса межтерриториального счета ID {0} произошла ошибка.", CurrentRow.TerritoryAccountId);
                }
            }
        }

        private bool CanAddPayment()
        {
            return CurrentRow != null;
        }

        private void AddPayment()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<AddPaymentViewModel>(
                        new NamedParameter("accountId", CurrentRow.TerritoryAccountId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new AddPaymentView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При добавлении платежа для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanAddSurcharge()
        {
            return CurrentRow != null;
        }

        private void AddSurcharge()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<AddSurchargeViewModel>(
                        new NamedParameter("accountId", CurrentRow.TerritoryAccountId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new AddSurchargeView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При добавлении/редактировании доплаты для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanAddRefuse()
        {
            return CurrentRow != null;
        }

        private void AddRefuse()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<AddRefuseViewModel>(
                        new NamedParameter("accountId", CurrentRow.TerritoryAccountId), new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new AddRefuseView());
                    view.ShowDialog();

                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При добавлении/редактировании отказа для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanPrintDocuments()
        {
            return true;
        }

        private void PrintDocuments()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<PrintReportViewModel>(
                        new NamedParameter("id", CurrentRow.TerritoryAccountId),
                        new NamedParameter("subId", CurrentRow.TerritoryAccountId),
                        new NamedParameter("scope", Constants.ScopeInterTerritorialAccount));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new PrintReportView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При печати документов для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanViewAccountError()
        {
            if (!CurrentRow.IsNotNull()) return false;
            TransactionResult<bool> countResult;
            if (CurrentRow.Version == Constants.Version30 || CurrentRow.Version == Constants.Version31 || CurrentRow.Version == Constants.Version32)
            {
                countResult = _repository.IsErrorForZTerritoryAccountExist(CurrentRow.TerritoryAccountId); 
            }
            else
            {
                countResult = _repository.IsErrorForTerritoryAccountExist(CurrentRow.TerritoryAccountId);
            }
            return countResult.Success && countResult.Data;
        }

        private void ViewAccountError()
        {
            try
            {
                if (CurrentRow.Version == Constants.Version30 || CurrentRow.Version == Constants.Version31 || CurrentRow.Version == Constants.Version32)
                {
                    _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.ZInterTerritorialError);
                }
                else
                {
                    _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.InterTerritorialError);
                }

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии позиций территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanViewAccountSrz()
        {
            if (CurrentRow.IsNotNull() && CurrentRow.Status < 3)
            {
                var countResult = _repository.IsSrzQueriesForTerritoryAccountExist(CurrentRow.TerritoryAccountId);
                return countResult.Success && countResult.Data;
            }
            return false;
        }

        private void ViewAccountSrz()
        {
            try
            {
                _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.InterTerritorialSrzQuery);

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии позиций территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        public ICommand ResetTerritoryCommand => _resetTerritoryCommand ??
                                                 (_resetTerritoryCommand = new RelayCommand(ResetTerritory, CanResetTerritory));

        public ICommand ResetDirectionCommand => _resetDirectionCommand ??
                                                 (_resetDirectionCommand = new RelayCommand(ResetDirection, CanResetDirection));

        private bool CanResetDirection()
        {
            return _selectedDirection.HasValue;
        }

        private void ResetDirection()
        {
            SelectedDirection = 0;
        }


        private bool CanResetTerritory()
        {
            return !string.IsNullOrWhiteSpace(_selectedTerritory);
        }

        private void ResetTerritory()
        {
            SelectedTerritory = null;
        }

        private bool CanCheckPolicy()
        {
            return CurrentRow != null && CurrentRow.Status < 3;
        }

        private void CheckPolicy()
        {
            try
            {
                var model = Di.I.Resolve<CheckPolicyViewModel>(new NamedParameter("id", CurrentRow.TerritoryAccountId),
                    new NamedParameter("version", CurrentRow.Version));
                var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new CheckPolicyView());
                view.ShowDialog();
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При проверке полисов межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
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
                    var model = scope.Resolve<RunExaminationViewModel>(
                        new NamedParameter("id", CurrentRow.TerritoryAccountId),
                        new NamedParameter("scope", Constants.ScopeInterTerritorialAccount),
                        new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, model, new RunExaminationView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При запуске проверок для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
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
            Expression<Func<FactTerritoryAccount, bool>> predicate = PredicateBuilder.True<FactTerritoryAccount>();
            int yearResult;
            if (int.TryParse(_selectedYear, out yearResult))
            {
                predicate = predicate.And(p => p.Date.Value.Year == yearResult);
            }
            if (SelectedDirection.HasValue && SelectedDirection > 0)
            {
                var direction = _selectedDirection - 1;
                predicate = predicate.And(p => p.Direction == direction);
            }
            if (!string.IsNullOrWhiteSpace(_selectedTerritory))
            {
                predicate = predicate.And(p => p.Source == _selectedTerritory || p.Destination == _selectedTerritory);
            }

            AccountListSource = Di.I.Resolve<PLinqAccountList>(new NamedParameter("predicate", predicate));
        }

        private void RefreshAccount()
        {
            UpdateFilter();
        }

        private bool CanExportXml()
        {
            return CurrentRow != null;
        }

        private void ExportXml()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<ExportXmlViewModel>(
                       new NamedParameter("id", CurrentRow.TerritoryAccountId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new ExportXmlView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При выгрузке файла OMS для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanCreateDocuments()
        {
            return CurrentRow != null && CurrentRow.Status < 3;
        }

        private void CreateDocuments()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<RunReportViewModel>(
                        new NamedParameter("id", CurrentRow.TerritoryAccountId),
                        new NamedParameter("scope", Constants.ScopeInterTerritorialAccount),
                        new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new RunReportView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При формировании документов для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanDeleteAcount()
        {
            return CurrentRow != null && CurrentRow.Status < 3 && !_repository.IsTerritoryAccountHaveMedicalAccount(CurrentRow.TerritoryAccountId);
        }

        private void DeleteAcount()
        {
            try
            {
                if (_messageService.AskQuestion("Вы действительно хотите удалить счет?", "Внимание!"))
                {
                    Task.Factory.StartNew(() => _dockManager.ShowOverlay(Constants.AccountDeleteTitleMsg, Constants.PleaseWaitMsg))
                    .ContinueWith(p =>
                    {
                        var deleteResult = _repository.DeleteTerritoryAccount(CurrentRow.TerritoryAccountId);
                        if (deleteResult.Success)
                        {
                            RefreshAccount();
                            _notifyManager.ShowNotify("Счет успешно удален.");
                        }
                        else
                        {
                            _messageService.ShowException(deleteResult.LastError, "При попытке удалить межтерриториальный счет ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
                        }
                    })
                    .ContinueWith(p => _dockManager.HideOverlay());
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке удалить межтерриториальный счет ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanBreakUpAcount()
        {
            return CurrentRow != null && CurrentRow.Status < 3 && (DirectionType?)CurrentRow.Direction == DirectionType.Out;
        }

        private void BreakUpAcount()
        {
            try
            {
                if (_messageService.AskQuestion("Вы действительно хотите расформировать счет?", "Внимание!"))
                {
                    Task.Factory.StartNew(() => _dockManager.ShowOverlay(Constants.AccountBreakUpTitleMsg, Constants.PleaseWaitMsg))
                    .ContinueWith(p =>
                    {

                        var breakUpResult = _repository.BreakUpTerritoryAccount(CurrentRow.TerritoryAccountId);
                        if (breakUpResult.Success)
                        {
                            RefreshAccount();
                            _notifyManager.ShowNotify("Счет успешно расформирован.");
                        }
                        else
                        {
                            _messageService.ShowException(breakUpResult.LastError,
                                "При попытке расформировать межтерриториальный счет ID {0} произошла ошибка.".F(
                                    CurrentRow.TerritoryAccountId), typeof (TerritoryAccountViewModel));
                        }


                    })
                    .ContinueWith(p => _dockManager.HideOverlay());
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке расформировать межтерриториальный счет ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private void RefreshEvent()
        {
            RefreshEventList();
        }

        private bool CanOpenAccount()
        {
            return SelectedEventShortView.IsNotNull() && SelectedEventShortView.AccountId.HasValue &&
                   SelectedEventShortView.PatientId.HasValue && SelectedEventShortView.ZslMedicalEventId.IsNotNull();
        }

        private void OpenAccount()
        {
            if (SelectedEventShortView.AccountId.HasValue && SelectedEventShortView.PatientId.HasValue &&
                SelectedEventShortView.ZslMedicalEventId.IsNotNull())
            {
                if (SelectedEventShortView.Version == Constants.Version30K || SelectedEventShortView.Version == Constants.Version30 ||
                    SelectedEventShortView.Version == Constants.Version31K || SelectedEventShortView.Version == Constants.Version31 ||
                    SelectedEventShortView.Version == Constants.Version32K || SelectedEventShortView.Version == Constants.Version32)
                {
                    _dockManager.ShowOperator(SelectedEventShortView.AccountId.Value, OperatorMode.ZInterTerritorial,
                        new List<OperatorAction>
                        {
                            new OperatorAction
                            {
                                Id = SelectedEventShortView.PatientId.Value,
                                ActionType = OperatorActionType.GoToPatient
                            },
                            new OperatorAction
                            {
                                Id = SelectedEventShortView.ZslMedicalEventId,
                                ActionType = OperatorActionType.GoToMedicalEvent
                            }
                        });
                }
                else
                {
                    _dockManager.ShowOperator(SelectedEventShortView.AccountId.Value, OperatorMode.InterTerritorial,
                        new List<OperatorAction>
                        {
                            new OperatorAction
                            {
                                Id = SelectedEventShortView.PatientId.Value,
                                ActionType = OperatorActionType.GoToPatient
                            },
                            new OperatorAction
                            {
                                Id = SelectedEventShortView.EventId,
                                ActionType = OperatorActionType.GoToMedicalEvent
                            }
                        });
                }
            }
        }

        private bool CanOpenAccountCheck()
        {
            return SelectedEventShortView.IsNotNull() && SelectedEventShortView.AccountId.HasValue &&
                   SelectedEventShortView.PatientId.HasValue && SelectedEventShortView.ZslMedicalEventId.IsNotNull();
        }
        private void OpenAccountCheck()
        {
            try
            {
                if (Constants.ZterritoryVersion.Contains(CurrentRow.Version ?? Constants.Version21))
                {
                    _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.ZInterTerritorial);
                }
                else
                {
                    _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.InterTerritorial);
                }

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии позиций территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanViewAccount()
        {
            return CurrentRow != null;
        }

        private bool CanViewAccountList()
        {
            return false;
        }

        private void ViewAccount()
        {
            try
            {
                if (Constants.ZterritoryVersion.Contains(CurrentRow.Version??Constants.Version21))
                {
                    _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.ZInterTerritorial);
                }
                else
                {
                    _dockManager.ShowOperator(CurrentRow.TerritoryAccountId, OperatorMode.InterTerritorial);
                }

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception,"Исключение при открытии позиций территориального счета ID {0}".F(CurrentRow.TerritoryAccountId),typeof(TerritoryAccountViewModel));
            }
        }

        private void ViewAccountList()
        {
            try
            {
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии позиций территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanEditAccount()
        {
            return CurrentRow != null;
        }

        private void EditAccount()
        {
            var copy = Map.ObjectToObject<FactTerritoryAccount>(CurrentRow);
            var model = new PropertyGridViewModel<EditTerritoryAccountViewModel>(new EditTerritoryAccountViewModel(copy));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);

            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactTerritoryAccount>(model.SelectedObject.Classifier);

                var result = _repository.Update(tmp);
                if (result.Success)
                {
                    var map = model.SelectedObject.MapObject<FactTerritoryAccount>(CurrentRow);
                    CurrentRow.Update(map.Affected);
                    view.Close();
                }
                
            };

            view.ShowDialog();
        }

        private bool CanViewPayments()
        {
            if (CurrentRow == null)
            {
                return false;
            }
            var result = _repository.IsEconomicAccountByAccountIdExists(CurrentRow.TerritoryAccountId);
            return result.Success && result.Data;
        }

        private void ViewPayments()
        {
            try
            {
                _dockManager.ShowEconomicAccount(CurrentRow.TerritoryAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии платежных поручений для территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanViewSurcharge()
        {
            if (CurrentRow == null)
            {
                return false;
            }
            var result = _repository.IsEconomicSurchargeByAccountIdExists(CurrentRow.TerritoryAccountId);
            return result.Success && result.Data;
        }

        private void ViewSurcharge()
        {
            try
            {
                _dockManager.ShowEconomicSurcharge(CurrentRow.TerritoryAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии доплат для территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }
        private IFileService _fileService;
        private IDockLayoutManager _dockLayoutManager;
        private void ExportExcel()
        {
            try
            {
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии сохранении {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }


        private bool CanViewRefuse()
        {
            if (CurrentRow == null)
            {
                return false;
            }
            var result = _repository.IsEconomicRefuseByAccountIdExists(CurrentRow.TerritoryAccountId);
            return result.Success && result.Data;
        }

        private void ViewRefuse()
        {
            try
            {
                _dockManager.ShowEconomicRefuse(CurrentRow.TerritoryAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии отказов для территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanViewExchange()
        {
            if (CurrentRow.IsNull())
            {
                return false;
            }
            var result = _repository.IsExchangeExistsForTerritoryAccount(CurrentRow.TerritoryAccountId);
            if (result.Success && result.Data)
            {
                return true;
            }
            return false;
        }

        private void ViewExchange()
        {
            try
            {
                _dockManager.ShowExchangeTerritory(CurrentRow.TerritoryAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии журнала информационного взаимодействия для территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
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
                    var model = scope.Resolve<RunProcessingViewModel>(
                        new NamedParameter("id", CurrentRow.TerritoryAccountId),
                        new NamedParameter("scope", Constants.ScopeInterTerritorialAccount),
                        new NamedParameter("version", CurrentRow.Version));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new RunProcessingView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке запуска функция обработки данных, счет ID {0}", typeof(MedicalAccountViewModel), CurrentRow.TerritoryAccountId);
            }
        }

        private void BreakUpAcountByPeriod()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<BreakUpByPeriodViewModel>();
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new BreakUpByPeriodView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке запуска расформирования счетов по периоду", typeof(MedicalAccountViewModel));
            }
        }

       
        private bool CanAddEqma()
        {
            return CurrentRow.IsNotNull() && _repository.IsEqmaWithoutActExistsForTerritoryAccount(CurrentRow.TerritoryAccountId);
        }

        private void AddEqma()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<EqmaActViewModel>(
                        new NamedParameter("accountId", CurrentRow.TerritoryAccountId),
                        new NamedParameter("scope", Constants.ScopeInterTerritorialAccount));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new EqmaView());
                    view.OkCallback = () =>
                    {
                        _notifyManager.ShowNotify("Акт ЭКМП успешно добавлен.");
                        view.Close();
                    };

                    view.CancelCallback = () =>
                    {
                        view.Close();
                    };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке добавления акта ЭКМП", typeof(TerritoryAccountViewModel));
            }
        }

        private bool CanAddMee()
        {
            return CurrentRow.IsNotNull() && _repository.IsMeeWithoutActExistsForTerritoryAccount(CurrentRow.TerritoryAccountId);
        }

        private void AddMee()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<MeeActViewModel>(
                        new NamedParameter("accountId", CurrentRow.TerritoryAccountId), 
                        new NamedParameter("scope", Constants.ScopeInterTerritorialAccount));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new MeeActView());
                    view.OkCallback = () =>
                    {
                        _notifyManager.ShowNotify("Акт МЭЭ успешно добавлен.");
                        view.Close();
                    };

                    view.CancelCallback = () =>
                    {
                        view.Close();
                    };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowExceptionFormatted(exception, "Ошибка при попытке добавления акта МЭЭ", typeof(MedicalAccountViewModel));
            }
        }

        private bool CanViewAct()
        {
            return CurrentRow.IsNotNull() && (_repository.IsMeeWithActExistsForTerritoryAccount(CurrentRow.TerritoryAccountId) || _repository.IsEqmaWithActExistsForTerritoryAccount(CurrentRow.TerritoryAccountId));
        }

        private void ViewAct()
        {
            try
            {
                _dockManager.ShowActs(Constants.ScopeInterTerritorialAccount, CurrentRow.TerritoryAccountId);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при открытии актов МЭЭ для территориального счета ID {0}".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

    }
}