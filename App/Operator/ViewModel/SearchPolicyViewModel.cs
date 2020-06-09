using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Core;
using Core.DataStructure;
using Core.Extensions;
using DevExpress.Xpf.Editors;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Attributes;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.PolicySearch;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Classifiers;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SearchPolicyViewModel : ViewModelBase
    {
        private readonly IMedicineRepository _repository;
        private readonly IProcessingService _processingService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly INotifyManager _notifyManager;

        private readonly int _patientId;
        private readonly int _medicalEventId;
        private readonly int _version;

        private bool? _isValid;
        private Action _closeAction;
        private Action<PatientForm> _applyAction;

        private RelayCommand _resetCommand;
        private RelayCommand _searchCommand;
        private RelayCommand _applyCommand;

        private PatientForm _patient;
        public PatientForm Patient {
            get { return _patient; }
            set { _patient = value;RaisePropertyChanged(()=>Patient); }
        }

        private PatientForm _selectedPatient;
        private SearchPeopleByPolicyModel _selectedPeople;
        private PolicyByPeopleModel _selectedPolicy;

        public PatientForm SelectedPatient
        {
            get { return _selectedPatient; }
            set { _selectedPatient = value; RaisePropertyChanged(() => SelectedPatient); }
        }

        public ObservableCollection<CommonTuple> FieldsList { get; set; }
        public List<object> SelectedFieldsList { get; set; }

        public ObservableCollection<SearchPeopleByPolicyModel> PeopleList { get; set; }
        public ObservableCollection<PolicyByPeopleModel> PolicyList { get; set; }

        public Action<PatientForm> ApplyAction
        {
            get { return _applyAction; }
            set
            {
                _applyAction = value;
                RaisePropertyChanged(() => ApplyAction);
            }
        }

        public Action CloseAction
        {
            get { return _closeAction; }
            set { _closeAction = value; RaisePropertyChanged(() => CloseAction); }
        }

        public bool? IsValid
        {
            get
            {
                if (_selectedPolicy.IsNotNull())
                {
                    if (TerritoryService.TerritoryDefaultOmsVersion != 46)
                    {
                        return ((_selectedPolicy.DateBegin.Value <= Patient.EventBegin) 
                            && ((_selectedPolicy.DateStop == null) || (_selectedPolicy.DateStop.Value >= Patient.EventBegin)));
                    }
                    return ((_selectedPolicy.DateStop == null || (_selectedPolicy.DateStop.Value >= Patient.EventBegin))
                            && (_selectedPolicy.DateEnd == null || (_selectedPolicy.DateEnd.Value >= Patient.EventBegin)));
                }
                return null;
            }
        }

        public SearchPeopleByPolicyModel SelectedPeople
        {
            get { return _selectedPeople; }
            set
            {
                _selectedPeople = value; 
                RaisePropertyChanged(() => SelectedPeople);
                if (_selectedPeople.IsNotNull())
                {
                    SelectedPatient = new PatientForm
                    {
                        Name = _selectedPeople.FirstName,
                        Surname = _selectedPeople.Surname,
                        Patronymic = _selectedPeople.Patronymic,
                        Sex = _selectedPeople.Sex,
                        BirthDate = _selectedPeople.Birthday
                    };

                    var policyResult = _processingService.GetPolicyByPeopleId(_selectedPeople.Id);
                    if (policyResult.Success)
                    {
                        var policyList = policyResult.Data.ToList();
                        for (int i = 0; i < policyList.Count(); i++)
                        {
                            policyList[i].EventBegin = Patient.EventBegin;
                            policyList[i].EventEnd = Patient.EventEnd; 
                        }

                        PolicyList = new ObservableCollection<PolicyByPeopleModel>(policyList);
                        RaisePropertyChanged(()=>PolicyList);

                        SelectedPolicy = PolicyList
                            .LastOrDefault(p => (p.PolicyNumber == Patient.INP && p.PolicyType == PolicyType.INP.ToInt32()) 
                                || (p.PolicyNumber == Patient.InsuranceNumber && (p.PolicyType == PolicyType.Old.ToInt32()||p.PolicyType == PolicyType.Temporary.ToInt32())));
                        if (SelectedPolicy.IsNull())
                        {
                            SelectedPolicy = PolicyList.LastOrDefault();
                        }
                    }
                    
                }
                RaisePropertyChanged(() => PolicyList);
            }
        }

        public PolicyByPeopleModel SelectedPolicy
        {
            get { return _selectedPolicy; }
            set
            {
                _selectedPolicy = value;
                RaisePropertyChanged(() => SelectedPolicy);
                RaisePropertyChanged(() => IsValid);
                if (_selectedPolicy.IsNotNull() && SelectedPatient.IsNotNull())
                {
                    SelectedPatient.InsuranceNumber = SelectedPolicy.PolicyType == 3 ? null : SelectedPolicy.PolicyNumber;
                    SelectedPatient.InsuranceSeries = SelectedPolicy.PolicyType == 3 ? null : SelectedPolicy.PolicySerial;
                    SelectedPatient.PolicyType = SelectedPolicy.PolicyType;
                    SelectedPatient.INP = SelectedPolicy.PolicyType == 3 ? SelectedPolicy.PolicyNumber : null;
                }
                RaisePropertyChanged(() => SelectedPatient);
            }
        }
        

        public SearchPolicyViewModel(IMedicineRepository repository,
            IDockLayoutManager dockLayoutManager,
            IProcessingService processingService,
            INotifyManager notifyManager,
            int patientId,
            int medicalEventId,
            int version)
        {
            _patientId = patientId;
            _medicalEventId = medicalEventId;
            _repository = repository;
            _notifyManager = notifyManager;
            _dockLayoutManager = dockLayoutManager;
            _processingService = processingService;
            _version = version;

            Init();
            
        }

        private void Init()
        {
            FieldsList = new ObservableCollection<CommonTuple>(FlagExtension.ToList<SearchPolicyFlag>());

            SelectedFieldsList = new List<object>(){
                SearchPolicyFlag.Name.ToInt32(),
                SearchPolicyFlag.Surname.ToInt32(),
                SearchPolicyFlag.Patronymic.ToInt32(),
                SearchPolicyFlag.Birthday.ToInt32()
            };


            var patientResult = _repository.GetPatientDocumentPerson(_patientId);
            if (patientResult.Success)
            {
                if (Constants.Zversion.Contains(_version))
                {
                    var eventResult = _repository.GetZslMedicalEventById(_medicalEventId);
                    if (eventResult.Success)
                    {
                        Patient = new PatientForm
                        {
                            Name = patientResult.Data.Item3.PName,
                            Surname = patientResult.Data.Item3.Surname,
                            Patronymic = patientResult.Data.Item3.Patronymic,
                            BirthDate = patientResult.Data.Item3.Birthday,
                            Sex = patientResult.Data.Item3.Sex,
                            INP = patientResult.Data.Item1.INP,
                            InsuranceNumber = patientResult.Data.Item1.InsuranceDocNumber,
                            InsuranceSeries = patientResult.Data.Item1.InsuranceDocSeries,
                            PolicyType = patientResult.Data.Item1.InsuranceDocType,
                            EventBegin = eventResult.Data.EventBeginZ1,
                            EventEnd = eventResult.Data.EventEndZ2
                        };
                    }
                }
                else
                {
                    var eventResult = _repository.GetMedicalEventById(_medicalEventId);
                    if (eventResult.Success)
                    {
                        Patient = new PatientForm
                        {
                            Name = patientResult.Data.Item3.PName,
                            Surname = patientResult.Data.Item3.Surname,
                            Patronymic = patientResult.Data.Item3.Patronymic,
                            BirthDate = patientResult.Data.Item3.Birthday,
                            Sex = patientResult.Data.Item3.Sex,
                            INP = patientResult.Data.Item1.INP,
                            InsuranceNumber = patientResult.Data.Item1.InsuranceDocNumber,
                            InsuranceSeries = patientResult.Data.Item1.InsuranceDocSeries,
                            PolicyType = patientResult.Data.Item1.InsuranceDocType,
                            EventBegin = eventResult.Data.EventBegin,
                            EventEnd = eventResult.Data.EventEnd
                        };
                    }
                }
            }
            
            SelectedPatient = new PatientForm();
        }

        public ICommand ResetCommand
        {
            get { return _resetCommand ?? (_resetCommand = new RelayCommand(Reset));}
        }

        private void Reset()
        {
            Debug.WriteLine("Reset command");
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand(Search, CanSearch)); }
        }

        private bool CanSearch()
        {
            return SelectedFieldsList.IsNotNull() && SelectedFieldsList.Any();
        }

        private void Search()
        {
            PeopleList = new ObservableCollection<SearchPeopleByPolicyModel>();
            string policyNumber = string.Empty;
            string policySeries = string.Empty;
            string inp = string.Empty;
            string name = string.Empty;
            string surname = string.Empty;
            string patronymic = string.Empty;
            DateTime? birthday = null;
            int? sex = null;

            var value = (SearchPolicyFlag)SelectedFieldsList.Select(Convert.ToInt32).Sum();
            if (value.HasFlag(SearchPolicyFlag.Inp))
            {
                inp = Patient.INP ?? string.Empty;
            }
            if (value.HasFlag(SearchPolicyFlag.Number))
            {
                policyNumber = Patient.InsuranceNumber ?? string.Empty;
            }
            if (value.HasFlag(SearchPolicyFlag.Series))
            {
                policySeries = Patient.InsuranceSeries ?? string.Empty;
            }
            if (value.HasFlag(SearchPolicyFlag.Name))
            {
                name = Patient.Name ?? string.Empty;
            }
            if (value.HasFlag(SearchPolicyFlag.Surname))
            {
                surname = Patient.Surname ?? string.Empty;
            }
            if (value.HasFlag(SearchPolicyFlag.Patronymic))
            {
                patronymic = Patient.Patronymic ?? string.Empty;
            }
            if (value.HasFlag(SearchPolicyFlag.Birthday))
            {
                birthday = Patient.BirthDate;
            }
            if (value.HasFlag(SearchPolicyFlag.Sex))
            {
                sex = Patient.Sex;
            }


            _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg);
            Task.Factory.StartNew(() =>
            {
                if (value.HasFlag(SearchPolicyFlag.Inp))
                {
                    var peopleByInpResult = _processingService.SearchPeopleByPolicy((int?)PolicyType.INP,  inp, null);
                    if (peopleByInpResult.Success && peopleByInpResult.Data.Any())
                    {
                        PeopleList.AddRange(peopleByInpResult.Data);
                        _notifyManager.ShowNotify("Поиск завершен");
                    }
                    else if (peopleByInpResult.Success && !peopleByInpResult.Data.Any())
                    {
                        _notifyManager.ShowNotify("С таким условием в базе SRZ нет данных");
                    }
                    else if (peopleByInpResult.HasError)
                    {
                        _notifyManager.ShowNotify(peopleByInpResult.LastError.Message);
                    }
                }

                if (value.HasFlag(SearchPolicyFlag.Number) || value.HasFlag(SearchPolicyFlag.Series))
                {
                    var peopleByOldResult = _processingService.SearchPeopleByPolicy((int?)PolicyType.Old, policyNumber, policySeries);
                    if (peopleByOldResult.Success && peopleByOldResult.Data.Any())
                    {
                        PeopleList.AddRange(peopleByOldResult.Data);
                        _notifyManager.ShowNotify("Поиск завершен");
                    }
                    else if (peopleByOldResult.Success && !peopleByOldResult.Data.Any())
                    {
                        _notifyManager.ShowNotify("С таким условием в базе SRZ нет данных");
                    }
                    else if (peopleByOldResult.HasError)
                    {
                        _notifyManager.ShowNotify(peopleByOldResult.LastError.Message);
                    }

                    var peopleByTmpResult = _processingService.SearchPeopleByPolicy((int?)PolicyType.Temporary, policyNumber, policySeries);
                    if (peopleByTmpResult.Success && peopleByTmpResult.Data.Any())
                    {
                        PeopleList.AddRange(peopleByTmpResult.Data);
                        _notifyManager.ShowNotify("Поиск завершен");
                    }
                    else if (peopleByTmpResult.Success && !peopleByTmpResult.Data.Any())
                    {
                        _notifyManager.ShowNotify("С таким условием в базе SRZ нет данных");
                    }
                    else if (peopleByTmpResult.HasError)
                    {
                        _notifyManager.ShowNotify(peopleByTmpResult.LastError.Message);
                    }
                }

                if (value.HasFlag(SearchPolicyFlag.Name) || value.HasFlag(SearchPolicyFlag.Surname) || value.HasFlag(SearchPolicyFlag.Patronymic) || value.HasFlag(SearchPolicyFlag.Birthday) || value.HasFlag(SearchPolicyFlag.Sex))
                {
                    var peopleByPersonResult = _processingService.SearchPeopleByPersonal(surname, name, patronymic, birthday, sex);
                    if (peopleByPersonResult.Success && peopleByPersonResult.Data.Any())
                    {
                        PeopleList.AddRange(peopleByPersonResult.Data);
                        _notifyManager.ShowNotify("Поиск завершен");
                    }
                    else if(peopleByPersonResult.Success && !peopleByPersonResult.Data.Any())
                    {
                        _notifyManager.ShowNotify("С таким условием в базе SRZ нет данных");
                    }
                    else if (peopleByPersonResult.HasError)
                    {
                        _notifyManager.ShowNotify(peopleByPersonResult.LastError.Message);
                    }
                }

                if (PeopleList.Count == 0 ) {
                    SelectedPeople = null;
                    PolicyList = null;
                    SelectedPolicy = null;
                }

                RaisePropertyChanged(()=>PeopleList);

            }).ContinueWith(p => _dockLayoutManager.HideOverlay());
        }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply, CanApply)); }
        }

        private bool CanApply()
        {
            return ApplyAction.IsNotNull();
        }

        private void Apply()
        {
            if (ApplyAction.IsNotNull() && SelectedPatient.IsNotNull())
            {
                ApplyAction(SelectedPatient);
            }
        }
    }

    public class PatientForm : ViewModelBase
    {
        private string _insuranceNumber;
        private string _insuranceSeries;
        private string _inp;
        private string _surname;
        private string _name;
        private string _patronymic;
        private DateTime? _birthDate;
        private int? _sex;
        private int? _policyType;
        private DateTime? _eventBegin;
        private DateTime? _eventEnd;

        [Display(GroupName = @"Документы ОМС", Name = @"Номер документа ОМС")]
        public string InsuranceNumber {
            get { return _insuranceNumber; }
            set { _insuranceNumber = value; RaisePropertyChanged(()=>InsuranceNumber); }
        }

        [Display(GroupName = @"Документы ОМС", Name = @"Серия документа ОМС")]
        public string InsuranceSeries
        {
            get { return _insuranceSeries; }
            set { _insuranceSeries = value; RaisePropertyChanged(() => InsuranceSeries); }
        }

        [Display(GroupName = @"Документы ОМС", Name = @"ЕНП")]
        public string INP
        {
            get { return _inp; }
            set { _inp = value; RaisePropertyChanged(() => INP); }
        }

        [Display(GroupName = @"Персональные данные", Name = @"Фамилия")]
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; RaisePropertyChanged(() => Surname); }
        }

        [Display(GroupName = @"Персональные данные", Name = @"Имя")]
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }

        [Display(GroupName = @"Персональные данные", Name = @"Отчество")]
        public string Patronymic
        {
            get { return _patronymic; }
            set { _patronymic = value; RaisePropertyChanged(() => Patronymic); }
        }

        [Display(GroupName = @"Персональные данные", Name = @"Дата рождения")]
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; RaisePropertyChanged(() => BirthDate); }
        }

        [Display(GroupName = @"Персональные данные", Name = @"Пол")]
        [CustomEditor(EditorType = typeof (ComboBoxEdit), ItemSourceType = typeof (V005ItemsSource), Value = "Value",
            DisplayName = "DisplayName")]
        public int? Sex
        {
            get { return _sex; }
            set { _sex = value; RaisePropertyChanged(() => Sex); }
        }

        [Display(GroupName = @"Данные случая", Name = @"Дата начала")]
        public DateTime? EventBegin
        {
            get { return _eventBegin; }
            set { _eventBegin = value; RaisePropertyChanged(() => EventBegin); }
        }

        [Display(GroupName = @"Данные случая", Name = @"Дата окончания")]
        public DateTime? EventEnd
        {
            get { return _eventEnd; }
            set { _eventEnd = value; RaisePropertyChanged(() => EventEnd); }
        }


        [Display(AutoGenerateField = false)]
        public int? PolicyType
        {
            get { return _policyType; }
            set { _policyType = value; RaisePropertyChanged(() => PolicyType); }
        }

        [Display(AutoGenerateField = false)]
        public new bool IsInDesignMode { get; set; }
    }
}
