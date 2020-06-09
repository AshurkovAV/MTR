using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Core;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Models;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Cache;
using Core.Infrastructure;
using System.ComponentModel;
using Core.Validation;
using System.Collections.ObjectModel;
using Medical.AppLayer.Models.PatientSearch;
using Core.Extensions;

namespace Medical.AppLayer.Register.ViewModels
{
    public class EqmaActViewModel : ViewModelBase, IDataErrorInfo, ICallbackable
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
        private List<FactEQMA> _eqmaList;

        private FactActEqma _current;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;
        
        public EqmaActViewModel(
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
                    AccountDate = accountResult.Data.AccountDate,
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetString(accountResult.Data.MedicalOrganization),
                    MedicalOrganizationId = accountResult.Data.MedicalOrganization
                };

                MedicalAccount = accountResult.Data;
            }

            var patientsResult = _repository.GetPatientsByMedicalAccountIdWithEqmaWithoutAct(_id);
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
                Account = new CommonAccountModel
                {
                    AccountNumber = accountResult.Data.AccountNumber,
                    AccountDate = accountResult.Data.AccountDate,
                    Source = accountResult.Data.Source
                };
                TerritoryAccount = accountResult.Data;
            }

            var patientsResult = _repository.GetPatientsByAccountIdWithEqmaWithoutAct(_id);
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


        private void InitAct()
        {
            if (_actId.HasValue)
            {
                var actResult = _repository.GetEqmaActById(_actId);
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
                        RaisePropertyChanged(() => SelectedPatientId);
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
                
                var lastNumberResult = _repository.GetEqmaActLastNumber(_scope);
                Current = new FactActEqma
                {
                    AccountId = _scope == Constants.ScopeLocalAccount ? default(int?) : _id,
                    AccountMoId = _scope == Constants.ScopeLocalAccount ? _id : default(int?),
                    Number = lastNumberResult,
                    Date = DateTime.Today,
                    EmployeeId = _userId
                };

                var medicalOrganizationResult = _repository.GetLocalF003ByCode(Account.MedicalOrganization);
                if (medicalOrganizationResult.Success) {
                    var data = medicalOrganizationResult.Data;
                    if (data.IsNotNull())
                    {
                        Current.MedicalOrganizationDirector = "{0} {1}.{2}.".F(data.Surname, data.Name.FirstOrDefault(), data.Patronymic.FirstOrDefault());
                    }
                   
                }
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
            var meventResult = _repository.GetMedicalEventByPatientIdWithEqmaWithoutAct(Current.PatientId, _scope);
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
            var meventResult = _repository.GetMedicalEventByPatientIdWithEqmaWithAct(Current.PatientId, _scope);
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

        public ObservableCollection<SearchResultModel> PatientList
        {
            get { return _patientList; }
            set
            {
                _patientList = value;
                RaisePropertyChanged(() => PatientList);
            }
        }
        public object SelectedPatientId
        {
            get { return _selectedPatientId; }
            set
            {
                _selectedPatientId = value;
                RaisePropertyChanged(() => SelectedPatientId);

                if (_selectedPatientId.IsNotNull())
                {
                    Current.PatientId = _selectedPatientId.ToInt32();

                    LoadPerson();
                    LoadPatient();

                    LoadWithoutActMedicalEvent();
                }
            }
        }

        public ObservableCollection<SearchResultModel> MedicalEventList
        {
            get { return _medicalEventList; }
            set { _medicalEventList = value; RaisePropertyChanged(() => MedicalEventList); }
        }

        public List<object> SelectedMedicalEventIdsList
        {
            get { return _selectedMedicalEventIdsList; }
            set
            {
                _selectedMedicalEventIdsList = value;
                RaisePropertyChanged(() => SelectedMedicalEventIdsList);
                if (_selectedMedicalEventIdsList.IsNotNull())
                {
                    var medicalEventsResult = _repository.GetMedicalEventsByIds(_selectedMedicalEventIdsList);
                    if (medicalEventsResult.Success)
                    {

                        SelectedMedicalEvents = new List<EventCustomModel>(medicalEventsResult.Data.Select(p => new EventCustomModel
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
                            MedicalOrganizationCode = p.MedicalOrganizationCode,
                            ProfileCodeId = p.ProfileCodeId,
                            Outcome = p.Outcome,
                            DiagnosisComplication = p.DiagnosisComplicationAggregate
                        }));

                        Current.AcceptPrice = SelectedMedicalEvents.Sum(p => p.AcceptPrice);
                        Current.Price = SelectedMedicalEvents.Sum(p => p.Price);
                        Current.PenaltyPrice = 0;
                        Current.RefusalPrice = 0;

                        _eqmaList = new List<FactEQMA>();
                        for (int i = 0; i < SelectedMedicalEvents.Count(); i++)
                        {
                            var eqmaResult = _repository.GetEqmaByMedicalEventIdAndSource(SelectedMedicalEvents[i].EventId, _dataService.GetRefusalSourceByScope(_id, _scope));
                            if (eqmaResult.Success)
                            {
                                var eqma = eqmaResult.Data.FirstOrDefault();
                                SelectedMedicalEvents[i].ReasonId = eqma.ReasonId ?? 0;
                                SelectedMedicalEvents[i].PenaltyPrice = eqma.Penalty;
                                SelectedMedicalEvents[i].RefusalPrice = eqma.Amount;

                                Current.PenaltyPrice += eqma.Penalty;
                                Current.RefusalPrice += eqma.Amount;

                                _eqmaList.Add(eqma);
                            }
                        }

                        RaisePropertyChanged(() => RefusalCodeComposition);
                        RaisePropertyChanged(() => Current);
                    }
                }
            }
        }

        public bool IsPreview { get; set; }
        public localUser User { get; set; }
        public CommonAccountModel Account { get; set; }
        public FactTerritoryAccount TerritoryAccount { get; set; }
        public FactMedicalAccount MedicalAccount { get; set; }
        public FactPatient Patient
        {
            get { return _patient; }
            set
            {
                _patient = value;
                RaisePropertyChanged(() => Patient);
                RaisePropertyChanged(() => PolicyNumber);
                RaisePropertyChanged(() => Insurance);
            }
        }
        public FactPerson Person
        {
            get { return _person; }
            set
            {
                _person = value;
                RaisePropertyChanged(() => Person);
                RaisePropertyChanged(() => FullName);
                RaisePropertyChanged(() => Birthday);
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
                RaisePropertyChanged(() => Department);
                RaisePropertyChanged(() => DiagnosisComplicationComposition);
                
            }
        }


        public FactActEqma Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged("Current");
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

        public DateTime? Birthday
        {
            get
            {
                if (Person.IsNotNull())
                {
                    Current.PersonalBirthday = Person.Birthday;
                }
                else
                {
                    Current.PersonalBirthday = null;
                }
                return Current.PersonalBirthday;
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

        public string Insurance
        {
            get
            {
                if (Patient.IsNotNull())
                {
                    Current.Insurance = null;

                    if (Patient.InsuranceId.HasValue)
                    {
                        Current.Insurance = _cache.Get(CacheRepository.F002IdToNameCache).GetString(Patient.InsuranceId);
                    }
                }

                return Current.Insurance;
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
                    Current.Positions = SelectedMedicalEvents.Select(p => p.ExternalId).Distinct(p => p).AggregateToString();
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

        public string DiagnosisComplicationComposition
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.DiagnosisComplication = SelectedMedicalEvents.Select(p => p.DiagnosisComplication).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.DiagnosisComplication = null;
                }
                return Current.DiagnosisComplication;
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

        public string Department
        {
            get
            {
                if (SelectedMedicalEvents.IsNotNull() && SelectedMedicalEvents.Any())
                {
                    Current.Department = SelectedMedicalEvents.Select(p => p.Profile).Distinct(p => p).AggregateToString();
                }
                else
                {
                    Current.Department = null;
                }
                return Current.Department;
            }
        }
        
        #endregion

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
            var result = _repository.InsertOrUpdateEqmaAct(Current, _eqmaList.Select(p => p.EQMAId));
            if (result.Success)
            {
                _repository.DeletePreparedReportByExternalIdAndScope(result.Data, Constants.ScopeActEqma);
                dynamic acts = _settings.Get(AppRemoteSettings.ActIds);


                var reportResult = _reportService.PrepareReports(new List<object> { 
                    _scope == Constants.ScopeLocalAccount ? acts["ActMedicalEqma"] : acts["ActEqma"] },
                    _userId, result.Data, Constants.ScopeActEqma, _id, IsPreview);
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