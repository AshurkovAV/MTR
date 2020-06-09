using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Autofac;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Services;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Models;
using Medical.AppLayer.Operator.Views;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class MovePatientViewModel : ViewModelBase
    {
        private readonly IMedicineRepository _repository;
        private readonly ICacheRepository _cacheRepository;

        private int _patientId;
        private PatientShortView _patient;
        private TerritoryAccountView _parentAccount;
        private TerritoryAccountView _account;
        private Action _successCallback;
        private int _version;

        private ObservableCollection<TerritoryAccountCustom> _accountCollection;
        private ObservableCollection<CommonTuple> _insuranceCollection;
        private List<F002> _insuranceCollectionFull;
        private int? _selectedTerritory;
        private string _selectedTerritoryOkato;
        private int? _selectedInsurance;
        private bool _isSelectAccount;
        private int? _selectedAccount;

        private RelayCommand _moveCommand;
        private RelayCommand _searchAccountCommand;
        private RelayCommand _selectAccountCommand;
        private RelayCommand _createAccountCommand;

        public Action SuccessCallback
        {
            get { return _successCallback; }
            set
            {
                _successCallback = value;
                RaisePropertyChanged(() => SuccessCallback);
            }
        }

        public MovePatientViewModel(
            IMedicineRepository repository, 
            ICacheRepository cacheRepository,
            int patientId,
            int version)
        {
            _repository = repository;
            _cacheRepository = cacheRepository;
            PatientId = patientId;
            _version = version;

            Init();
        }

        private void Init()
        {
            var f010Result = _repository.GetF010();
            if (f010Result.Success)
            {
                TerritoryCollection = new ObservableCollection<F010>(f010Result.Data);
            }

            var f002Result = _repository.GetF002();
            if (f002Result.Success)
            {
                _insuranceCollectionFull = new List<F002>(f002Result.Data);
            }

            var patientResult = _repository.GetPatientShortViewByPatientId(PatientId);
            if (patientResult.Success)
            {
                Patient = patientResult.Data;
            }

            if(Patient.AccountId.HasValue)
            {
                var accountResult = _repository.GetTerritoryAccountViewById(Patient.AccountId.Value);
                if (accountResult.Success)
                {
                    ParentAccount = accountResult.Data;
                }
            }
        }

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; RaisePropertyChanged("PatientId"); }
        }

        public PatientShortView Patient
        {
            get { return _patient; }
            set
            {
                _patient = value;
                RaisePropertyChanged("Patient");
                RaisePropertyChanged("PatientFullName");
            }
        }

        public TerritoryAccountView ParentAccount
        {
            get { return _parentAccount; }
            set
            {
                _parentAccount = value;
                RaisePropertyChanged("ParentAccount");
                RaisePropertyChanged("ParentAccountShortInfo");
                RaisePropertyChanged("ParentAccountInfo");
            }
        }

        public TerritoryAccountView Account
        {
            get { return _account; }
            set
            {
                _account = value;
                RaisePropertyChanged("Account");
                RaisePropertyChanged("AccountShortInfo");
                RaisePropertyChanged("AccountInfo");
            }
        }


        public int? SelectedTerritory
        {
            get { return _selectedTerritory; }
            set { 
                _selectedTerritory = value; 
                RaisePropertyChanged("SelectedTerritory");
                SelectedInsurance = null;
                if (_selectedTerritory.HasValue)
                {
                    var okato = _cacheRepository.Get(CacheRepository.F010Cache).GetString(_selectedTerritory);
                    SelectedTerritoryOkato = okato;
                    InsuranceCollection = new ObservableCollection<CommonTuple>(_insuranceCollectionFull.Where(p => p.tf_okato == okato).Select(p=>new CommonTuple
                    {
                        ValueField = p.Id,
                        DisplayField = "{0} {1}".F(p.smocod, p.nam_smok),
                        DataField = p
                    }));
                }
            }
        }

        public string SelectedTerritoryOkato
        {
            get { return _selectedTerritoryOkato; }
            set
            {
                _selectedTerritoryOkato = value;
                RaisePropertyChanged("SelectedTerritoryOkato");
            }
        }
        
        public int? SelectedInsurance
        {
            get { return _selectedInsurance; }
            set { 
                _selectedInsurance = value;
                RaisePropertyChanged("SelectedInsurance");
                if (_selectedInsurance.HasValue)
                {
                    Insurance = _insuranceCollectionFull.FirstOrDefault(p => p.Id == _selectedInsurance);
                }
                else
                {
                    Insurance = null;
                }
            }
        }

        public F002 Insurance { get; set; }

        public int? SelectedAccount
        {
            get { return _selectedAccount; }
            set {
                _selectedAccount = value;
                RaisePropertyChanged("SelectedAccount");
                if (_selectedAccount.HasValue)
                {
                    var territoryAccountCustom = AccountCollection.FirstOrDefault(p => p.Id == _selectedAccount);
                    if (territoryAccountCustom != null)
                    {
                        Account = territoryAccountCustom.View;
                    }
                }
            }
        }
        
        public string ParentAccountShortInfo
        {
            get { return "№{0}, отчётная дата {1:MMMM yyyy}, Территория {2}".F(ParentAccount.AccountNumber, ParentAccount.Date, ParentAccount.DestinationName); }

        }
        public string ParentAccountInfo
        {
            //TODO add tooltip text
            get { return "Подсказка TODO"; }
            
        }

        public string AccountShortInfo
        {
            get
            {
                return Account != null ? 
                    "№{0}, отчётная дата {1:MMMM yyyy}, Территория {2}".F(Account.AccountNumber, Account.Date, Account.DestinationName):
                    "Счет не найден";
            }

        }
        public string AccountInfo
        {
            //TODO add tooltip text
            get { return "Подсказка TODO"; }

        }

        public string PatientFullName
        {
            get { return "{0} {1} {2}".F(Patient.Surname, Patient.Name, Patient.Patronymic); }

        }

        public string IsSelectAccount
        {
            get { return _isSelectAccount ? "Visible" : "Collapsed"; }
        }

        public ObservableCollection<F010> TerritoryCollection { get; set; }
        public ObservableCollection<CommonTuple> InsuranceCollection
        {
            get { return _insuranceCollection; }
            set { _insuranceCollection = value; RaisePropertyChanged(()=>InsuranceCollection); }
        }
        public ObservableCollection<TerritoryAccountCustom> AccountCollection {
            get { return _accountCollection; }
            set { _accountCollection = value;RaisePropertyChanged(()=>AccountCollection); }
        }

        public ICommand MoveCommand
        {
            get { return _moveCommand ?? (_moveCommand = new RelayCommand(Move, CanMove)); }
        }

        private void Move()
        {
            var moveResult = _repository.MovePatientToAnotherTerritoryAccount(
                PatientId,
                Account.TerritoryAccountId,
                ParentAccount.TerritoryAccountId,
                SelectedTerritory,
                SelectedInsurance,
                Insurance != null ? Insurance.Ogrn : null
                );
            if (moveResult.Success)
            {
                if (SuccessCallback.IsNotNull())
                {
                    SuccessCallback();
                }
            }
        }

        private bool CanMove()
        {
            return Account != null;
        }

        public ICommand SearchAccountCommand
        {
            get { return _searchAccountCommand ?? (_searchAccountCommand = new RelayCommand(SearchAccount, CanSearchAccount)); }
        }

        private void SearchAccount()
        {
            _isSelectAccount = false;
            SelectedAccount = null;
            RaisePropertyChanged("IsSelectAccount");

            var accountResult = _repository.GetTerritoryAccountView(p =>
                p.Status < (int)AccountStatus.Sended &&
                p.Direction == (int)DirectionType.Out &&
                p.Type == (int)AccountType.GeneralPart &&
                p.Destination == SelectedTerritoryOkato &&
                p.Date >= ParentAccount.Date);

            if (accountResult.Success && accountResult.Data.Any())
            {
                Account = accountResult.Data
                    .OrderBy(p => p.Date)
                    .Last();
            }
            else
            {
                Account = null;
            }
        }

        private bool CanSearchAccount()
        {
            return SelectedTerritory.HasValue;
        }

        public ICommand SelectAccountCommand
        {
            get { return _selectAccountCommand ?? (_selectAccountCommand = new RelayCommand(SelectAccount, CanSelectAccount)); }
        }

        private void SelectAccount()
        {
            _isSelectAccount = true;
            RaisePropertyChanged("IsSelectAccount");

            var accountResult = _repository.GetTerritoryAccountView(p =>
                p.Status < (int)AccountStatus.Sended &&
                p.Direction == (int)DirectionType.Out &&
                p.Type == (int)AccountType.GeneralPart &&
                p.Destination == SelectedTerritoryOkato &&
                p.Date >= ParentAccount.Date);

            if (accountResult.Success && accountResult.Data.Any())
            {
                AccountCollection = new ObservableCollection<TerritoryAccountCustom>(accountResult.Data.OrderBy(p=>p.Date).Select(p => new TerritoryAccountCustom
                {
                    Id = p.TerritoryAccountId,
                    Description = "№{0}/{1:MMMM yyyy}/{2}".F(p.AccountNumber, p.Date, p.DestinationName),
                    View = p
                }));
            }
            else
            {
                AccountCollection = null;
            }
        }

        private bool CanSelectAccount()
        {
            return SelectedTerritory.HasValue;
        }

        public ICommand CreateAccountCommand
        {
            get { return _createAccountCommand ?? (_createAccountCommand = new RelayCommand(CreateAccount, CanCreateAccount)); }
        }

        private void CreateAccount()
        {
            var view = new CreateAccountView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<CreateAccountViewModel>(new NamedParameter("territoryOkato", SelectedTerritoryOkato),
                    new NamedParameter("version", _version));
                view.DataContext = model;
                view.ShowDialog();
                if (model.AccountId > 0)
                {
                    SearchAccount();
                }
            }
        }

        private bool CanCreateAccount()
        {
            return SelectedTerritory.HasValue;
        }
    }
}
