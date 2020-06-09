using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Models;
using Medical.AppLayer.Models.PatientSearch;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    public class MeeActViewModel : ViewModelBase, IDataErrorInfo, ICallbackable
    {
        private readonly IMedicineRepository _repository;
        private readonly ICacheRepository _cache;
        private readonly IReportService _reportService;
        private readonly IDataService _dataService;
        private readonly IAppRemoteSettings _settings;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private readonly int _id;
        private readonly int _scope;
        private readonly int _userId;

        private int? _actId;
        private FactPerson _person;
        private FactPatient _patient;
        private List<EventCustomModel> _selectedMedicalEvents;

        private Dictionary<int, Action> _modeHandlers;

        private ObservableCollection<SearchResultModel> _patientList;
        private ObservableCollection<SearchResultModel> _medicalEventList;
        private object _selectedPatientId;
        private List<object> _selectedMedicalEventIdsList;
        private List<FactMEE> _meeList;

        private FactActMee _current;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;
        
        public MeeActViewModel(
            IMedicineRepository repository,
            IUserService userService,
            ICacheRepository cache,
            IReportService reportService,
            IDataService dataService,
            IAppRemoteSettings settings,
            int accountId, 
            int scope,
            int? actId = null)
        {
            _id = accountId;
            _actId = actId;
            _scope = scope;
            _userId = userService.UserId;
            _repository = repository;
            _cache = cache;
            _reportService = reportService;
            _dataService = dataService;
            _settings = settings;

            Init();
            InitMode();
            InitAct();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void InitAct()
        {
            if (_actId.HasValue)
            {
                var actResult = _repository.GetMeeActById(_actId);
                if (actResult.Success)
                {
                    Current = actResult.Data;

                    var patientsResult = _repository.GetPatientShortViewByPatientId(Current.PatientId);
                    if (patientsResult.Success)
                    {
                        var patient = patientsResult.Data;
                        if (PatientList.IsNull())
                        {
                            PatientList = new ObservableCollection<SearchResultModel>();
                        }

                        var result = new SearchResultModel
                        {
                            PatientData = "{0} {1} {2}/ДР: {3}/{4}/Полис: {5}".F(
                                patient.Surname,
                                patient.Name,
                                patient.Patronymic,
                                patient.Birthday.HasValue ? patient.Birthday.Value.ToString("dd.MM.yyyy") : "нет",
                                patient.Sex,
                                patient.InsuranceDocType == 3
                                    ? patient.INP
                                    : "{0} {1}".F(patient.InsuranceDocNumber, patient.InsuranceDocSeries)),
                            PatientId = patientsResult.Data.PatientId
                        };
                    
                        PatientList.Add(result);
                        _selectedPatientId = result.PatientId;
                        RaisePropertyChanged(()=>SelectedPatientId);
                    }


                    LoadPerson();
                    LoadPatient();
                    LoadWithActMedicalEvent();

                    var positions = Current.Positions.ToInt32List();
                    SelectedMedicalEventIdsList = new List<object>(MedicalEventList.Where(p => positions.Contains(p.ExternalId ?? 0)).Select(p => p.EventId as object));
                }
            }
            else
            {
                var lastNumberResult = _repository.GetMeeActLastNumber(_scope);
                Current = new FactActMee
                {
                    AccountId = _scope == Constants.ScopeLocalAccount ? default(int?) : _id,
                    AccountMoId = _scope == Constants.ScopeLocalAccount ? _id : default(int?),
                    Number = lastNumberResult,
                    Date = DateTime.Today,
                    EmployeeId = _userId
                };
            }
        }

        private void InitMode()
        {
            if (_modeHandlers.ContainsKey(_scope))
            {
                _modeHandlers[_scope]();
            }
        }


        private void Init()
        {
            _modeHandlers = new Dictionary<int, Action>
            {
                {Constants.ScopeInterTerritorialAccount, InitInterTerritorial},
                {Constants.ScopeLocalAccount, InitLocal}
            };
        }

        private void InitLocal()
        {
            var accountResult = _repository.GetMedicalAccountById(_id);
            if (accountResult.Success)
            {
                Account = new CommonAccountModel
                {
                    AccountNumber = accountResult.Data.AccountNumber,
                    AccountDate = accountResult.Data.AccountDate
                };
                MedicalAccount = accountResult.Data;
            }

            var patientsResult = _repository.GetPatientsByMedicalAccountIdWithMeeWithoutAct(_id);
            if (patientsResult.Success)
            {
                PatientList = new ObservableCollection<SearchResultModel>(patientsResult.Data.Select(p => new SearchResultModel
                {
                    PatientData = "{0} {1} {2}/ДР: {3}/{4}/Полис: {5}".F(
                        p.Surname,
                        p.Name,
                        p.Patronymic,
                        p.Birthday.HasValue ? p.Birthday.Value.ToString("dd.MM.yyyy") : "нет",
                        p.Sex,
                        p.InsuranceDocType == 3 ? p.INP : "{0} {1}".F(p.InsuranceDocNumber, p.InsuranceDocSeries)),
                    PatientId = p.PatientId
                }));
            }

            var userResult = _repository.GetUserById(_userId);
            if (userResult.Success)
            {
                User = userResult.Data;
            }

        }

        private void InitInterTerritorial()
        {
            var accountResult = _repository.GetTerritoryAccountById(_id);
            if (accountResult.Success)
            {
                Account = new CommonAccountModel{
                    AccountNumber = accountResult.Data.AccountNumber,
                    AccountDate= accountResult.Data.AccountDate,
                    Source = accountResult.Data.Source
                };
                TerritoryAccount = accountResult.Data;
            }

            var patientsResult = _repository.GetPatientsByAccountIdWithMeeWithoutAct(_id);
            if (patientsResult.Success)
            {
                PatientList = new ObservableCollection<SearchResultModel>(patientsResult.Data.Select(p => new SearchResultModel
                {
                    PatientData = "{0} {1} {2}/ДР: {3}/{4}/Полис: {5}".F(
                        p.Surname,
                        p.Name,
                        p.Patronymic,
                        p.Birthday.HasValue ? p.Birthday.Value.ToString("dd.MM.yyyy") : "нет",
                        p.Sex,
                        p.InsuranceDocType == 3 ? p.INP : "{0} {1}".F(p.InsuranceDocNumber, p.InsuranceDocSeries)),
                    PatientId = p.PatientId
                }));
            }

            var userResult = _repository.GetUserById(_userId);
            if (userResult.Success)
            {
                User = userResult.Data;
            }
        }

        private void LoadPerson()
        {
            var personResult = _repository.GetPersonByPatientId(Current.PatientId);
            if (personResult.Success)
            {
                Person = personResult.Data;
            }
        }

        private void LoadPatient()
        {
            var patientResult = _repository.GetPatient(Current.PatientId);
            if (patientResult.Success)
            {
                Patient = patientResult.Data;
            }
        }

        private void LoadWithoutActMedicalEvent()
        {
            var meventResult = _repository.GetMedicalEventByPatientIdWithMeeWithoutAct(Current.PatientId, _scope);
            if (meventResult.Success)
            {
                MedicalEventList = new ObservableCollection<SearchResultModel>();
                MedicalEventList.AddRange(meventResult.Data.Select(p => new SearchResultModel
                {
                    MeventData = p.MeventData,
                    EventId = p.EventId,
                    ExternalId = _scope == Constants.ScopeLocalAccount ? p.MedicalExternalId : p.ExternalId
                }));
            }
        }

        private void LoadWithActMedicalEvent()
        {
            var meventResult = _repository.GetMedicalEventByPatientIdWithMeeWithAct(Current.PatientId, _scope);
            if (meventResult.Success)
            {
                if (MedicalEventList.IsNull())
                {
                    MedicalEventList = new ObservableCollection<SearchResultModel>();
                }
                MedicalEventList.AddRange(meventResult.Data.Select(p => new SearchResultModel
                {
                    MeventData = p.MeventData,
                    EventId = p.EventId,
                    ExternalId = _scope == Constants.ScopeLocalAccount ? p.MedicalExternalId : p.ExternalId
                }));
            }
        }

        public ObservableCollection<SearchResultModel> PatientList {
            get { return _patientList; }
            set { 
                _patientList = value; 
                RaisePropertyChanged(()=>PatientList);
            }
        }
        public object SelectedPatientId {
            get { return _selectedPatientId; }
            set
            {
                _selectedPatientId = value; 
                RaisePropertyChanged(()=>SelectedPatientId);

                if (_selectedPatientId.IsNotNull())
                {
                    Current.PatientId = _selectedPatientId.ToInt32();

                    LoadPerson();
                    LoadPatient();

                    LoadWithoutActMedicalEvent();
                }
            }
        }

        public ObservableCollection<SearchResultModel> MedicalEventList {
            get { return _medicalEventList; }
            set { _medicalEventList = value; RaisePropertyChanged(()=>MedicalEventList); }
        }

        public List<object> SelectedMedicalEventIdsList {
            get { return _selectedMedicalEventIdsList; }
            set
            {
                _selectedMedicalEventIdsList = value;
                RaisePropertyChanged(() => SelectedMedicalEventIdsList);
                if (_selectedMedicalEventIdsList.IsNotNull())
                {
                    var medicalEventsResult =_repository.GetMedicalEventsByIds(_selectedMedicalEventIdsList);
                    if (medicalEventsResult.Success)
                    {

                        SelectedMedicalEvents = new List<EventCustomModel>(medicalEventsResult.Data.Select(p=>new EventCustomModel
                        {
                            History = p.History,
                            DoctorId = p.DoctorId,
                            EventBegin = p.EventBegin,
                            EventEnd = p.EventEnd,
                            EventId = p.MedicalEventId,
                            ExternalId = _scope == Constants.ScopeLocalAccount ? p.MedicalExternalId : p.ExternalId,
                            DiagnosisGeneral = _cache.Get(CacheRepository.IDC10Cache).GetString(p.DiagnosisGeneral),
                            DiagnosisSecondary = p.DiagnosisSecondaryAggregate,
                            AcceptPrice = p.AcceptPrice,
                            Price = _scope == Constants.ScopeLocalAccount ? p.MoPrice : p.Price,
                            Quantity = p.Quantity,
                            MedicalOrganizationCode = p.MedicalOrganizationCode
                        }));

                        Current.AcceptPrice = SelectedMedicalEvents.Sum(p => p.AcceptPrice);
                        Current.Price = SelectedMedicalEvents.Sum(p => p.Price);
                        Current.PenaltyPrice = 0;
                        Current.RefusalPrice = 0;

                        _meeList = new List<FactMEE>();
                        for (int i = 0; i < SelectedMedicalEvents.Count(); i++)
                        {
                            var meeResult = _repository.GetMeeByMedicalEventIdAndSource(SelectedMedicalEvents[i].EventId, _dataService.GetRefusalSourceByScope(_id, _scope));
                            if (meeResult.Success)
                            {
                                var mee = meeResult.Data.FirstOrDefault();
                                SelectedMedicalEvents[i].ReasonId = mee.ReasonId ?? 0;
                                SelectedMedicalEvents[i].PenaltyPrice = mee.Penalty;
                                SelectedMedicalEvents[i].RefusalPrice = mee.Amount;

                                Current.PenaltyPrice += mee.Penalty;
                                Current.RefusalPrice += mee.Amount;

                                _meeList.Add(mee);
                            }
                        }

                        RaisePropertyChanged(() => RefusalCodeComposition);
                        RaisePropertyChanged(()=>Current);
                    }
                }
            }
        }


        public bool IsPreview { get; set; }
        public localUser User { get; set; }
        public CommonAccountModel Account { get; set; }
        public FactTerritoryAccount TerritoryAccount { get; set; }
        public FactMedicalAccount MedicalAccount { get; set; }
        public FactPatient Patient {
            get { return _patient; }
            set
            {
                _patient = value;
                RaisePropertyChanged(()=>Patient);
                RaisePropertyChanged(()=>PolicyNumber);
            }
        }
        public FactPerson Person {
            get { return _person; }
            set
            {
                _person = value; 
                RaisePropertyChanged(()=>Person);
                RaisePropertyChanged(()=>FullName);
            }
        }

        public List<EventCustomModel> SelectedMedicalEvents
        {
            get { return _selectedMedicalEvents; }
            set
            {
                _selectedMedicalEvents = value;
                RaisePropertyChanged(() => SelectedMedicalEvents);
                RaisePropertyChanged(() => HistoryComposition);
                RaisePropertyChanged(() => EventExternalIdComposition);
                RaisePropertyChanged(() => EventMin);
                RaisePropertyChanged(() => EventMax);
                RaisePropertyChanged(() => RefusalCodeComposition);
                RaisePropertyChanged(() => MedicalOrganizationComposition);
                RaisePropertyChanged(() => DiagnosisGeneralComposition); 
                RaisePropertyChanged(() => DiagnosisSecondaryComposition);
            }
        }
        

        public FactActMee Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged("Current");
            }
        }


        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave)); }
        }

        private bool CanSave()
        {
            return Error.IsNullOrWhiteSpace();
        }

        private void Save()
        {
            var result = _repository.InsertOrUpdateMeeAct(Current, _meeList.Select(p => p.MEEId));
            if (result.Success)
            {
                _repository.DeletePreparedReportByExternalIdAndScope(result.Data, Constants.ScopeActMee);
                dynamic acts = _settings.Get(AppRemoteSettings.ActIds);


                var reportResult = _reportService.PrepareReports(new List<object> { _scope == Constants.ScopeLocalAccount ? acts["ActMedicalMee"] : acts["ActMee"] }, _userId, result.Data, Constants.ScopeActMee, _id, IsPreview);
                if (reportResult.Success)
                {
                    if (OkCallback.IsNotNull())
                    {
                        OkCallback();
                    }
                }
            }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
        }

        private void Cancel()
        {
            if (CancelCallback.IsNotNull())
            {
                CancelCallback();
            }
        }

        #region ReadOnly properties

        public string FullName
        {
            get
            {
                if (Person.IsNotNull())
                {
                    Current.Personal = string.Format("{0} {1} {2}", Person.Surname, Person.PName, Person.Patronymic);
                    return Current.Personal;
                }
                return null;
            }
        }

        public string PolicyNumber
        {
            get
            {
                if (Patient.IsNotNull())
                {
                    Current.PolicyNumber = string.Empty;

                    if (Patient.INP.IsNotNullOrWhiteSpace())
                    {
                        Current.PolicyNumber = Patient.INP;
                    }
                    else
                    {
                        if (Patient.InsuranceDocSeries.IsNotNullOrWhiteSpace())
                        {
                            Current.PolicyNumber += "{0} ".F(Patient.InsuranceDocSeries);
                        }
                        if (Patient.InsuranceDocNumber.IsNotNullOrWhiteSpace())
                        {
                            Current.PolicyNumber += "{0}".F(Patient.InsuranceDocNumber);
                        }
                    }
                }
                else
                {
                    Current.PolicyNumber = null;
                }
                return Current.PolicyNumber;
            }
        }


        public string UserFullNamePos
        {
            get
            {
                if (User.IsNotNull())
                {
                    return string.Format("{0} {1} {2}, {3}", User.LastName, User.FirstName, User.Patronymic, User.Position);
                }
                return null;
            }
        }

        public object EventExternalIdComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.Positions = SelectedMedicalEvents.Select(p => p.ExternalId).Distinct(p=>p).AggregateToString();
                }
                else
                {
                    Current.Positions = null;
                }
                return Current.Positions;
            }
        }

        public string HistoryComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.History = SelectedMedicalEvents.Select(p => p.History).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.History = null;
                }
                return Current.History;
            }
        }

        public string MedicalOrganizationComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.MedicalOrganization = SelectedMedicalEvents.Select(p => p.MedicalOrganizationCode).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.MedicalOrganization = null;
                }
                return Current.MedicalOrganization;
            }
        }

        
        public string DiagnosisGeneralComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.DiagnosisGeneral = SelectedMedicalEvents.Select(p => p.DiagnosisGeneral).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.DiagnosisGeneral = null;
                }
                return Current.DiagnosisGeneral;
            }
        }

        public string DiagnosisSecondaryComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.DiagnosisSecondary = SelectedMedicalEvents.Select(p => p.DiagnosisSecondary).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.DiagnosisSecondary = null;
                }
                return Current.DiagnosisSecondary;
            }
        }

        public DateTime? EventMin
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.EventBegin = SelectedMedicalEvents.Min(p => p.EventBegin);
                }
                else
                {
                    Current.EventBegin = null;
                }
                return Current.EventBegin;
            }
        }

        public DateTime? EventMax
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.EventEnd = SelectedMedicalEvents.Max(p => p.EventEnd);
                }
                else
                {
                    Current.EventEnd = null;
                }
                return Current.EventEnd;
            }
        }

        public string RefusalCodeComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.RefusalCode = SelectedMedicalEvents.Select(p => p.ReasonCode).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.RefusalCode = null;
                }
                return Current.RefusalCode;
            }
        }
#endregion
        

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

        [Browsable(false)]
        public string Error
        {
            get
            {
                return _dataErrorInfoSupport.Error;
            }
        }

        public Action OkCallback { get; set; }
        public Action CancelCallback { get; set; }
        public Action CreateCallback { get; set; }
    }

   
}