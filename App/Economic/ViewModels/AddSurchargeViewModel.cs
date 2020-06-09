using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Autofac;
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
    public class AddSurchargeViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;
        private readonly INotifyManager _notifyManager;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        private FactEconomicSurcharge _current;

        private RelayCommand _createAccountCommand;
        private RelayCommand<object> _updateCommand;

        private ObservableCollection<SurchargeCustomModel> _surchargeList;

        private readonly int _accountId;
        private int? _economicSurchargeId;

        public Action OkCallback { get; set; }

        public AddSurchargeViewModel(IMedicineRepository repository,
            IMessageService messageService, 
            int accountId, 
            int? economicSurchargeId = null)
        {
            _repository = repository;
            _messageService = messageService;
            _accountId = accountId;
            _economicSurchargeId = economicSurchargeId;
            Init();
            _notifyManager = Di.I.Resolve<INotifyManager>();
        }

        public void Init()
        {
            try
            {
                if (_economicSurchargeId.HasValue)
                {
                    var economicSurchargeResult = _repository.GetEconomicSurchargeById(_economicSurchargeId.Value);
                    if (economicSurchargeResult.Success)
                    {
                        Current = economicSurchargeResult.Data;
                    }

                    var surchargesResult = _repository.GetSurchargeDetailById(Current.EconomicSurchargeId);
                    if (surchargesResult.Success)
                    {
                        SurchargeList = new ObservableCollection<SurchargeCustomModel>(surchargesResult.Data.Select(p => new SurchargeCustomModel
                        {
                            Amount = p.AmountSurcharge,
                            AssistanceConditionsId = p.AssistanceConditionsId,
                            EconomicSurchargeId = p.EconomicSurchargeId
                        }));
                    }
                }
                else
                {
                    Current = new FactEconomicSurcharge();
                    AccountId = _accountId;

                    SurchargeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    var refusesResult = _repository.GetExpectedPaymentsByAccountId(_accountId);
                    if (refusesResult.Success)
                    {
                        SurchargeList = new ObservableCollection<SurchargeCustomModel>(refusesResult.Data.Select(p => new SurchargeCustomModel
                        {
                            Amount = 0,
                            AssistanceConditionsId = p.AssistanceConditionsId,
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

        public FactEconomicSurcharge Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged("Current");
            }
        }


        public ObservableCollection<SurchargeCustomModel> SurchargeList
        {
            get { return _surchargeList; }
            set
            {
                _surchargeList = value;
                RaisePropertyChanged(()=>SurchargeList);
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

        public int EconomicSurchargeId
        {
            get { return Current.EconomicSurchargeId; }
            set
            {
                Current.EconomicSurchargeId = value;
                RaisePropertyChanged("EconomicSurchargeId");
            }
        }

        public DateTime SurchargeDate
        {
            get { return Current.SurchargeDate.Date; }
            set
            {
                Current.SurchargeDate = value;
                RaisePropertyChanged(()=>SurchargeDate);
            }
        }

        public decimal SurchargeTotalAmount
        {
            get { return Current.SurchargeTotalAmount; }
            set
            {
                Current.SurchargeTotalAmount = value;
                RaisePropertyChanged("SurchargeTotalAmount");
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
            var createOrUpdateResult = _repository.CreateOrUpdateEconomicSurcharge(Current);
            if (!createOrUpdateResult.Success)
            {
                return;
            }

            Current.EconomicSurchargeId = createOrUpdateResult.Id;

            var updateSurchargeResult = _repository.UpdateEconomicSurcharge(AccountId, EconomicSurchargeId, SurchargeList.Select(p => Tuple.Create(p.Amount, p.AssistanceConditionsId)));
            if (!updateSurchargeResult.Success)
            {
                return;
            }

            if (OkCallback != null)
            {
                OkCallback();
            }
            _notifyManager.ShowNotify("Доплата успешно добавлена");
        }

        private bool CanCreateAccount()
        {
            return true;
        }

        private void Update(object sender)
        {
            RaisePropertyChanged(()=>SurchargeList);
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
