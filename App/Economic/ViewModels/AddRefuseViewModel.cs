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
    public class AddRefuseViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;
        private readonly INotifyManager _notifyManager;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        private FactEconomicRefuse _current;

        private RelayCommand _createAccountCommand;
        private RelayCommand<object> _updateCommand;
        private RelayCommand _evalCommand;
        
        private ObservableCollection<RefuseCustomModel> _refuseList;

        public Action OkCallback { get; set; }

        private readonly int _accountId;
        private readonly int _version;
        private int? _economicRefuseId;

        public AddRefuseViewModel(IMedicineRepository repository, 
            IMessageService messageService, 
            int accountId, 
            int version,
            int? economicRefuseId = null)
        {
            _repository = repository;
            _messageService = messageService;
            _accountId = accountId;
            _version = version;
            _economicRefuseId = economicRefuseId;
            Init();
            _notifyManager = Di.I.Resolve<INotifyManager>();
        }

        public void Init()
        {
            try
            {
                if (_economicRefuseId.HasValue)
                {
                    var economicRefuseResult = _repository.GetEconomicRefuseById(_economicRefuseId.Value);
                    if (economicRefuseResult.Success)
                    {
                        Current = economicRefuseResult.Data;
                    }

                    var refusesResult = _repository.GetRefusesDetailById(Current.EconomicRefuseId);
                    if (refusesResult.Success)
                    {
                        RefuseList = new ObservableCollection<RefuseCustomModel>(refusesResult.Data.Select(p => new RefuseCustomModel
                        {
                            Amount = p.AmountRefuse, 
                            AssistanceConditionsId = p.AssistanceConditionsId,
                            EconomicRefuseId = p.EconomicRefuseId
                        }));
                    }
                }
                else
                {
                    Current = new FactEconomicRefuse();
                    AccountId = _accountId;

                    RefuseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    var refusesResult = _repository.GetExpectedRefuseByAccountId(_accountId);
                    if (refusesResult.Success)
                    {
                        RefuseList = new ObservableCollection<RefuseCustomModel>(refusesResult.Data.Select(p => new RefuseCustomModel
                        {
                            Amount = p.AmountRefuse, 
                            AssistanceConditionsId = p.AssistanceConditionsId,
                            EconomicRefuseId = p.EconomicRefuseId
                        }));
                    }

                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке добавления/редактирования платежа произошло исключение", typeof(AddPaymentViewModel));
            }

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public FactEconomicRefuse Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged("Current");
            }
        }


        public ObservableCollection<RefuseCustomModel> RefuseList {
            get { return _refuseList; }
            set
            {
                _refuseList = value;
                RaisePropertyChanged("RefuseList");
                RefuseTotalAmount = RefuseList.Sum(p => p.Amount);
            }
        }

        public int EconomicRefuseId
        {
            get { return Current.EconomicRefuseId; }
            set
            {
                Current.EconomicRefuseId = value;
                RaisePropertyChanged("EconomicRefuseId");
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

        public DateTime RefuseDate
        {
            get { return Current.RefuseDate.Date; }
            set
            {
                Current.RefuseDate = value;
                RaisePropertyChanged("RefuseDate");
            }
        }

        public decimal RefuseTotalAmount
        {
            get { return Current.RefuseTotalAmount; }
            set
            {
                Current.RefuseTotalAmount = value;
                RaisePropertyChanged("RefuseTotalAmount");
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

        public ICommand EvalCommand
        {
            get { return _evalCommand ?? (_evalCommand = new RelayCommand(Eval)); }
        }

        private void CreateAccount()
        {
            var createOrUpdateResult = _repository.CreateOrUpdateEconomicRefuse(Current);
            if (!createOrUpdateResult.Success)
            {
                return;
            }

            Current.EconomicRefuseId = createOrUpdateResult.Id;

            var updateRefuseResult = _repository.UpdateEconomicRefuse(AccountId, EconomicRefuseId, RefuseList.Select(p => Tuple.Create(p.Amount, p.AssistanceConditionsId)));
            if (!updateRefuseResult.Success)
            {
                return;
            }

            if (OkCallback != null)
            {
                OkCallback();
            }
            _notifyManager.ShowNotify("Отказ успешно добавлен");
        }

        private bool CanCreateAccount()
        {
            return true;
        }

        private void Eval()
        {
            var refusesResult = _repository.GetExpectedRefuseByAccountId(_accountId);
            if (refusesResult.Success)
            {
                RefuseList = new ObservableCollection<RefuseCustomModel>(refusesResult.Data.Select(p => new RefuseCustomModel
                {
                    Amount = p.AmountRefuse,
                    AssistanceConditionsId = p.AssistanceConditionsId,
                    EconomicRefuseId = p.EconomicRefuseId
                }));
            }
            RaisePropertyChanged("RefuseList");
        }

        private void Update(object sender)
        {
            RaisePropertyChanged("PaymentList");
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
