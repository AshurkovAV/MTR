using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Autofac;
using BLToolkit.Mapping;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.EditableModels;
using Medical.AppLayer.Models.OperatorModels;
using Medical.AppLayer.Services;
using Medical.CoreLayer.PropertyGrid;
using Medical.CoreLayer.Service;
using Medical.CoreLayer.Validation.Algorithm;
using Medical.DatabaseCore.Services.Database;
using FactDocument = DataModel.FactDocument;
using FactEQMA = DataModel.FactEQMA;
using FactMEC = DataModel.FactMEC;
using FactMEE = DataModel.FactMEE;
using FactPerson = DataModel.FactPerson;
using Medical.AppLayer.Models.PolicySearch;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Operator.Views;
using CommonWindow = Medical.AppLayer.Editors.CommonWindow;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class OperatorViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;
        private readonly INotifyManager _notifyManager;
        private readonly IDataService _dataService;
        private readonly IProcessingService _processingService;
        private readonly IDockLayoutManager _dockLayoutManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private Dictionary<OperatorMode, Action> _modeHandlers;
        private Dictionary<int, MedicalServiceContainer> _serviceCache;

        private readonly int _id;
        private readonly OperatorMode _mode;
        private readonly IEnumerable<OperatorAction> _operatorActions;
        private readonly int _userId;

        private FactPerson _factPerson;
        private FactDocument _factDocument;

        private RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??
                                              (_saveCommand = new RelayCommand(Save, CanSave));
        private RelayCommand _revertCommand;
        public ICommand RevertCommand => _revertCommand ??
                                              (_revertCommand = new RelayCommand(Revert, CanRevert));
        private RelayCommand _checkPolicyCommand;
        public ICommand CheckPolicyCommand => _checkPolicyCommand ??
                                              (_checkPolicyCommand = new RelayCommand(CheckPolicy, CanCheckPolicy));
        private RelayCommand _viewSrzCommand;
        public ICommand ViewSrzCommand => _viewSrzCommand ??
                                              (_viewSrzCommand = new RelayCommand(ViewSrz, CanViewSrz));
        private RelayCommand _sendSrzQueryCommand;
        public ICommand SendSrzQueryCommand => _sendSrzQueryCommand ??
                                              (_sendSrzQueryCommand = new RelayCommand(SendQuerySrz, CanSendQuerySrz));
        private RelayCommand _searchDignosisCommand;
        public ICommand SearchDignosisCommand => _searchDignosisCommand ??
                                              (_searchDignosisCommand = new RelayCommand(SearchDiagnosis));
        private RelayCommand _generateInpCommand;
        public ICommand GenerateInpCommand => _generateInpCommand ??
                                              (_generateInpCommand = new RelayCommand(GenerateEnp, CanGenerateEnp));
        private RelayCommand _copyPatientCommand;
        public ICommand CopyPatientCommand => _copyPatientCommand ??
                                              (_copyPatientCommand = new RelayCommand(CopyPatient, CanCopyPatient));
        private RelayCommand _movePatientCommand;
        public ICommand MovePatientCommand => _movePatientCommand ??
                                              (_movePatientCommand = new RelayCommand(MovePatient, CanMovePatient));
        private RelayCommand _excludePatientCommand;
        public ICommand ExcludePatientCommand => _excludePatientCommand ??
                                              (_excludePatientCommand = new RelayCommand(ExcludePatient, CanExcludePatient));
        private RelayCommand _removeAllMecCommand;
        public ICommand RemoveAllMecCommand => _removeAllMecCommand ??
                                              (_removeAllMecCommand = new RelayCommand(RemoveAllRefusals, CanRemoveAllRefusals));

        public OperatorMode Mode { get { return _mode; }}
        public bool IsInterMode { get{ return Mode == OperatorMode.InterTerritorial ||  Mode == OperatorMode.InterTerritorialError || Mode == OperatorMode.InterTerritorialSrzQuery;}}
        public bool IsLocalMode { get { return Mode == OperatorMode.Local || Mode == OperatorMode.LocalError || Mode == OperatorMode.LocalSrzQuery; } }
        public List<int> PatientsIds { get; set; }
        public PatientModel Patient { get; set; }
        public PersonModel Person { get; set; }
        public DocumentModel Document { get; set; }
        public MedicalEventContainer Mevent { get; set; }
        public MedicalServiceContainer Mservice { get; set; }
        public RefusalContainer Refusal { get; set; }

        public bool NoServices
        {
            get { return Mservice.IsNull() || !Mservice.HasData; }
        }

        public bool Services
        {
            get { return Mservice.IsNotNull() && Mservice.HasData; }
        }

        private int _currentPatientPosition;
        private int? _goToPatientPosition;
        

        private RelayCommand _nextPatientCommand;
        private RelayCommand _prevPatientCommand;
        private RelayCommand _lastPatientCommand;
        private RelayCommand _firstPatientCommand;

        private RelayCommand _searchPolicyCommand;
        private RelayCommand _goToPatientPositionCommand;
        private RelayCommand<object> _goToPatientPositionKeyCommand;
        private RelayCommand _selectDiagnosisGeneralCommand;
        private RelayCommand _searchInsuranceCommand;
        private RelayCommand _selectMeventMedicalOrganizationCodeCommand;
        private RelayCommand _selectMeventDoctorCommand;
        private RelayCommand _selectMeventReferralOrganizationCommand;
        private RelayCommand _selectMeventDepartmentCommand;
        private RelayCommand _selectMeventDiagnosisPrimaryCommand;
        private RelayCommand _selectMeventDiagnosisSecondaryCommand;
        private RelayCommand _selectMserviceMedicalOrganizationCommand;
        private RelayCommand _selectMserviceDiagnosisCommand;
        private RelayCommand _selectMserviceDoctorCommand;
   
        private RelayCommand _deleteRefusalCommand;
        private RelayCommand _addMecCommand;
        private RelayCommand _addMeeCommand;
        private RelayCommand _addEqmaCommand;
        private RelayCommand _editRefuseCommand;
        private RelayCommand _blockRefuseCommand;
        private RelayCommand<object> _agreeRefuseCommand;

        #region IHash

        public string Hash
        {
            get { return typeof(OperatorViewModel).FullName + InnerName; }
        }

        public string InnerName { get; set; }

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

        public string CurrentPatientPositionText
        {
            get { return "{0} из {1}".F(_currentPatientPosition + 1, PatientsIds.Count); }
        }

        public int CurrentPatientPosition
        {
            get { return _currentPatientPosition; }
            set
            {
                if (!SaveCurrent())
                {
                    _messageService.ShowWarning("Ошибка сохранения данных");
                    return;
                }

                _currentPatientPosition = value; 
                RaisePropertyChanged(() => CurrentPatientPosition); 
                RaisePropertyChanged(() => CurrentPatientPositionText);
                if (PatientsIds.Count > 0)
                {
                    LoadPatientData(PatientsIds[CurrentPatientPosition]);
                }
                else
                {
                    LoadPatientData(0);
                }
                
            }
        }

        public int? GoToPatientPosition
        {
            get { return _goToPatientPosition; }
            set
            {
                _goToPatientPosition = value;
                RaisePropertyChanged(() => GoToPatientPosition); 
            }
        }
        

        public OperatorViewModel(IMedicineRepository repository, 
            IMessageService messageService, 
            INotifyManager notifyManager,
            IUserService userService,
            IDataService dataService,
            IProcessingService processingService,
            IDockLayoutManager dockLayoutManager,
            int id, 
            OperatorMode mode,
            IEnumerable<OperatorAction> operatorActions = null)
        {
            _id = id;
            _mode = mode;
            _userId = userService.UserId;
            _operatorActions = operatorActions;
            _repository = repository;
            _messageService = messageService;
            _notifyManager = notifyManager;
            _dataService = dataService;
            _processingService = processingService;
            _dockLayoutManager = dockLayoutManager;

            Initialize();

            InitMode();

            InitActions();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            _modeHandlers = new Dictionary<OperatorMode, Action>
            {
                {OperatorMode.InterTerritorial, InitInterTerritorial},
                {OperatorMode.InterTerritorialError, InitInterTerritorialError},
                {OperatorMode.InterTerritorialSrzQuery, InitInterTerritorialSrzQuery},
                {OperatorMode.Local, InitLocal},
                {OperatorMode.LocalError, InitLocalError},
                {OperatorMode.LocalSrzQuery, InitLocalSrzQuery},
                {OperatorMode.Patient, InitPatient}

            };

            _serviceCache = new Dictionary<int, MedicalServiceContainer>();
        }

        private bool CanCheckPolicy()
        {
            return Patient.INP.IsNotNullOrWhiteSpace() || Patient.InsuranceDocNumber.IsNotNullOrWhiteSpace();
        }

        private void CheckPolicy()
        {
            _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg);
            Task.Factory.StartNew(() =>
            {
            var checkResult = _processingService.CheckPatientPolicy(Patient.PatientId);
            if (checkResult.Success)
            {
                var applyResult = _processingService.ApplyPolicy(_id, new List<PolicyCheckPatientModel>{checkResult.Data}, Constants.ScopePatient.ToInt32());
                if (applyResult.Success)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => Refusal.ReloadRefusals());
                    
                    _notifyManager.ShowNotify("Полис пациента успешно проверен");
                }
            }
            }).ContinueWith(p => _dockLayoutManager.HideOverlay());
        }

        private bool CanRevert()
        {
            return Patient.IsDirty ||
                   Person.IsDirty ||
                   (Document.IsNotNull() && Document.IsDirty) ||
                   Mevent.IsDirty ||
                   (_serviceCache.IsNotNull() && _serviceCache.Any(p => p.Value.IsDirty));
        }

        private void Revert()
        {
            if (Patient.IsDirty)
            {
                Patient.RejectChanges();
            }

            if (Person.IsDirty)
            {
                Person.RejectChanges();
            }

            if (Document.IsNotNull() && Document.IsDirty)
            {
                Document.RejectChanges();
            }

            if (Mevent.IsNotNull() && Mevent.IsDirty)
            {
                Mevent.RejectChanges();
            }

            if (_serviceCache.IsNotNull() && _serviceCache.Any(p => p.Value.IsDirty))
            {
                _serviceCache.ForEachAction(p => p.Value.RejectChanges());
            }
        }

        private bool CanSave()
        {
            return Patient.IsDirty ||
                Person.IsDirty ||
                (Document.IsNotNull() && Document.IsDirty) ||
                Mevent.IsDirty ||
                (_serviceCache.IsNotNull() && _serviceCache.Any(p => p.Value.IsDirty));

        }

        private void Save()
        {
            if (!SaveCurrent())
            {
                _messageService.ShowWarning("Ошибка сохранения данных");
            }
        }

        private bool SaveCurrent()
        {
            if (Person.IsNotNull() && Person.IsDirty)
            {
                var updatedPerson = Person.Update(_factPerson);
                var updatedResult = _repository.Update(updatedPerson);
                if (updatedResult.HasError)
                {

                    return false;
                }
            }

            if (Patient.IsNotNull() && Patient.IsDirty)
            {
                var updatedPatient = Patient.Update();
                var updatedResult = _repository.Update(updatedPatient);
                if (updatedResult.HasError)
                {
                    
                    return false;
                }
            }

            if (Document.IsNotNull() && Document.IsDirty)
            {
                if (_factDocument.IsNotNull())
                {
                    var updatedDocument = Document.Update(_factDocument);
                    var updatedResult = _repository.Update(updatedDocument);
                    if (updatedResult.HasError)
                    {

                        return false;
                    }
                }
                else
                {
                    _factDocument = new FactDocument();
                    var updatedDocument = Document.Update(_factDocument);
                    updatedDocument.PersonId = Patient.PersonalId;
                    var insertResult = _repository.InsertWithIdentity(updatedDocument);
                    if (insertResult.HasError)
                    {
                        _factDocument = null;
                        return false;
                    }
                    Document.DocumentId = _factDocument.DocumentId = insertResult.Id;
                     
                }
            }

            if (Mevent.IsNotNull() && Mevent.IsDirty)
            {
                var meventSaveResult = Mevent.Save();
                if (meventSaveResult.HasError)
                {

                    return false;
                }
            }

            if (_serviceCache.IsNotNull() && _serviceCache.Any(p => p.Value.IsDirty))
            {
                foreach (var service in _serviceCache.Where(p=>p.Value.IsDirty).Select(p=>p.Value))
                {
                    var serviceSaveResult = service.Save();
                    if (serviceSaveResult.HasError)
                    {

                        return false;
                    }
                }
            }

            if (Person.IsNotNull())
            {
                Person.AcceptChanges();
            }
            if (Patient.IsNotNull())
            {
                Patient.AcceptChanges();
            }
            if (Document.IsNotNull())
            {
                Document.AcceptChanges();
            }
            if (Mevent.IsNotNull())
            {
                Mevent.AcceptChanges();
            }
            if (_serviceCache.IsNotNull())
            {
                foreach (var service in _serviceCache.Where(p => p.Value.IsDirty).Select(p => p.Value))
                {
                    service.AcceptChanges();
                }
            }
            
            return true;
        }


        public void InitMode()
        {
            if (_modeHandlers.ContainsKey(_mode))
            {
                _modeHandlers[_mode]();
            }
        }

        private void InitInterTerritorialCommon()
        {
            
        }

        private void InitInterTerritorial()
        {
            var result = _repository.GetPatientsIdsByAccountId(_id);
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitInterTerritorialCommon();
        }

        private void InitInterTerritorialError()
        {
            var result = _repository.GetPatientsIdsByAccountIdWithError(_id);
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitInterTerritorialCommon();
        }

        private void InitInterTerritorialSrzQuery()
        {
            var result = _repository.GetPatientsIdsByAccountIdWithSrzQuery(_id);
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitInterTerritorialCommon();
        }

        private void InitLocalCommon()
        {
            
        }

        private void InitLocal()
        {
            var result = _repository.GetPatientsIdsByMedicalAccountId(_id);
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitLocalCommon();
        }

        private void InitLocalError()
        {
            var result = _repository.GetPatientsIdsByMedicalAccountIdWithError(_id);
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitLocalCommon();
        }

        private void InitLocalSrzQuery()
        {
            var result = _repository.GetPatientsIdsByMedicalAccountIdWithSrzQuery(_id);
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitLocalCommon();
        }

        private void InitPatient()
        {
            PatientsIds = new List<int>{_id};
            CurrentPatientPosition = 0;
        }

        private void InitActions()
        {
            if (_operatorActions.IsNotNull())
            {
                var goToPatientAction = _operatorActions.FirstOrDefault(p => p.ActionType == OperatorActionType.GoToPatient);
                if (goToPatientAction.IsNotNull())
                {
                    CurrentPatientPosition = PatientsIds.FindIndex(i => i == goToPatientAction.Id);

                    var goToMedicalEventAction = _operatorActions.FirstOrDefault(p => p.ActionType == OperatorActionType.GoToMedicalEvent);
                    if (goToMedicalEventAction.IsNotNull())
                    {
                        Mevent.CurrentMedicalEventPosition = Mevent.MedicalEventIds.FindIndex(i => i == goToMedicalEventAction.Id);
                    }
                }
            }
        }

        private void LoadPatientData(int patientId)
        {
            if (patientId == 0)
            {
                Patient = null;
                Document = null;
                Person = null;
                
                RaisePropertyChanged(() => Patient);
                RaisePropertyChanged(() => Document);
                RaisePropertyChanged(() => Person);
                return;
            }

            
            var patientResult = _repository.GetPatientDocumentPerson(patientId);
            if (patientResult.Success)
            {
                var factPatient = patientResult.Data.Item1;
                _factDocument = patientResult.Data.Item2;
                _factPerson = patientResult.Data.Item3;

                Patient = Map.ObjectToObject<PatientModel>(factPatient, factPatient);
                Document = patientResult.Data.Item2 != null
                    ? Map.ObjectToObject<DocumentModel>(patientResult.Data.Item2)
                    : null;
                Person = Map.ObjectToObject<PersonModel>(patientResult.Data.Item3);

                RaisePropertyChanged(() => Patient);
                RaisePropertyChanged(() => Document);
                RaisePropertyChanged(() => Person);

                Mevent = Di.I.Resolve<MedicalEventContainer>(new NamedParameter("id", patientId),
                    new NamedParameter("changePosition", new Action<int>(i =>
                    {

                        if (_serviceCache.ContainsKey(i))
                        {
                            Mservice = _serviceCache[i];
                        }
                        else
                        {
                            Mservice = Di.I.Resolve<MedicalServiceContainer>(new NamedParameter("id", i));
                            _serviceCache[i] = Mservice;
                        }

                        RaisePropertyChanged(() => Mservice);

                        Refusal = Di.I.Resolve<RefusalContainer>(new NamedParameter("medicalEventId", i),
                            new NamedParameter("patientId", patientId),
                            new NamedParameter("mode", (int) _mode));
                        RaisePropertyChanged(() => Refusal);
                    })));

                RaisePropertyChanged(() => Mevent);
                RaisePropertyChanged(() => NoServices);
                RaisePropertyChanged(() => Services);
            }
        }


        #region Commands methods

        private bool CanViewSrz()
        {
            return true;
        }
        private void ViewSrz()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SrzResultViewModel>(new NamedParameter("patientId", Patient.PatientId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SrzResultView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке отправить запрос в СРЗ для пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(OperatorViewModel));
            }
        }

        private bool CanSendQuerySrz()
        {
            return true;
        }

        private void SendQuerySrz()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<CreateSrzQueryViewModel>(new NamedParameter("patientId", Patient.PatientId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new CreateSrzQueryView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке отправить запрос в СРЗ для пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(OperatorViewModel));
            }
        }

        private void SearchDiagnosis()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchDiagnosisViewModel>();

                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDiagnosisView());
                    model.CloseCallback = () => { view.Close(); };
                   
                    view.ShowDialog();

                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке скопировать пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(OperatorViewModel));
            }
            
        }

        public ICommand SelectDiagnosisGeneralCommand
        {
            get { return _selectDiagnosisGeneralCommand ?? (_selectDiagnosisGeneralCommand = new RelayCommand(SelectDiagnosisGeneral)); }
        }

        private void SelectDiagnosisGeneral()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchDiagnosisViewModel>(new NamedParameter("id", Mevent.MedicalEvent.DiagnosisGeneral));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDiagnosisView());
                   
                    model.OkCallback = i =>
                    {
                        Mevent.MedicalEvent.DiagnosisGeneral = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске диагноза произошла ошибка.", typeof(OperatorViewModel));
            }
        }
        

        private bool CanGenerateEnp()
        {
            return true;
        }

        private void GenerateEnp()
        {
            var territory = Patient.TerritoryOkato;
            var birthday = Person.Birthday;
            var sex = Person.Sex;
            if (!territory.HasValue || !birthday.HasValue || !sex.HasValue)
            {
                _messageService.ShowWarning("Для генерации ЕНП необходимо заполнить территорию страхования, дату рождения и пол пациента");
                return;
            }
            var inpGenerate = SrzAlgorithms.GenerateInp(territory.Value, birthday.Value, sex.Value);
            if (!string.IsNullOrWhiteSpace(inpGenerate))
            {
                Patient.InsuranceDocType = 3;
                Patient.InsuranceDocNumber = inpGenerate;
                Patient.INP = inpGenerate;
            }
            else
            {
                _messageService.ShowError("Ошибка генерации ЕНП");
            }
        }

        private bool CanRemoveAllRefusals()
        {
            return true;
        }

        private void RemoveAllRefusals()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить все отказы пациента ID {0}", "Внимание", Patient.PatientId))
            {
                _dockLayoutManager.ShowOverlay(Constants.RemoveRefusalsMsg, Constants.PleaseWaitMsg);
                Task.Factory.StartNew(() =>
                {
                    switch (_mode)
                    {
                        case OperatorMode.Local:
                        case OperatorMode.LocalError:
                        case OperatorMode.LocalSrzQuery:
                            _notifyManager.ShowNotify("Не реализовано.");
                            break;
                        case OperatorMode.InterTerritorial:
                        case OperatorMode.InterTerritorialError:
                        case OperatorMode.InterTerritorialSrzQuery:
                            var refusalsResult = _repository.GetF014();
                            if (refusalsResult.HasError)
                            {
                                _notifyManager.ShowNotify("Ошибка при получении списка отказов.");
                                return;
                            }
                            var refusals = refusalsResult.Data.Select(p => p.Id as int?).ToList();
                            var deleteTerritorialRefusal = _repository.DeletePatientTerritorialMec(_id, refusals, Patient.PatientId);
                            if (deleteTerritorialRefusal.Success)
                            {
                                Mevent.ReloadCurrentMedicalEvent();
                                Refusal.ReloadRefusals();
                                _notifyManager.ShowNotify("Отказы успешно удалены.");
                            }
                            break;
                    }
                }).ContinueWith(p => _dockLayoutManager.HideOverlay()); ;

            }
        }

        private bool CanExcludePatient()
        {
            return _mode == OperatorMode.InterTerritorial || 
                _mode == OperatorMode.InterTerritorialError || 
                _mode == OperatorMode.InterTerritorialSrzQuery && Patient.MedicalAccountId.HasValue;
        }

        private void ExcludePatient()
        {
            if (PatientsIds.Count == 1)
            {
                //TODO необходимо сделать возможность удалять последнего пациента в счете
                _messageService.ShowWarning("Пациент последний в счете. Нужно расформировать счет.");
                return;
            }

            var accountResult = _repository.GetTerritoryAccountViewById(_id);
            if (accountResult.Success)
            {
                var accountData = accountResult.Data;
                if (_messageService.AskQuestionFormatted("Исключить пациента ID {0} из счета ID {1}, № {2}, источник {3} от {4}?", "Внимание!",
                    Patient.PatientId,
                    accountData.TerritoryAccountId, 
                    accountData.AccountNumber, 
                    accountData.SourceName, 
                    accountData.AccountDate.HasValue ? accountData.AccountDate.Value.ToString(Constants.DefaultDateFormat): "дата отсутствует"))
                {
                    var excludeResult = _repository.ExcludePatientFromTerritorryAccount(Patient.PatientId, accountData.TerritoryAccountId);
                    if (excludeResult.Success)
                    {
                        PatientsIds.RemoveAt(CurrentPatientPosition);
                        RaisePropertyChanged(()=>PatientsIds);
                        CurrentPatientPosition = Math.Max(CurrentPatientPosition - 1, 0);

                        _notifyManager.ShowNotify("Пациент успешно исключен из счета.");
                    }
                }
            }
            
        }

        private bool CanMovePatient()
        {
            return true;
        }

        private void MovePatient()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<MovePatientViewModel>(new NamedParameter("patientId", Patient.PatientId),
                        new NamedParameter("version", Constants.Version21));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new MovePatientView());

                    model.SuccessCallback = () =>
                    {
                        PatientsIds.RemoveAt(CurrentPatientPosition);
                        RaisePropertyChanged(() => PatientsIds);
                        CurrentPatientPosition = Math.Max(CurrentPatientPosition - 1, 0);

                        _notifyManager.ShowNotify("Пациент успешно перенесен из счета.");
                        view.Close();
                    };
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке перенести пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(OperatorViewModel));
            }
        }

        private bool CanCopyPatient()
        {
            return true;
        }

        private void CopyPatient()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<CopyPatientViewModel>(new NamedParameter("patientId", Patient.PatientId),
                        new NamedParameter("mode", Mode));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 650, model, new CopyPatientView());
                    
                    model.SuccessCallback = () =>
                    {
                        _notifyManager.ShowNotify("Пациент успешно скопирован.");
                        view.Close();
                    };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке скопировать пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(OperatorViewModel));
            }
        }

        #endregion

        #region Navigate commands
        public ICommand NextPatientCommand {
            get { return _nextPatientCommand ?? (_nextPatientCommand = new RelayCommand(NextPatient, CanNextPatient)); }
        }

        private bool CanNextPatient()
        {
            return _currentPatientPosition < PatientsIds.Count-1;
        }

        private void NextPatient()
        {
            CurrentPatientPosition++;
        }

        public ICommand PrevPatientCommand
        {
            get { return _prevPatientCommand ?? (_prevPatientCommand = new RelayCommand(PrevPatient, CanPrevPatient)); }
        }

        private bool CanPrevPatient()
        {
            return _currentPatientPosition > 0;
        }

        private void PrevPatient()
        {
            CurrentPatientPosition--;
        }

        public ICommand FirstPatientCommand
        {
            get { return _firstPatientCommand ?? (_firstPatientCommand = new RelayCommand(FirstPatient, CanPrevPatient)); }
        }

        private void FirstPatient()
        {
            CurrentPatientPosition = 0;
        }

        public ICommand LastPatientCommand
        {
            get { return _lastPatientCommand ?? (_lastPatientCommand = new RelayCommand(LastPatient, CanNextPatient)); }
        }

        private void LastPatient()
        {
            CurrentPatientPosition = PatientsIds.Count - 1;
        }

        #endregion

        public ICommand SearchPolicyCommand
        {
            get { return _searchPolicyCommand ?? (_searchPolicyCommand = new RelayCommand(SearchPolicy, CanSearchPolicy)); }
        }

        private bool CanSearchPolicy()
        {
            return true;
        }

        private void SearchPolicy()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchPolicyViewModel>(new NamedParameter("patientId", Patient.PatientId), 
                        new NamedParameter("medicalEventId", Mevent.MedicalEvent.MedicalEventId),
                        new NamedParameter("version", Constants.Version21K));
                    model.SearchCommand.Execute(null);
                    var view = new CommonWindowControl(Application.Current.MainWindow, 900, 800, model, new SearchPolicyView());
                    
                    model.ApplyAction = form =>
                    {
                        Person.PName = form.Name;
                        Person.Surname = form.Surname;
                        Person.Patronymic = form.Patronymic;
                        Person.Birthday = form.BirthDate;
                        Person.Sex = form.Sex;
                        if ((PolicyType?)form.PolicyType == PolicyType.INP)
                        {
                            Patient.INP = form.INP;
                            Patient.InsuranceDocNumber = string.Empty;
                            Patient.InsuranceDocSeries = string.Empty;
                        }
                        else
                        {
                            Patient.InsuranceDocNumber = form.InsuranceNumber;
                            Patient.InsuranceDocSeries = form.InsuranceSeries;
                            Patient.INP = string.Empty;
                        }
                        Patient.InsuranceDocType = form.PolicyType;

                        view.Close();
                    };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске документов ОМС пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(OperatorViewModel));
            }
        }

        public ICommand SearchInsuranceCommand
        {
            get { return _searchInsuranceCommand ?? (_searchInsuranceCommand = new RelayCommand(SearchInsurance)); }
        }

        private void SearchInsurance()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchInsuranceViewModel>(new NamedParameter("id", Patient.InsuranceId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchInsuranceView());
                    
                    model.OkCallback = i =>
                    {
                        Patient.InsuranceId = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска СМО произошла ошибка.", typeof(OperatorViewModel));
            }
        }

        public ICommand SelectMeventMedicalOrganizationCodeCommand
        {
            get { return _selectMeventMedicalOrganizationCodeCommand ?? (_selectMeventMedicalOrganizationCodeCommand = new RelayCommand(SelectMeventMedicalOrganizationCode)); }
        }

        private void SelectMeventMedicalOrganizationCode()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchMedicalOrganizationViewModel>(new NamedParameter("code", Mevent.MedicalEvent.MedicalOrganizationCode));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchMedicalOrganizationView());
                   
                    model.OkCallback = i =>
                    {
                        Mevent.MedicalEvent.MedicalOrganizationCode = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(OperatorViewModel));
            }
        }

        public ICommand SelectMeventDoctorCommand
        {
            get { return _selectMeventDoctorCommand ?? (_selectMeventDoctorCommand = new RelayCommand(SelectMeventDoctor)); }
        }

        private void SelectMeventDoctor()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchDoctorViewModel>(new NamedParameter("code", Mevent.MedicalEvent.DoctorId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDoctorView());
                    
                    model.OkCallback = i =>
                    {
                        Mevent.MedicalEvent.DoctorId = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(OperatorViewModel));
            }
        }

        public ICommand SelectMeventReferralOrganizationCommand
        {
            get { return _selectMeventReferralOrganizationCommand ?? (_selectMeventReferralOrganizationCommand = new RelayCommand(SelectMeventReferralOrganization)); }
        }

        private void SelectMeventReferralOrganization()
        {
            //TODO
            _messageService.ShowWarning("В процессе разработки");
        }

        public ICommand SelectMeventDepartmentCommand
        {
            get { return _selectMeventDepartmentCommand ?? (_selectMeventDepartmentCommand = new RelayCommand(SelectMeventDepartment)); }
        }

        private void SelectMeventDepartment()
        {
            //TODO
            _messageService.ShowWarning("В процессе разработки");
        }

        public ICommand SelectMeventDiagnosisPrimaryCommand
        {
            get { return _selectMeventDiagnosisPrimaryCommand ?? (_selectMeventDiagnosisPrimaryCommand = new RelayCommand(SelectMeventDiagnosisPrimary)); }
        }

        private void SelectMeventDiagnosisPrimary()
        {
            //TODO
            _messageService.ShowWarning("В процессе разработки");
        }

        public ICommand SelectMeventDiagnosisSecondaryCommand
        {
            get { return _selectMeventDiagnosisSecondaryCommand ?? (_selectMeventDiagnosisSecondaryCommand = new RelayCommand(SelectMeventDiagnosisSecondary)); }
        }

        private void SelectMeventDiagnosisSecondary()
        {
            //TODO
            _messageService.ShowWarning("В процессе разработки");
        }

        public ICommand SelectMserviceMedicalOrganizationCommand
        {
            get { return _selectMserviceMedicalOrganizationCommand ?? (_selectMserviceMedicalOrganizationCommand = new RelayCommand(SelectMserviceMedicalOrganization)); }
        }

        private void SelectMserviceMedicalOrganization()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchMedicalOrganizationViewModel>(new NamedParameter("code", Mservice.MedicalService.MedicalOrganizationCode));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchMedicalOrganizationView());
                    
                    model.OkCallback = i =>
                    {
                        Mservice.MedicalService.MedicalOrganizationCode = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(OperatorViewModel));
            }
        }

        public ICommand SelectMserviceDiagnosisCommand
        {
            get { return _selectMserviceDiagnosisCommand ?? (_selectMserviceDiagnosisCommand = new RelayCommand(SelectMserviceDiagnosis)); }
        }

        private void SelectMserviceDiagnosis()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchDiagnosisViewModel>(new NamedParameter("id", Mservice.MedicalService.Diagnosis));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDiagnosisView());
                    
                    model.OkCallback = i =>
                    {
                        Mservice.MedicalService.Diagnosis = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске диагноза произошла ошибка.", typeof(OperatorViewModel));
            }
        }

        public ICommand SelectMserviceDoctorCommand
        {
            get { return _selectMserviceDoctorCommand ?? (_selectMserviceDoctorCommand = new RelayCommand(SelectMserviceDoctor)); }
        }

        private void SelectMserviceDoctor()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchDoctorViewModel>(new NamedParameter("code", Mservice.MedicalService.DoctorId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDoctorView());

                    model.OkCallback = i =>
                    {
                        Mservice.MedicalService.DoctorId = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(OperatorViewModel));
            }
        }

        #region Refusals

        public ICommand DeleteRefusalCommand
        {
            get
            {
                return _deleteRefusalCommand ?? (_deleteRefusalCommand = new RelayCommand(DeleteRefusal, CanDeleteRefusal));
            }
        }

        private bool CanDeleteRefusal()
        {
            return Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest != RefusalDest.In;

        }

        private void DeleteRefusal()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить отказ ID {0}", "Внимание", Refusal.SelectedRefusal.Id))
            {
                if (Refusal.SelectedRefusal.Id > 0)
                {
                    switch (_mode)
                    {
                        case OperatorMode.Local:
                        case OperatorMode.LocalError:
                        case OperatorMode.LocalSrzQuery:
                            var deleteRefusal = _repository.DeleteLocalRefusal(_id, Refusal.SelectedRefusal.Id, (int)Refusal.SelectedRefusal.RefusalType, UpdateFlag.MedicalEvent | UpdateFlag.Account);
                            if (deleteRefusal.Success)
                            {
                                Mevent.ReloadCurrentMedicalEvent();
                                Refusal.ReloadRefusals();
                            }
                        break;
                        case OperatorMode.InterTerritorial:
                        case OperatorMode.InterTerritorialError:
                        case OperatorMode.InterTerritorialSrzQuery:
                        var deleteTerritorialRefusal = _repository.DeleteTerritorialRefusal(_id, Refusal.SelectedRefusal.Id, (int)Refusal.SelectedRefusal.RefusalType, UpdateFlag.MedicalEvent | UpdateFlag.Account);
                        if (deleteTerritorialRefusal.Success)
                        {
                            Mevent.ReloadCurrentMedicalEvent();
                            Refusal.ReloadRefusals();
                        }
                        break;
                    }
                    
                }

            }

        }

        public ICommand AddMecCommand
        {
            get
            {
                return _addMecCommand ?? (_addMecCommand = new RelayCommand(AddMec));
            }
        }

        private void AddMec()
        {
            var source = _dataService.GetRefusalSource(_id, _mode);
            if (source == 0)
            {
                //TODO log error
                return;
            }

            
            var model = new PropertyGridViewModel<EditMecViewModel>(new EditMecViewModel(new FactMEC
            {
                MedicalEventId = Mevent.MedicalEvent.MedicalEventId,
                PatientId = Patient.PatientId,
                Date = DateTime.Now,
                Amount = Mevent.MedicalEvent.AcceptPrice,
                Source = source,
                EmployeeId = _userId,
                ExternalGuid = Guid.NewGuid().ToString()
            }));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactMEC>(model.SelectedObject.Classifier);
                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalMec(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialMec(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }
                
                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    _notifyManager.ShowNotify("МЭК успешно добавлен.");
                    view.Close();
                }
                
            };

            view.ShowDialog();
        }

        public ICommand AddMeeCommand
        {
            get
            {
                return _addMeeCommand ?? (_addMeeCommand = new RelayCommand(AddMee));
            }
        }

        private void AddMee()
        {
            var source = _dataService.GetRefusalSource(_id, _mode);
            if (source == 0)
            {
                //TODO log error
                return;
            }

            
            var editModel = new EditMeeViewModel(new FactMEE
            {
                MedicalEventId = Mevent.MedicalEvent.MedicalEventId,
                PatientId = Patient.PatientId,
                Date = DateTime.Now,
                Amount = Mevent.MedicalEvent.AcceptPrice,
                Source = source,
                EmployeeId = _userId,
                ExternalGuid = Guid.NewGuid().ToString(),
            });
            editModel.Price = Mevent.MedicalEvent.AcceptPrice;
            var model = new PropertyGridViewModel<EditMeeViewModel>(editModel);
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactMEE>(model.SelectedObject.Classifier);
                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalMee(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialMee(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }
                
                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    _notifyManager.ShowNotify("МЭЭ успешно добавлен.");
                    view.Close();
                }
                
            };

            view.ShowDialog();
        }

        public ICommand AddEqmaCommand
        {
            get
            {
                return _addEqmaCommand ?? (_addEqmaCommand = new RelayCommand(AddEqma));
            }
        }

        private void AddEqma()
        {
            var source = _dataService.GetRefusalSource(_id, _mode);
            if (source == 0)
            {
                //TODO log error
                return;
            }

            var model = new PropertyGridViewModel<EditEqmaViewModel>(new EditEqmaViewModel(new FactEQMA
            {
                MedicalEventId = Mevent.MedicalEvent.MedicalEventId,
                PatientId = Patient.PatientId,
                Date = DateTime.Now,
                Amount = Mevent.MedicalEvent.AcceptPrice,
                Source = source,
                EmployeeId = _userId,
                ExternalGuid = Guid.NewGuid().ToString()
            }));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactEQMA>(model.SelectedObject.Classifier);
                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalEqma(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialEqma(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }

                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    _notifyManager.ShowNotify("ЭКМП успешно добавлен.");
                    view.Close();
                }
            };

            view.ShowDialog();

        }

        public ICommand EditRefuseCommand
        {
            get
            {
                return _editRefuseCommand ?? (_editRefuseCommand = new RelayCommand(EditRefuse, CanEditRefuse));
            }
        }

        private bool CanEditRefuse()
        {
            return Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest != RefusalDest.In;

        }

        private void EditRefuse()
        {
            switch (Refusal.SelectedRefusal.RefusalType)
            {
                case RefusalType.MEC:
                    EditMec();
                break;
                case RefusalType.MEE:
                    EditMee();
                break;
                case RefusalType.EQMA:
                    EditEqma();
                break;
            }
        }

        private void EditEqma()
        {
            var eqmaResult = _repository.GetEqmaById(Refusal.SelectedRefusal.Id);
            if (eqmaResult.HasError)
            {
                //TODO error log
                return;
            }
            if (eqmaResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("ЭКМП ID {0} не найден", Refusal.SelectedRefusal.Id);
                return;
            }

            var model = new PropertyGridViewModel<EditEqmaViewModel>(new EditEqmaViewModel(eqmaResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactEQMA>(model.SelectedObject.Classifier);
                tmp.Date = DateTime.Now;
                tmp.EmployeeId = _userId;

                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalError:
                    case OperatorMode.LocalSrzQuery:
                        result  = _repository.InsertOrUpdateLocalEqma(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialEqma(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }

                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    _notifyManager.ShowNotify("Данные ЭКМП успешно обновлены.");
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        private void EditMee()
        {
            var meeResult = _repository.GetMeeById(Refusal.SelectedRefusal.Id);
            if (meeResult.HasError)
            {
                //TODO error log
                return;
            }
            if (meeResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("МЭЭ ID {0} не найден", Refusal.SelectedRefusal.Id);
                return;
            }

            var editModel = new EditMeeViewModel(meeResult.Data);
            editModel.Price = Mevent.MedicalEvent.AcceptPrice;

            var model = new PropertyGridViewModel<EditMeeViewModel>(editModel);
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactMEE>(model.SelectedObject.Classifier);
                tmp.Date = DateTime.Now;
                tmp.EmployeeId = _userId;

                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalMee(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialMee(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }

                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    _notifyManager.ShowNotify("Данные МЭЭ успешно обновлены.");
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        private void EditMec()
        {

            var mecResult = _repository.GetMecById(Refusal.SelectedRefusal.Id);
            if (mecResult.HasError)
            {
                //TODO error log
                return;
            }
            if (mecResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("МЭК ID {0} не найден", Refusal.SelectedRefusal.Id);
                return;
            }

            
            var model = new PropertyGridViewModel<EditMecViewModel>(new EditMecViewModel(mecResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<FactMEC>(model.SelectedObject.Classifier);
                tmp.Date = DateTime.Now;
                tmp.EmployeeId = _userId;

                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalMec(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.InterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialMec(tmp, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }

                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        public ICommand BlockRefuseCommand
        {
            get
            {
                return _blockRefuseCommand ?? (_blockRefuseCommand = new RelayCommand(BlockRefuse, CanBlockRefuse));
            }
        }

        private bool CanBlockRefuse()
        {
            return Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest != RefusalDest.In && Refusal.SelectedRefusal.RefusalType == RefusalType.MEC;

        }

        private void BlockRefuse()
        {
            var mecResult = _repository.GetMecById(Refusal.SelectedRefusal.Id);
            if (mecResult.HasError)
            {
                //TODO error log
                return;
            }
            if (mecResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("МЭК ID {0} не найден", Refusal.SelectedRefusal.Id);
                return;
            }

            TransactionResult blockResult;
            switch (_mode)
            {
                case OperatorMode.Local:
                case OperatorMode.LocalError:
                case OperatorMode.LocalSrzQuery:
                    blockResult = _repository.BlockLocalMec(mecResult.Data, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                    break;
                case OperatorMode.InterTerritorial:
                case OperatorMode.InterTerritorialError:
                case OperatorMode.InterTerritorialSrzQuery:
                    blockResult = _repository.BlockTerritorialMec(mecResult.Data, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                    break;
                default:
                    //TODO unsupported mode
                    return;
            }
            
            if (blockResult.Success)
            {
                Mevent.ReloadCurrentMedicalEvent();
                Refusal.ReloadRefusals();
                if (!mecResult.Data.IsLock.HasValue || mecResult.Data.IsLock == false)
                {
                    _notifyManager.ShowNotify("Отказ успешно разблокирован.");
                }
                else
                {
                    _notifyManager.ShowNotify("Отказ успешно заблокирован.");
                   
                }
                
            }
        }

        public ICommand AgreeRefuseCommand
        {
            get
            {
                return _agreeRefuseCommand ?? (_agreeRefuseCommand = new RelayCommand<object>(AgreeRefuse, CanAgreeRefuse));
            }
        }

        private bool CanAgreeRefuse(object arg)
        {
            return Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest == RefusalDest.In;

        }

        private void AgreeRefuse(object arg)
        {
            var externalRefuseResult = _repository.GetExternalRefuseById(Refusal.SelectedRefusal.Id);
            if (externalRefuseResult.HasError)
            {
                //TODO error log
                return;
            }
            if (externalRefuseResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Внешний отказ ID {0} не найден", Refusal.SelectedRefusal.Id);
                return;
            }

            TransactionResult updateResult;
            switch (_mode)
            {
                case OperatorMode.InterTerritorial:
                case OperatorMode.InterTerritorialError:
                case OperatorMode.InterTerritorialSrzQuery:
                    var flag = arg.ToInt32();
                    externalRefuseResult.Data.IsAgree = flag == 1; 
                    updateResult = _repository.InsertOrUpdateExternalRefuse(externalRefuseResult.Data, UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                    break;
                default:
                    //TODO unsupported mode
                    return;
            }

            if (updateResult.Success)
            {
                Mevent.ReloadCurrentMedicalEvent();
                Refusal.ReloadRefusals();
                if (externalRefuseResult.Data.IsAgree == false)
                {
                    _notifyManager.ShowNotify("Не согласны с отказом.");
                }
                else
                {
                    _notifyManager.ShowNotify("Согласны с отказом.");
                   

                }

            }
        }
        #endregion


        #region GoToPatientPosition
        public ICommand GoToPatientPositionCommand
        {
            get { return _goToPatientPositionCommand ?? (_goToPatientPositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        }

        private void GoToPosition()
        {
            if (GoToPatientPosition.HasValue)
            {
                CurrentPatientPosition = GoToPatientPosition.Value - 1;
                GoToPatientPosition = null;
            }
            
        }

        private bool CanGoToPosition()
        {
            return GoToPatientPosition.HasValue && GoToPatientPosition > 0 && GoToPatientPosition <= PatientsIds.Count;
        }

        public ICommand GoToPatientPositionKeyCommand
        {
            get { return _goToPatientPositionKeyCommand ?? (_goToPatientPositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
        }

        private bool CanGoToPositionByEnter(object obj)
        {
            if (obj is KeyEventArgs)
            {
                var key = obj as KeyEventArgs;
                if (key.IsDown && key.Key == Key.Enter)
                {
                    return CanGoToPosition();
                }
            }
            
            return false;
        }

        private void GoToPositionByEnter(object obj)
        {
            if (obj is KeyEventArgs)
            {
                var key = obj as KeyEventArgs;
                if (key.IsDown && key.Key == Key.Enter)
                {
                    GoToPosition();
                }
            }
        }
        #endregion
    }
}
