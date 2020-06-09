using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Autofac;
using Core;
using Core.Services;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Economic.Models;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class AddPaymentViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;
        private readonly INotifyManager _notifyManager;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        private FactEconomicAccount _current;

        private RelayCommand _createAccountCommand;
        private RelayCommand<object> _updateCommand;
        private RelayCommand _evalCommand;

        private ObservableCollection<PaymentCustomModel> _paymentList;

        public Action OkCallback { get; set; }

        private readonly int _accountId;
        private int? _economicAccountId;

        public AddPaymentViewModel(IMedicineRepository repository, IMessageService messageService, int accountId, int? economicAccountId = null)
        {
            _repository = repository;
            _messageService = messageService;
            _accountId = accountId;
            _economicAccountId = economicAccountId;
            Init();
            _notifyManager = Di.I.Resolve<INotifyManager>();
        }

        public void Init()
        {
            try
            {
                if (_economicAccountId.HasValue)
                {
                    var economicAccountResult = _repository.GetEconomicAccountById(_economicAccountId.Value);
                    if (economicAccountResult.Success)
                    {
                        Current = economicAccountResult.Data;
                    }

                    var paymentsResult = _repository.GetPaymentsByEconomicAccountId(_economicAccountId.Value);
                    if (paymentsResult.Success)
                    {
                        PaymentList = new ObservableCollection<PaymentCustomModel>(paymentsResult.Data.Select(p => new PaymentCustomModel { Amount = p.Amount, AssistanceConditionsId = p.AssistanceConditionsId }));
                    }
                }
                else
                {
                    var accountResult = _repository.GetTerritoryAccountById(_accountId);
                    if (accountResult.HasError)
                    {
                        return;
                    }

                    Current = new FactEconomicAccount();
                    AccountId = _accountId;

                    PaymentDate = DateTime.Now.Date;
                    PaymentOrderDate = DateTime.Now.Date;
                    PaymentStatus = 1;
                    TotalAmount = 0;

                    var paymentsResult = (accountResult.Data.Version != Constants.Version30 && accountResult.Data.Version != Constants.Version31 && accountResult.Data.Version != Constants.Version32)
                        ? _repository.GetExpectedPaymentsByAccountId(_accountId)
                        : _repository.GetZExpectedPaymentsByAccountId(_accountId);
                    if (paymentsResult.Success)
                    {
                        PaymentList = new ObservableCollection<PaymentCustomModel>(paymentsResult.Data.Select(p => new PaymentCustomModel { Amount = p.Amount, AssistanceConditionsId = p.AssistanceConditionsId }));
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception,"При попытке добавления/редактирования платежа произошло исключение", typeof(AddPaymentViewModel));
            }

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public FactEconomicAccount Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged("Current");
            }
        }

        public ObservableCollection<PaymentCustomModel> PaymentList {
            get { return _paymentList; }
            set
            {
                _paymentList = value;
                RaisePropertyChanged("PaymentList");
                TotalAmount = PaymentList.Sum(p => p.Amount);
            }
        }

        public int EconomicAccountId
        {
            get { return Current.EconomicAccountId; }
            set
            {
                Current.EconomicAccountId = value;
                RaisePropertyChanged("EconomicAccountId");
            }
        }

        public int AccountId
        {
            get { return Current.AccountId; }
            set
            {
                Current.AccountId = value;
                RaisePropertyChanged("MedicalAccountId");
            }
        }

        public string PaymentOrder
        {
            get { return Current.PaymentOrder; }
            set
            {
                Current.PaymentOrder = value;
                RaisePropertyChanged("PaymentOrder");
            }
        }

        public DateTime? PaymentOrderDate
        {
            get { return Current.PaymentOrderDate.HasValue ? Current.PaymentOrderDate.Value.Date : default(DateTime); }
            set
            {
                Current.PaymentOrderDate = value;
                RaisePropertyChanged("PaymentOrderDate");
            }
        }

        public DateTime? PaymentDate
        {
            get { return Current.PaymentDate.HasValue ? Current.PaymentDate.Value.Date : default(DateTime); }
            set
            {
                Current.PaymentDate = value;
                RaisePropertyChanged("PaymentDate");
            }
        }

        public Decimal? TotalAmount
        {
            get { return Current.TotalAmount; }
            set
            {
                Current.TotalAmount = value;
                RaisePropertyChanged("TotalAmount");
            }
        }

        public int? PaymentStatus
        {
            get { return Current.PaymentStatus; }
            set
            {
                Current.PaymentStatus = value;
                RaisePropertyChanged(()=>PaymentStatus);
            }
        }

        public string Comments
        {
            get { return Current.Comments; }
            set
            {
                Current.Comments = value;
                RaisePropertyChanged("Comments");
            }
        }

        public ICommand CreateAccountCommand
        {
            get { return _createAccountCommand ?? (_createAccountCommand = new RelayCommand(CreateAccount, CanCreateAccount)); }
        }

        public ICommand UpdateCommand
        {
            get { return _updateCommand ?? (_updateCommand = new RelayCommand<object>(Update)); }
        }

        private void CreateAccount()
        {
            var createOrUpdateResult = _repository.CreateOrUpdateEconomicAccount(Current);
            if (!createOrUpdateResult.Success)
            {
                return;
            }

            Current.EconomicAccountId = createOrUpdateResult.Id;

            var updatePaymentResult = _repository.UpdateEconomicPayment(AccountId, EconomicAccountId, PaymentList.Select(p => Tuple.Create(p.Amount, p.AssistanceConditionsId)));
            if (!updatePaymentResult.Success)
            {
                return;
            }

            if (OkCallback != null)
            {
                OkCallback();
            }
            _notifyManager.ShowNotify("Платежное поручение успешно добавлено");
        }

        private bool CanCreateAccount()
        {
            return true;
        }

        private void Update(object sender)
        {
            RaisePropertyChanged("PaymentList");
        }

        public ICommand EvalCommand
        {
            get { return _evalCommand ?? (_evalCommand = new RelayCommand(Eval)); }
        }

        private void Eval()
        {
            try
            {
                var accountResult = _repository.GetTerritoryAccountById(_accountId);
                if (accountResult.HasError)
                {
                    return;
                }

                var paymentsResult = (accountResult.Data.Version != Constants.Version30 && accountResult.Data.Version != Constants.Version31)
                    ? _repository.GetExpectedPaymentsByAccountId(_accountId)
                    : _repository.GetZExpectedPaymentsByAccountId(_accountId);
                if (paymentsResult.Success)
                {
                    PaymentList = new ObservableCollection<PaymentCustomModel>(paymentsResult.Data.Select(p => new PaymentCustomModel { Amount = p.Amount, AssistanceConditionsId = p.AssistanceConditionsId }));
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке посчитать сумму платежа произошло исключение", typeof(AddPaymentViewModel));
            }
        }

        string IDataErrorInfo.Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }

        string IDataErrorInfo.this[string memberName]
        {
            get { return _dataErrorInfoSupport[memberName]; }
        }

        [Browsable(false)]
        public string Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }
    }
}
