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
using DataModel;
using FastReport.DevComponents.DotNetBar;
using GalaSoft.MvvmLight.Threading;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Models.Validators;
using Medical.AppLayer.Operator.Views;
using Medical.CoreLayer.Models.Common;
using Medical.DatabaseCore.Services.Classifiers;
using CommonWindow = Medical.AppLayer.Editors.CommonWindow;
using ICommand = System.Windows.Input.ICommand;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class ZslOperatorViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;
        private readonly INotifyManager _notifyManager;
        private readonly IDataService _dataService;
        private readonly IProcessingService _processingService;
        private readonly IDockLayoutManager _dockLayoutManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private Dictionary<OperatorMode, Action> _modeHandlers;
        private Dictionary<int, ZMedicalServiceContainer> _zserviceCache;
        private Dictionary<int, ZMedicalServiceOnkContainer> _zserviceOnkCache;
        private Dictionary<int, ZMedicalEventContainer> _zMedicalEventCache;

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

        public bool IsVersion31
        {
            get { return _version == Constants.Version31K || _version == Constants.Version31 || _version == Constants.Version32K || _version == Constants.Version32; }
        }

        public bool IsInterMode { get{ return Mode == OperatorMode.InterTerritorial ||  Mode == OperatorMode.InterTerritorialError || Mode == OperatorMode.InterTerritorialSrzQuery || Mode == OperatorMode.ZInterTerritorial;}}
        public bool IsLocalMode { get { return Mode == OperatorMode.Local || Mode == OperatorMode.LocalError || Mode == OperatorMode.LocalSrzQuery || Mode == OperatorMode.Zlocal; } }
        public List<int> PatientsIds { get; set; }
        public PatientModel Patient { get; set; }
        public PersonModel Person { get; set; }
        public DocumentModel Document { get; set; }
        public ZslMedicalEventContainer Mevent { get; set; }
        public ZMedicalServiceContainer Mservice { get; set; }
        public ZMedicalServiceOnkContainer MserviceOnk { get; set; }
        public ZKsgKpgContainer KKsg { get; set; }
        public ZMedicalEventOnkContainer ZmedicalEventOnk { get; set; }
        public ZRefusalContainer Refusal { get; set; }
        public ZMedicalEventContainer ZmedicalEvent { get; set; }
        public ZSlKoefContainer ZSlkoef { get; set; }
        public ZSlCritContainer ZSlCrit { get; set; }
        public ZDiagBlokContainer ZDiagBlok { get; set; }
        public ZDsContainer ZDs { get; set; }
        public ZAnticancerDrugContainer ZAnticancerDrug { get; set; }
        public ZMedicalConsultationsOnkContainer ZConsultations { get; set; }
        public ZDirectionContainer ZDirection { get; set; }
        public ZContraindicationsContainer ZContraindications { get; set; }

        public bool NoServices
        {
            get { return (Mservice.IsNull() || !Mservice.HasData) && (MserviceOnk.IsNull() || !MserviceOnk.HasData) && (ZDirection.IsNull() || !ZDirection.HasData); }
        }

        public bool Services
        {
            get { return (Mservice.IsNotNull() && Mservice.HasData) || (MserviceOnk.IsNotNull() && MserviceOnk.HasData) || (ZDirection.IsNotNull() && ZDirection.HasData); }
        }

        public bool NoConsultations
        {
            get { return (ZConsultations.IsNull() || !ZConsultations.HasData); }
        }

        public bool Consultations
        {
            get { return (ZConsultations.IsNotNull() && ZConsultations.HasData); }
        }

        public bool NoServices1
        {
            get { return (Mservice.IsNull() || !Mservice.HasData); }
        }

        public bool Services1
        {
            get { return (Mservice.IsNotNull() && Mservice.HasData); }
        }

        public bool NoServices1Onk
        {
            get { return (MserviceOnk.IsNull() || !MserviceOnk.HasData); }
        }

        public bool Services1Onk
        {
            get { return (MserviceOnk.IsNotNull() && MserviceOnk.HasData); }
        }

        public bool NoNapr
        {
            get { return (ZDirection.IsNull() || !ZDirection.HasData); }
        }

        public bool Napr
        {
            get { return (ZDirection.IsNotNull() && ZDirection.HasData); }
        }

        public bool OneServ
        {
            get { return Mservice.IsNotNull() && Mservice.HasData; }
        }

        private int _currentPatientPosition;
        private int? _goToPatientPosition;
        private int? _version;
        

        private RelayCommand _nextPatientCommand;
        private RelayCommand _prevPatientCommand;
        private RelayCommand _lastPatientCommand;
        private RelayCommand _firstPatientCommand;

        private RelayCommand _searchPolicyCommand;
        private RelayCommand _goToPatientPositionCommand;
        private RelayCommand<object> _goToPatientPositionKeyCommand;
        private RelayCommand _selectDiagnosisGeneralCommand;
        private RelayCommand _selectKsgKpgCommand;
        private RelayCommand _selectHighTechAssistanceTypeCommand;
        private RelayCommand _selectHighTechAssistanceMethodCommand ;
        private RelayCommand _searchInsuranceCommand;
        private RelayCommand _selectMeventMedicalOrganizationCodeCommand;
        private RelayCommand _selectMeventDoctorCommand;
        private RelayCommand _selectMeventReferralOrganizationCommand;
        private RelayCommand _selectDirectionMoCommand; 
        private RelayCommand _selectMeventDepartmentCommand;
        private RelayCommand _selectMeventDiagnosisPrimaryCommand;
        private RelayCommand _selectMeventDiagnosisSecondaryCommand;
        private RelayCommand _selectMserviceMedicalOrganizationCommand;
        private RelayCommand _selectMserviceDiagnosisCommand;
        private RelayCommand _selectMserviceDoctorCommand;
   
        private RelayCommand _deleteRefusalCommand;
        private RelayCommand _deleteDiagBlokCommand;
        private RelayCommand _deleteDsCommand;
        private RelayCommand _deleteAnticancerDrugCommand;
        private RelayCommand _addDiagBlokCommand;
        private RelayCommand _addDsCommand;
        private RelayCommand _addAnticancerDrugCommand;
        private RelayCommand _editDiagBlokCommand;
        private RelayCommand _editDsCommand;
        private RelayCommand _editAnticancerDrugCommand;
        private RelayCommand _deleteContraindicationsCommand;
        private RelayCommand _addContraindicationsCommand;
        private RelayCommand _editContraindicationsCommand; 
        private RelayCommand _deleteDirectionCommand;
        private RelayCommand _addDirectionCommand;
        private RelayCommand _editDirectionCommand;
        private RelayCommand _addMecCommand;
        private RelayCommand _addMeeCommand;
        private RelayCommand _addEqmaCommand;
        private RelayCommand _editRefuseCommand;
        private RelayCommand _blockRefuseCommand;
        private RelayCommand<object> _agreeRefuseCommand;

        #region IHash

        public string Hash
        {
            get { return typeof(ZslOperatorViewModel).FullName + InnerName; }
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
        

        public ZslOperatorViewModel(IMedicineRepository repository, 
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

            InitVersion();

            Initialize();

            InitMode();

            InitActions();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
            Revert();
        }

        private void Initialize()
        {
            _modeHandlers = new Dictionary<OperatorMode, Action>
            {
                {OperatorMode.ZInterTerritorial, InitInterTerritorial},
                {OperatorMode.ZInterTerritorialError, InitInterTerritorialError},
                {OperatorMode.InterTerritorialSrzQuery, InitInterTerritorialSrzQuery},
                {OperatorMode.ZLocalError, InitLocalError},
                {OperatorMode.LocalSrzQuery, InitLocalSrzQuery},
                {OperatorMode.Patient, InitPatient},
                {OperatorMode.Zlocal,  InitZlocal}

            };
            _zserviceCache = new Dictionary<int, ZMedicalServiceContainer>();
            _zserviceOnkCache = new Dictionary<int, ZMedicalServiceOnkContainer>();
        }

        private void InitVersion()
        {
            var result = _repository.GetMedicalAccountById(_id);
            if (result.Success)
            {
                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalSrzQuery:
                    case OperatorMode.Zlocal:
                    case OperatorMode.ZLocalError:
                        _version = _repository.GetMedicalAccountById(_id).Data?.Version;
                        break;
                    case OperatorMode.InterTerritorial:
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.ZInterTerritorialError:
                        _version = _repository.GetTerritoryAccountById(_id).Data?.Version;
                        break;
                }

            }
            else
            {

                switch (_mode)
                {
                    case OperatorMode.Local:
                    case OperatorMode.LocalSrzQuery:
                    case OperatorMode.InterTerritorial:
                        _version = Constants.Version21;
                        break;
                    case OperatorMode.Zlocal:
                    case OperatorMode.ZLocalError:
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.ZInterTerritorialError:
                        _version = Constants.Version30;
                        break;
                }
            }



        }

        private bool CanCheckPolicy()
        {
            return Patient.INP.IsNotNullOrWhiteSpace() || Patient.InsuranceDocNumber.IsNotNullOrWhiteSpace();
        }

        private void CheckPolicy()
        {
            _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg);
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var checkResult = _processingService.CheckZslPatientPolicy(Patient.PatientId);
                    if (checkResult.Success)
                    {
                        var applyResult = _processingService.ApplyPolicy(_id, new List<PolicyCheckPatientModel> { checkResult.Data }, Constants.ScopePatient.ToInt32());
                        if (applyResult.Success)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => Refusal.ReloadRefusals());

                            _notifyManager.ShowNotify("Полис пациента успешно проверен");
                        }
                    }
                    else
                    {
                        _notifyManager.ShowNotify($"Произошла ошибка {checkResult.LastError}");
                    }
                }).ContinueWith(p => _dockLayoutManager.HideOverlay());
                

            }
            catch (Exception ex)
            {
                _notifyManager.ShowNotify($"Произошла ошибка {ex.Message}");
            }
            
        }

        private bool CanRevert()
        {
            return Patient.IsDirty ||
                   Person.IsDirty ||
                   (Document.IsNotNull() && Document.IsDirty) ||
                   Mevent.IsDirty ||
                   (_zserviceCache.IsNotNull() && _zserviceCache.Any(p => p.Value.IsDirty)) ||
                   (_zserviceOnkCache.IsNotNull() && _zserviceOnkCache.Any(p => p.Value.IsDirty)) ||
                   (ZmedicalEvent.IsNotNull() && ZmedicalEvent.IsDirty) ||
                   (KKsg.IsNotNull() && KKsg.IsDirty) ||
                   (ZSlkoef.IsNotNull() && ZSlkoef.IsDirty) ||
                   (ZConsultations.IsNotNull() && ZConsultations.IsDirty) ||
                   (ZDirection.IsNotNull() && ZDirection.IsDirty) ||
                   (ZmedicalEventOnk.IsNotNull() && ZmedicalEventOnk.IsDirty);


        }

        //private bool CanRevert()
        //{
        //    return Patient.IsDirty ||
        //           Person.IsDirty ||
        //           (Document.IsNotNull() && Document.IsDirty) ||
        //           Mevent.IsDirty ||
        //           (_zserviceCache.IsNotNull() && _zserviceCache.Any(p => p.Value.IsDirty)) ||
        //           (_zserviceOnkCache.IsNotNull() && _zserviceOnkCache.Any(p => p.Value.IsDirty)) ||
        //           ZmedicalEvent.IsDirty ||
        //           ZmedicalEventOnk.IsDirty ||
        //           KKsg.IsDirty ||
        //           ZDirection.IsDirty ||
        //           ZConsultations.IsDirty ||
        //           (ZSlkoef.IsNotNull() && ZSlkoef.IsDirty);


        //}

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
            if (ZmedicalEvent.IsNotNull() && ZmedicalEvent.IsDirty)
            {
                ZmedicalEvent.RejectChanges();
            }
            if (ZmedicalEventOnk.IsNotNull() && ZmedicalEventOnk.IsDirty)
            {
                ZmedicalEventOnk.RejectChanges();
            }
            if (KKsg.IsNotNull() && KKsg.IsDirty)
            {
                KKsg.RejectChanges();
            }
            if (ZDirection.IsNotNull() && ZDirection.IsDirty)
            {
                ZDirection.RejectChanges();
            }
            if (ZConsultations.IsNotNull() && ZConsultations.IsDirty)
            {
                ZConsultations.RejectChanges();
            }

            if (_zserviceCache.IsNotNull() && _zserviceCache.Any(p => p.Value.IsDirty))
            {
                _zserviceCache.ForEachAction(p => p.Value.RejectChanges());
            }
            if (_zserviceOnkCache.IsNotNull() && _zserviceOnkCache.Any(p => p.Value.IsDirty))
            {
                _zserviceOnkCache.ForEachAction(p => p.Value.RejectChanges());
            }
        }

        private bool CanSave()
        {
            return Patient.IsDirty ||
                   Person.IsDirty ||
                   (Document.IsNotNull() && Document.IsDirty) ||
                   Mevent.IsDirty ||
                   (_zserviceCache.IsNotNull() && _zserviceCache.Any(p => p.Value.IsDirty)) ||
                   (_zserviceOnkCache.IsNotNull() && _zserviceOnkCache.Any(p => p.Value.IsDirty)) ||
                   (ZmedicalEvent.IsNotNull() && ZmedicalEvent.IsDirty) ||
                   (KKsg.IsNotNull() && KKsg.IsDirty) ||
                   (ZSlkoef.IsNotNull() && ZSlkoef.IsDirty) ||
                   (ZConsultations.IsNotNull() && ZConsultations.IsDirty) ||
                   (ZDirection.IsNotNull() && ZDirection.IsDirty) ||
                   (ZmedicalEventOnk.IsNotNull() && ZmedicalEventOnk.IsDirty);
        }

        //return Patient.IsDirty ||
        //Person.IsDirty ||
        //(Document.IsNotNull() && Document.IsDirty) ||
        //Mevent.IsDirty ||
        //(_zserviceCache.IsNotNull() && _zserviceCache.Any(p => p.Value.IsDirty)) ||
        //(_zserviceOnkCache.IsNotNull() && _zserviceOnkCache.Any(p => p.Value.IsDirty)) ||
        //(ZmedicalEvent.IsNotNull() && ZmedicalEvent.IsDirty) || 
        //(KKsg?.IsDirty ?? false) || 
        //(ZSlkoef?.IsDirty ?? false) ||
        //(ZConsultations?.IsDirty ?? false)||
        //(ZDirection?.IsDirty ??false)||
        //(ZmedicalEventOnk.IsNotNull() && ZmedicalEventOnk.IsDirty);

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

            if (ZmedicalEvent.IsNotNull() && ZmedicalEvent.IsDirty)
            {
                var meventSaveResult = ZmedicalEvent.Save();
                if (meventSaveResult.HasError)
                {
                    return false;
                }
            }

            if (_zserviceCache.IsNotNull() && _zserviceCache.Any(p => p.Value.IsDirty))
            {
                foreach (var service in _zserviceCache.Where(p=>p.Value.IsDirty).Select(p=>p.Value))
                {
                    var serviceSaveResult = service.Save();
                    if (serviceSaveResult.HasError)
                    {

                        return false;
                    }
                }
            }

            if (_zserviceOnkCache.IsNotNull() && _zserviceOnkCache.Any(p => p.Value.IsDirty))
            {
                foreach (var service in _zserviceOnkCache.Where(p => p.Value.IsDirty).Select(p => p.Value))
                {
                    var serviceSaveResult = service.Save();
                    if (serviceSaveResult.HasError)
                    {

                        return false;
                    }
                }
            }

            if (KKsg.IsNotNull() && KKsg.IsDirty)
            {
                var kKsgResult = KKsg.Save();
                if (kKsgResult.HasError)
                {
                    return false;
                }
            }
            if (ZSlkoef.IsNotNull() && ZSlkoef.IsDirty)
            {
                var kKsgResult = ZSlkoef.Save();
                if (kKsgResult.HasError)
                {
                    return false;
                }
            }

            if (ZmedicalEventOnk.IsNotNull() && ZmedicalEventOnk.IsDirty)
            {
                var zmedicalEventOnkResult = ZmedicalEventOnk.Save();
                if (zmedicalEventOnkResult.HasError)
                {
                    return false;
                }
            }
            if(ZConsultations.IsNotNull() && ZConsultations.IsDirty)
            {
                var zconsultationsResult = ZConsultations.Save();
                if (zconsultationsResult.HasError)
                {
                    return false;
                }
            }
            if (ZDs.IsNotNull() && ZDs.IsDirty)
            {
                var zdsResult = ZDs.Save();
                if (zdsResult.HasError)
                {
                    return false;
                }
            }
            if (ZDirection.IsNotNull() && ZDirection.IsDirty)
            {
                var zdirectionResult = ZDirection.Save();
                if (zdirectionResult.HasError)
                {
                    return false;
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
            if (ZmedicalEvent.IsNotNull())
            {
                ZmedicalEvent.AcceptChanges();
            }
            if (KKsg.IsNotNull())
            {
                KKsg.AcceptChanges();
            }
            if (ZSlkoef.IsNotNull())
            {
                ZSlkoef.AcceptChanges();
            }
            if (ZmedicalEventOnk.IsNotNull())
            {
                ZmedicalEventOnk.AcceptChanges();
            }
            if (ZConsultations.IsNotNull())
            {
                ZConsultations.AcceptChanges();
            }
            if (ZDs.IsNotNull())
            {
                ZDs.AcceptChanges();
            }
            if (ZDirection.IsNotNull())
            {
                ZDirection.AcceptChanges();
            }
            if (_zserviceCache.IsNotNull())
            {
                foreach (var service in _zserviceCache.Where(p => p.Value.IsDirty).Select(p => p.Value))
                {
                    service.AcceptChanges();
                }
            }
            if (_zserviceOnkCache.IsNotNull())
            {
                foreach (var service in _zserviceOnkCache.Where(p => p.Value.IsDirty).Select(p => p.Value))
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
            var result = CommonGridControl<ZslEventShortView>._selectTerritoryEvent.Any()
                ? (_version != null && _version != 1 && _version != 2 && _version != 3
                    ? _repository.GetPatientsIdsBySelectZslid(
                        CommonGridControl<ZslEventShortView>._selectTerritoryEvent.Select(x => x.ZslMedicalEventId).ToList())
                    : _repository.GetPatientsIdsBySelectSlid(
                        CommonGridControl<EventExtendedView>._selectTerritoryEvent.Select(x => x.EventId).ToList()))
                : _repository.GetPatientsIdsByAccountId(_id);
                
            if (result.Success)
            {
                PatientsIds = result.Data;
                CurrentPatientPosition = 0;
            }
            InitInterTerritorialCommon();
            CommonGridControl<ZslEventShortView>._selectTerritoryEvent = new List<ZslEventShortView>();
        }

        private void InitInterTerritorialError()
        {
            var result = _repository.GetZPatientsIdsByAccountIdWithError(_id);
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

        private void InitZlocal()
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
            var result = _repository.GetZPatientsIdsByMedicalAccountIdWithError(_id);
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
                ZSlkoef = null;
                ZSlCrit = null;
                //ZDiagBlok = null;
                //ZContraindications = null;
                RaisePropertyChanged(() => ZSlkoef);
                RaisePropertyChanged(() => ZSlCrit);
                RaisePropertyChanged(() => ZmedicalEventOnk);

                Mevent = Di.I.Resolve<ZslMedicalEventContainer>(new NamedParameter("id", patientId),
                    new NamedParameter("changePosition", new Action<int>(i =>
                    {
                        ZmedicalEvent = Di.I.Resolve<ZMedicalEventContainer>(new NamedParameter("id", i),
                            new NamedParameter("changePosition", new Action<int>(ii =>
                            {
                                if (_zserviceCache.ContainsKey(ii))
                                {
                                    Mservice = _zserviceCache[ii];
                                    //if (Mservice.MedicalService.IsNotNull())
                                    //{
                                    //    MserviceOnk = _zserviceOnkCache[Mservice.MedicalService.ZmedicalServicesId];
                                    //    RaisePropertyChanged(() => MserviceOnk);
                                    //    ZDirection = Di.I.Resolve<ZDirectionContainer>(new NamedParameter("id", Mservice.MedicalService.ZmedicalServicesId));
                                    //    RaisePropertyChanged(() => ZDirection);
                                    //}
                                }
                                else
                                {
                                    Mservice = Di.I.Resolve<ZMedicalServiceContainer>(new NamedParameter("id", ii),
                                        new NamedParameter("changePosition", new Action<int>(mserId =>
                                    {
                                        //MserviceOnk = Di.I.Resolve<ZMedicalServiceOnkContainer>(new NamedParameter("id", mserId));
                                        //RaisePropertyChanged(() => MserviceOnk);
                                        //ZDirection = Di.I.Resolve<ZDirectionContainer>(new NamedParameter("id", mserId));
                                        //RaisePropertyChanged(() => ZDirection);
                                        //_zserviceOnkCache[mserId] = MserviceOnk;
                                    }
                                    )));
                                    _zserviceCache[ii] = Mservice;
                                }
                                RaisePropertyChanged(() => Mservice);
                                RaisePropertyChanged(() => NoServices);
                                RaisePropertyChanged(() => Services);
                                RaisePropertyChanged(() => Services1);
                                RaisePropertyChanged(() => NoServices1);
                                RaisePropertyChanged(() => Services1Onk);
                                RaisePropertyChanged(() => NoServices1Onk);


                                Refusal = Di.I.Resolve<ZRefusalContainer>(new NamedParameter("medicalEventId", ii),
                                    new NamedParameter("patientId", patientId),
                                    new NamedParameter("mode", (int) _mode));
                                RaisePropertyChanged(() => Refusal);

                                KKsg = Di.I.Resolve<ZKsgKpgContainer>(new NamedParameter("id", ii),
                                    new NamedParameter("changePosition", new Action<int>(koef =>
                                    {
                                        ZSlkoef = Di.I.Resolve<ZSlKoefContainer>(new NamedParameter("ksgkpgId", koef));
                                        RaisePropertyChanged(()=>ZSlkoef);
                                        ZSlCrit = Di.I.Resolve<ZSlCritContainer>(new NamedParameter("ksgkpgId", koef));
                                        RaisePropertyChanged(() => ZSlCrit);
                                    })));
                                RaisePropertyChanged(() => KKsg);

                                ZConsultations = Di.I.Resolve<ZMedicalConsultationsOnkContainer>(new NamedParameter("id", ii));
                                RaisePropertyChanged(() => ZConsultations);
                                ZDs = Di.I.Resolve<ZDsContainer>(new NamedParameter("medicalEventId", ii));
                                RaisePropertyChanged(() => ZDs);
                                ZDirection = Di.I.Resolve<ZDirectionContainer>(new NamedParameter("id", ii));
                                RaisePropertyChanged(() => ZDirection);

                                ZDiagBlok = null;
                                ZContraindications = null;
                                ZAnticancerDrug = null;
                                MserviceOnk = null;
                                RaisePropertyChanged(() => ZDiagBlok);
                                RaisePropertyChanged(() => ZContraindications);
                                RaisePropertyChanged(() => MserviceOnk);
                                RaisePropertyChanged(() => OneServ);
                                RaisePropertyChanged(() => Napr);
                                RaisePropertyChanged(() => NoNapr);
                                RaisePropertyChanged(() => Consultations);
                                RaisePropertyChanged(() => NoConsultations);
                                RaisePropertyChanged(() => ZAnticancerDrug);


                                ZmedicalEventOnk = Di.I.Resolve<ZMedicalEventOnkContainer>(new NamedParameter("id", ii),
                                     new NamedParameter("changePosition", new Action<int>(medicalEventOnkId =>
                                     {
                                         ZDiagBlok = Di.I.Resolve<ZDiagBlokContainer>(new NamedParameter("medicalEventOnkId", medicalEventOnkId));
                                         RaisePropertyChanged(() => ZDiagBlok);
                                         ZContraindications = Di.I.Resolve<ZContraindicationsContainer>(new NamedParameter("medicalEventOnkId", medicalEventOnkId));
                                         RaisePropertyChanged(() => ZContraindications);
                                         if (_zserviceOnkCache.ContainsKey(ii))
                                         {
                                             MserviceOnk = _zserviceOnkCache[ii];
                                             if (MserviceOnk.MedicalServiceOnk.IsNotNull())
                                             {
                                                 ZAnticancerDrug = Di.I.Resolve<ZAnticancerDrugContainer>(new NamedParameter("medicalServiceOnkId", MserviceOnk.MedicalServiceOnk.ZmedicalServicesOnkId));
                                                 RaisePropertyChanged(() => ZAnticancerDrug);
                                             }
                                         }
                                         else
                                         {
                                             MserviceOnk =
                                                 Di.I.Resolve<ZMedicalServiceOnkContainer>(new NamedParameter("id", medicalEventOnkId),
                                                     new NamedParameter("changePosition", new Action<int>(mserId =>
                                                     {
                                                         ZAnticancerDrug = Di.I.Resolve<ZAnticancerDrugContainer>(new NamedParameter("medicalServiceOnkId", mserId));
                                                         RaisePropertyChanged(() => ZAnticancerDrug);
                                                     }
                                                         )));
                                             _zserviceOnkCache[ii] = MserviceOnk;
                                         }
                                         RaisePropertyChanged(() => MserviceOnk);
                                         RaisePropertyChanged(() => NoServices);
                                         RaisePropertyChanged(() => Services);
                                         RaisePropertyChanged(() => OneServ);
                                         RaisePropertyChanged(() => Services1Onk);
                                         RaisePropertyChanged(() => NoServices1Onk);
                                     })));
                                RaisePropertyChanged(() => ZmedicalEventOnk);

                            })));
                        RaisePropertyChanged(() => ZmedicalEvent);
                    })));
                RaisePropertyChanged(() => Mevent);
               
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
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, model, new SrzResultView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке отправить запрос в СРЗ для пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(ZslOperatorViewModel));
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
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 600, model, new CreateSrzQueryView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке отправить запрос в СРЗ для пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(ZslOperatorViewModel));
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
                _messageService.ShowException(exception, "При попытке скопировать пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(ZslOperatorViewModel));
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
                    var model = scope.Resolve<SearchDiagnosisViewModel>(new NamedParameter("id", ZmedicalEvent.MedicalEvent.DiagnosisGeneral));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDiagnosisView());

                    model.OkCallback = i =>
                    {
                        ZmedicalEvent.MedicalEvent.DiagnosisGeneral = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске диагноза произошла ошибка.", typeof(ZslOperatorViewModel));
            }
        }

        public ICommand SelectKsgKpgCommand
        {
            get { return _selectKsgKpgCommand ?? (_selectKsgKpgCommand = new RelayCommand(SelectKsgKpg)); }
        }

        private void SelectKsgKpg()
        {
            try
            {
                //var model = new PropertyGridViewModel<EditZSlKoefViewModel>(new EditZSlKoefViewModel(new NamedParameter("ksgKpg", KKsg.KsgKpg),
                //       new NamedParameter("slkoef", ZSlkoef.MedicalSlkoef)));
                //var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                //model.OkCallback = () =>
                //{
                //    var tmp = Map.ObjectToObject<ZFactSank>(model.SelectedObject.Classifier);
                //    tmp.Date = DateTime.Now;
                //    tmp.EmployeeId = _userId;

                //    TransactionResult result;
                //    switch (_mode)
                //    {
                //        case OperatorMode.Zlocal:
                //        case OperatorMode.ZLocalError:
                //        case OperatorMode.LocalSrzQuery:
                //            result = _repository.InsertOrUpdateLocalSank(tmp, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                //            break;
                //        case OperatorMode.ZInterTerritorial:
                //        case OperatorMode.ZInterTerritorialError:
                //        case OperatorMode.InterTerritorialSrzQuery:
                //            result = _repository.InsertOrUpdateTerritorialSank(tmp, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
                //            break;
                //        default:
                //            //TODO unsupported mode
                //            return;
                //    }

                //    if (result.Success)
                //    {
                //        Mevent.ReloadCurrentMedicalEvent();
                //        Refusal.ReloadRefusals();
                //        view.Close();
                //    }
                //};


                using (var scope = Di.I.BeginLifetimeScope())
                {
                    if (KKsg.KsgKpg.IsNotNull()) KKsg.KsgKpg.AssistanceCondition = Mevent.ZslMedicalEvent.AssistanceConditions;
                    var model = scope.Resolve<SearchKsgKpgViewModel>(new NamedParameter("ksgKpg", KKsg.KsgKpg),
                        new NamedParameter("slkoef", ZSlkoef), new NamedParameter("crit", ZSlCrit));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 650, model, new SearchKsgKpgView());

                    model.OkCallback = ksgKpg =>
                    {

                        if (KKsg.IsNotNull() && KKsg.IsDirty)
                        {
                            var kKsgResult = KKsg.Save();
                            if (kKsgResult.HasError)
                            {
                                _messageService.ShowWarning("При сохранении КСГ произошла ошибка");
                                return;
                            }
                        }
                        if (ZSlkoef.IsNotNull() && ZSlkoef.IsDirty)
                        {
                            var kKsgResult = ZSlkoef.Save();
                            if (kKsgResult.HasError)
                            {
                                _messageService.ShowWarning("При сохранении коэффициента сложности лечения произошла ошибка");
                                return;
                            }
                        }
                        if (ZSlCrit.IsNotNull() && ZSlCrit.IsDirty)
                        {
                            var critResult = ZSlCrit.Save();
                            if (critResult.HasError)
                            {
                                _messageService.ShowWarning("При сохранении crit произошла ошибка");
                                return;
                            }
                        }

                        if (KKsg.IsNotNull())
                        {
                            KKsg.AcceptChanges();
                        }
                        if (ZSlkoef.IsNotNull())
                        {
                            ZSlkoef.AcceptChanges();
                        }
                        if (ZSlCrit.IsNotNull())
                        {
                            ZSlCrit.AcceptChanges();
                        }

                        // KKsg.KsgKpg = ksgKpg;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске КСГ/КПГ произошла ошибка.", typeof(ZslOperatorViewModel));
            }
        }


        public ICommand SelectHighTechAssistanceTypeCommand
        {
            get { return _selectHighTechAssistanceTypeCommand ?? (_selectHighTechAssistanceTypeCommand = new RelayCommand(SelectHighTechAssistanceType)); }
        }

        private void SelectHighTechAssistanceType()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    //var model = scope.Resolve<SearchDiagnosisViewModel>(new NamedParameter("id", ZmedicalEvent.MedicalEvent.DiagnosisGeneral));
                    //var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDiagnosisView());

                    //model.OkCallback = i =>
                    //{
                    //    ZmedicalEvent.MedicalEvent.DiagnosisGeneral = i;
                    //    view.Close();
                    //};
                    //model.CloseCallback = () => { view.Close(); };

                    //view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске вида вмп произошла ошибка.", typeof(ZslOperatorViewModel));
            }
        }

        public ICommand SelectHighTechAssistanceMethodCommand
        {
            get { return _selectHighTechAssistanceMethodCommand ?? (_selectHighTechAssistanceMethodCommand = new RelayCommand(SelectHighTechAssistanceMethod)); }
        }

        private void SelectHighTechAssistanceMethod()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    //var model = scope.Resolve<SearchDiagnosisViewModel>(new NamedParameter("id", ZmedicalEvent.MedicalEvent.DiagnosisGeneral));
                    //var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDiagnosisView());

                    //model.OkCallback = i =>
                    //{
                    //    ZmedicalEvent.MedicalEvent.DiagnosisGeneral = i;
                    //    view.Close();
                    //};
                    //model.CloseCallback = () => { view.Close(); };

                    //view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При поиске метода вмп произошла ошибка.", typeof(ZslOperatorViewModel));
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
                        case OperatorMode.Zlocal:
                        case OperatorMode.ZLocalError:
                        case OperatorMode.LocalSrzQuery:
                            _notifyManager.ShowNotify("Не реализовано.");
                            break;
                        case OperatorMode.ZInterTerritorial:
                        case OperatorMode.ZInterTerritorialError:
                        case OperatorMode.InterTerritorialSrzQuery:
                            var refusalsResult = _repository.GetF014();
                            if (refusalsResult.HasError)
                            {
                                _notifyManager.ShowNotify("Ошибка при получении списка отказов.");
                                return;
                            }
                            var refusals = refusalsResult.Data.Select(p => p.Id as int?).ToList();
                            var deleteTerritorialRefusal = _repository.DeletePatientTerritorialSank(_id, refusals, Patient.PatientId);
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
                        new NamedParameter("version", _version));
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
                _messageService.ShowException(exception, "При попытке перенести пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(ZslOperatorViewModel));
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
                    var model = scope.Resolve<CopyZPatientViewModel>(new NamedParameter("patientId", Patient.PatientId),
                        new NamedParameter("mode", Mode),
                        new NamedParameter("version", _version));
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
                _messageService.ShowException(exception, "При попытке скопировать пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(ZslOperatorViewModel));
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
                    var model = scope.Resolve<SearchPolicyViewModel>(
                        new NamedParameter("patientId", Patient.PatientId), 
                        new NamedParameter("medicalEventId", Mevent.ZslMedicalEvent.ZslMedicalEventId),
                        new NamedParameter("version", _version));
                    model.SearchCommand.Execute(null);
                    var view = new CommonWindowControl(Application.Current.MainWindow, 1100, 750, model, new SearchPolicyView());
                    
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
                _messageService.ShowException(exception, "При поиске документов ОМС пациента ID {0} произошла ошибка.".F(Patient.PatientId), typeof(ZslOperatorViewModel));
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
                _messageService.ShowException(exception, "При попытке поиска СМО произошла ошибка.", typeof(ZslOperatorViewModel));
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
                    var model = scope.Resolve<SearchMedicalOrganizationViewModel>(new NamedParameter("code", Mevent.ZslMedicalEvent.MedicalOrganizationCode));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchMedicalOrganizationView());

                    model.OkCallback = i =>
                    {
                        Mevent.ZslMedicalEvent.MedicalOrganizationCode = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(ZslOperatorViewModel));
            }
        }

        public ICommand SelectMeventDoctorCommand
        {
            get { return _selectMeventDoctorCommand ?? (_selectMeventDoctorCommand = new RelayCommand(SelectMeventDoctor)); }
        }

        /// <summary>
        /// Выбор доктора в случаи
        /// </summary>
        private void SelectMeventDoctor()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchDoctorViewModel>(new NamedParameter("code", ZmedicalEvent.MedicalEvent.DoctorId));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchDoctorView());

                    model.OkCallback = i =>
                    {
                        ZmedicalEvent.MedicalEvent.DoctorId = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(ZslOperatorViewModel));
            }
        }

        public ICommand SelectDirectionMoCommand
        {
            get { return _selectDirectionMoCommand ?? (_selectDirectionMoCommand = new RelayCommand(SelectDirectionMo)); }
        }

        private void SelectDirectionMo()
        {
            //TODO
            //_messageService.ShowWarning("В процессе разработки");
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchMedicalOrganizationViewModel>(new NamedParameter("code", ZDirection.MedicalDirection.DirectionMo));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchMedicalOrganizationView());

                    model.OkCallback = i =>
                    {
                        ZDirection.MedicalDirection.DirectionMo = i.ToString();
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска МО произошла ошибка.", typeof(ZslOperatorViewModel));
            }
        }

        public ICommand SelectMeventReferralOrganizationCommand
        {
            get { return _selectMeventReferralOrganizationCommand ?? (_selectMeventReferralOrganizationCommand = new RelayCommand(SelectMeventReferralOrganization)); }
        }

        private void SelectMeventReferralOrganization()
        {
            //TODO
            //_messageService.ShowWarning("В процессе разработки");
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<SearchReferralOrganizationViewModel>(new NamedParameter("code", Mevent.ZslMedicalEvent.ReferralOrganization));
                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new SearchMedicalOrganizationView());

                    model.OkCallback = i =>
                    {
                        Mevent.ZslMedicalEvent.ReferralOrganization = i;
                        view.Close();
                    };
                    model.CloseCallback = () => { view.Close(); };

                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(ZslOperatorViewModel));
            }
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
                _messageService.ShowException(exception, "При попытке поиска МО произошла ошибка.", typeof(ZslOperatorViewModel));
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
                _messageService.ShowException(exception, "При поиске диагноза произошла ошибка.", typeof(ZslOperatorViewModel));
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
                _messageService.ShowException(exception, "При попытке поиска врача произошла ошибка.", typeof(ZslOperatorViewModel));
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
            return Refusal.IsNotNull() && (Refusal.SelectedRefusal.IsNotNull() && Refusal?.SelectedRefusal?.Dest != RefusalDest.In);

        }

        private void DeleteRefusal()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить отказ ID {0}", "Внимание", Refusal.SelectedRefusal.ZSankId))
            {
                if (Refusal.SelectedRefusal.ZSankId > 0)
                {
                    switch (_mode)
                    {
                        case OperatorMode.Zlocal:
                        case OperatorMode.ZLocalError:
                        case OperatorMode.LocalSrzQuery:
                            var deleteRefusal = _repository.DeleteZLocalRefusal(_id, Mevent.ZslMedicalEvent.ZslMedicalEventId, Refusal.SelectedRefusal.ZSankId, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account);
                            if (deleteRefusal.Success)
                            {
                                ZmedicalEvent.ReloadCurrentMedicalEvent();
                                Mevent.ReloadCurrentMedicalEvent();
                                Refusal.ReloadRefusals();
                            }
                            break;
                        case OperatorMode.ZInterTerritorial:
                        case OperatorMode.ZInterTerritorialError:
                        case OperatorMode.InterTerritorialSrzQuery:
                        var deleteTerritorialRefusal = _repository.DeleteZTerritorialRefusal(_id, Mevent.ZslMedicalEvent.ZslMedicalEventId, Refusal.SelectedRefusal.ZSankId, (int)Refusal.SelectedRefusal.RefusalType, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account);
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

        //private void AddMec()
        //{
        //    AddMec(null);
        //}

        private void AddMec()
        {
            var source = _dataService.GetRefusalSource(_id, _mode);
            if (source == 0)
            {
                //TODO log error
                return;
            }
            var model = new PropertyGridViewModel<EditZMecViewModel>(new EditZMecViewModel(new ZFactSank
            {
                ZmedicalEventId = ZmedicalEvent.MedicalEvent.ZmedicalEventId,
                ZslMedicalEventId = Mevent.ZslMedicalEvent.ZslMedicalEventId,
                PatientId = Patient.PatientId,
                Date = DateTime.Now,
                NumAct = ZmedicalEvent.MedicalEvent.ZmedicalEventId.ToString(),
                Amount = ZmedicalEvent.MedicalEvent.EventPrice,
                ZslAmount = Mevent.ZslMedicalEvent.Price,
                Source = source,//reasonId == null ?  : (int)RefusalSource.LocalCorrected, //Если вызов метода произошел в момент подтверждения входящей санкции от ТФОМС, то мы ставим уточнеенные санкции к МО
                EmployeeId = _userId,
                SlidGuid = ZmedicalEvent.MedicalEvent.SlIdGuid,
                ExternalGuid = Guid.NewGuid().ToString()
            }));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactSank>(model.SelectedObject.Classifier);
                tmp.Type = 1;
                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Zlocal:
                    case OperatorMode.ZLocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalSank(tmp, UpdateFlag.MedicalEvent | UpdateFlag.ZslMedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
                        break;
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.ZInterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialSank(tmp, UpdateFlag.MedicalEvent | UpdateFlag.ZslMedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id); 
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

        public ICommand AddEqmaCommand
        {
            get
            {
                return _addEqmaCommand ?? (_addEqmaCommand = new RelayCommand(AddEqma));
            }
        }


        private void AddMee()
        {
            var model = new EditZMeeViewModel(new ZFactSank()) { Price = ZmedicalEvent.MedicalEvent.AcceptPrice };
            AddSank(model);
        }

        private void AddEqma()
        {
            var model = new EditZEqmaViewModel(new ZFactSank()) {Price = ZmedicalEvent.MedicalEvent.AcceptPrice};
            AddSank(model);
        }

        private void AddSank<T>(T models) where T : ClassifierBaseVm<ZFactSank>, new()
        {
            var source = _dataService.GetRefusalSource(_id, _mode);
            if (source == 0)
            {
                //TODO log error
                return;
            }
            string slIdGuid;
            switch (_mode)
            {
                case OperatorMode.Zlocal:
                case OperatorMode.ZLocalError:
                case OperatorMode.LocalSrzQuery:
                    slIdGuid = ZmedicalEvent.MedicalEvent.SlIdGuid;
                    break;
                case OperatorMode.ZInterTerritorial:
                case OperatorMode.ZInterTerritorialError:
                case OperatorMode.InterTerritorialSrzQuery:
                    slIdGuid = ZmedicalEvent.MedicalEvent.ExternalId;
                    break;
                default:
                    //TODO unsupported mode
                    return;
            }

            models.Classifier.ZmedicalEventId = ZmedicalEvent.MedicalEvent.ZmedicalEventId;
            models.Classifier.ZslMedicalEventId = Mevent.ZslMedicalEvent.ZslMedicalEventId;
            models.Classifier.PatientId = Patient.PatientId;
            models.Classifier.ZslAmount = Mevent.ZslMedicalEvent.Price;
            models.Classifier.Date = DateTime.Now;
            models.Classifier.Amount = ZmedicalEvent.MedicalEvent.AcceptPrice;
            models.Classifier.Source = source;
            models.Classifier.SlidGuid = slIdGuid;
            models.Classifier.EmployeeId = _userId;
            models.Classifier.ExternalGuid = Guid.NewGuid().ToString();

            var model = new PropertyGridViewModel<T>(models);
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactSank>(model.SelectedObject.Classifier);
                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Zlocal:
                    case OperatorMode.ZLocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalSank(tmp,
                            UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account,
                            Mevent.ZslMedicalEvent.ZslMedicalEventId, _id, (int) RefusalType.EQMA);
                        break;
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.ZInterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialSank(tmp,
                            UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account,
                            Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
                        break;
                    default:
                        //TODO unsupported mode
                        return;
                }

                if (result.Success)
                {
                    Mevent.ReloadCurrentMedicalEvent();
                    Refusal.ReloadRefusals();
                    _notifyManager.ShowNotify("Санкция успешно добавлена.");
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
            return Refusal.IsNotNull() && Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest != RefusalDest.In;

        }

        private void EditRefuse()
        {
            var sankResult = _repository.GetZsankById(Refusal.SelectedRefusal.ZSankId);
            if (sankResult.HasError)
            {
                //TODO error log
                return;
            }
            if (sankResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Санкция ID {0} не найдена", Refusal.SelectedRefusal?.ZSankId);
                return;
            }

            if (Refusal.SelectedRefusal?.RefusalType == (int)RefusalType.MEC)
            {
                EditZsank();
            }

            if (Constants.Mee.Contains(Refusal.SelectedRefusal?.RefusalType))
            {
                EditSank(new EditZMeeViewModel(sankResult.Data));
            }
            if (Constants.Eqma.Contains(Refusal.SelectedRefusal?.RefusalType))
            {
                EditSank(new EditZEqmaViewModel(sankResult.Data));
            }
        }

        private void EditZsank() 
        {

            var mecResult = _repository.GetZsankById(Refusal.SelectedRefusal.ZSankId);
            if (mecResult.HasError)
            {
                //TODO error log
                return;
            }
            if (mecResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Санкция ID {0} не найдена", Refusal.SelectedRefusal.ZSankId);
                return;
            }

            var model = new PropertyGridViewModel<EditZMecViewModel>(new EditZMecViewModel(mecResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactSank>(model.SelectedObject.Classifier);
                tmp.Date = DateTime.Now;
                tmp.EmployeeId = _userId;

                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Zlocal:
                    case OperatorMode.ZLocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalSank(tmp, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.ZInterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialSank(tmp, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
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

        private void EditSank<T>(T models) where T : ClassifierBaseVm<ZFactSank>, new()
        {
            var model = new PropertyGridViewModel<T>(models);
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactSank>(model.SelectedObject.Classifier);
                tmp.Date = DateTime.Now;
                tmp.EmployeeId = _userId;

                TransactionResult result;
                switch (_mode)
                {
                    case OperatorMode.Zlocal:
                    case OperatorMode.ZLocalError:
                    case OperatorMode.LocalSrzQuery:
                        result = _repository.InsertOrUpdateLocalSank(tmp, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, _id);
                        break;
                    case OperatorMode.ZInterTerritorial:
                    case OperatorMode.ZInterTerritorialError:
                    case OperatorMode.InterTerritorialSrzQuery:
                        result = _repository.InsertOrUpdateTerritorialSank(tmp, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
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
            return Refusal.IsNotNull() && (Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest != RefusalDest.In && Refusal.SelectedRefusal.RefusalType == (int)RefusalType.MEC);

        }

        private void BlockRefuse()
        {
            var mecResult = _repository.GetZsankById(Refusal.SelectedRefusal.ZSankId);
            if (mecResult.HasError)
            {
                //TODO error log
                return;
            }
            if (mecResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("МЭК ID {0} не найден", Refusal.SelectedRefusal.ZSankId);
                return;
            }

            TransactionResult blockResult;
            switch (_mode)
            {
                case OperatorMode.Zlocal:
                case OperatorMode.ZLocalError:
                case OperatorMode.LocalSrzQuery:
                    blockResult = _repository.BlockLocalSank(mecResult.Data, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
                    break;
                case OperatorMode.ZInterTerritorial:
                case OperatorMode.ZInterTerritorialError:
                case OperatorMode.InterTerritorialSrzQuery:
                    blockResult = _repository.BlockTerritorialSank(mecResult.Data, UpdateFlag.ZslMedicalEvent | UpdateFlag.MedicalEvent | UpdateFlag.Account, Mevent.ZslMedicalEvent.ZslMedicalEventId, _id);
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
            return Refusal.IsNotNull() && (Refusal.SelectedRefusal.IsNotNull() && Refusal.SelectedRefusal.Dest == RefusalDest.In);

        }

        private void AgreeRefuse(object arg)
        {
            var externalRefuseResult = _repository.GetZExternalRefuseById(Refusal.SelectedRefusal.ZSankId);
            if (externalRefuseResult.HasError)
            {
                //TODO error log
                return;
            }
            if (externalRefuseResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Внешний отказ ID {0} не найден", Refusal.SelectedRefusal.ZSankId);
                return;
            }

            TransactionResult updateResult;
            switch (_mode)
            {
                case OperatorMode.ZInterTerritorial:
                case OperatorMode.ZInterTerritorialError:
                    var flag = arg.ToInt32();
                    externalRefuseResult.Data.IsAgree = flag == 1; 
                    updateResult = _repository.InsertOrUpdateZExternalRefuse(externalRefuseResult.Data, UpdateFlag.MedicalEvent | UpdateFlag.ZslMedicalEvent | UpdateFlag.Account,
                        Mevent.ZslMedicalEvent.ZslMedicalEventId, _id );
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
                    //AddMec(externalRefuseResult.Data.ReasonId);
                }
            }
        }
        #endregion


        public ICommand AddDsCommand
        {
            get
            {
                return _addDsCommand ?? (_addDsCommand = new RelayCommand(AddDs));
            }
        }

        public ICommand EditDsCommand
        {
            get
            {
                return _editDsCommand ?? (_editDsCommand = new RelayCommand(EditDs, CanDs));
            }
        }

        private void AddDs()
        {
           
                var model =
                    new PropertyGridViewModel<EditZDsViewModel>(new EditZDsViewModel(new ZFactDs
                    {
                        ZmedicalEventId = ZmedicalEvent.MedicalEvent.ZmedicalEventId
                    }));
                var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<ZFactDs>(model.SelectedObject.Classifier);
                    TransactionResult result = _repository.InsertOrUpdateZDs(tmp);

                    if (result.Success)
                    {
                        ZDs.ReloadDiagBlok();
                        _notifyManager.ShowNotify("Запись успешно добавлена.");
                        view.Close();
                    }

                };
                view.ShowDialog();
        }

        private void EditDs()
        {
            var dsResult = _repository.GetZDsById(ZDs.SelectedDs.ZFactDsId);
            if (dsResult.HasError)
            {
                //TODO error log
                return;
            }
            if (dsResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Диагноз ID {0} не найден", ZDs.SelectedDs.ZFactDsId);
                return;
            }

            var model = new PropertyGridViewModel<EditZDsViewModel>(new EditZDsViewModel(dsResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactDs>(model.SelectedObject.Classifier);
                TransactionResult result = _repository.InsertOrUpdateZDs(tmp);
                if (result.Success)
                {
                    ZDs.ReloadDiagBlok();
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        private bool CanDs()
        {
            return ZDs.IsNotNull() && ZDs.SelectedDs.IsNotNull();
        }

        public ICommand DeleteDsCommand
        {
            get
            {
                return _deleteDsCommand ??
                       (_deleteDsCommand = new RelayCommand(DeleteDs, CanDeleteDs));
            }
        }

        private void DeleteDs()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить диагноз ID {0}",
                "Внимание", ZDs.SelectedDs.ZFactDsId))
            {
                if (ZDs.SelectedDs.ZFactDsId > 0)
                {
                    var deleteRefusal = _repository.DeleteZds(ZDs.SelectedDs.ZFactDsId);
                    if (deleteRefusal.Success)
                    {
                        ZDs.ReloadDiagBlok();
                    }
                }
            }
        }

        private bool CanDeleteDs()
        {
            return ZDs.IsNotNull() && ZDs.SelectedDs.IsNotNull();

        }

        #region DiagBlok
        public ICommand AddDiagBlokCommand
        {
            get
            {
                return _addDiagBlokCommand ?? (_addDiagBlokCommand = new RelayCommand(AddDiagBlok));
            }
        }

        private void AddDiagBlok()
        {
            if (ZmedicalEventOnk.MedicalEventOnk.IsNotNull())
            {
                var model =
                    new PropertyGridViewModel<EditZDiagBlokViewModel>(new EditZDiagBlokViewModel(new ZFactDiagBlok
                    {
                        ZMedicalEventOnkId = ZmedicalEventOnk.MedicalEventOnk.ZMedicalEventOnkId
                    }));
                var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<ZFactDiagBlok>(model.SelectedObject.Classifier);
                    TransactionResult result = _repository.InsertOrUpdateZDiagBlok(tmp);

                    if (result.Success)
                    {
                        ZDiagBlok.ReloadDiagBlok();
                        _notifyManager.ShowNotify("Запись успешно добавлена.");
                        view.Close();
                    }
                };

                view.ShowDialog();
            }
            else
            { _notifyManager.ShowNotify("ОНКО случай отсутствует. "); }
        }

        public ICommand EditDiagBlokCommand
        {
            get
            {
                return _editDiagBlokCommand ?? (_editDiagBlokCommand = new RelayCommand(EditDiagBlok, CanDiagBlokRefuse));
            }
        }

        private bool CanDiagBlokRefuse()
        {
            return ZDiagBlok.IsNotNull() && ZDiagBlok.SelectedDiagBlok.IsNotNull();
        }

        private void EditDiagBlok()
        {
            var diagBlokResult = _repository.GetZDiagBlokById(ZDiagBlok.SelectedDiagBlok.ZDiagBlokId);
            if (diagBlokResult.HasError)
            {
                //TODO error log
                return;
            }
            if (diagBlokResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Диагностический блок ID {0} не найден", ZDiagBlok.SelectedDiagBlok.ZDiagBlokId);
                return;
            }

            var model = new PropertyGridViewModel<EditZDiagBlokViewModel>(new EditZDiagBlokViewModel(diagBlokResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactDiagBlok>(model.SelectedObject.Classifier);
                TransactionResult result = _repository.InsertOrUpdateZDiagBlok(tmp);
                if (result.Success)
                {
                    ZDiagBlok.ReloadDiagBlok();
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        public ICommand DeleteDiagBlokCommand
        {
            get
            {
                return _deleteDiagBlokCommand ??
                       (_deleteDiagBlokCommand = new RelayCommand(DeleteDiagBlok, CanDeleteDiagBlok));
            }
        }

        private bool CanDeleteDiagBlok()
        {
            return ZDiagBlok.IsNotNull() && ZDiagBlok.SelectedDiagBlok.IsNotNull();

        }

        private void DeleteDiagBlok()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить диагностический блок ID {0}",
                "Внимание", ZDiagBlok.SelectedDiagBlok.ZDiagBlokId))
            {
                if (ZDiagBlok.SelectedDiagBlok.ZDiagBlokId > 0)
                {
                    var deleteRefusal = _repository.DeleteZdiagBlok(ZDiagBlok.SelectedDiagBlok.ZDiagBlokId);
                    if (deleteRefusal.Success)
                    {
                        ZDiagBlok.ReloadDiagBlok();
                    }

                }

            }
        }

        #endregion
        
        #region Сontraindications


        public ICommand AddContraindicationsCommand
        {
            get
            {
                return _addContraindicationsCommand ?? (_addContraindicationsCommand = new RelayCommand(AddContraindications));
            }
        }

        private void AddContraindications()
        {
            if (ZmedicalEventOnk.MedicalEventOnk != null)
            {
                var model =
                    new PropertyGridViewModel<EditZContraindicationskViewModel>(
                        new EditZContraindicationskViewModel(new ZFactContraindications
                        {
                            ZMedicalEventOnkId = ZmedicalEventOnk.MedicalEventOnk.ZMedicalEventOnkId
                        }));
                var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<ZFactContraindications>(model.SelectedObject.Classifier);
                    TransactionResult result = _repository.InsertOrUpdateZContraindications(tmp);

                    if (result.Success)
                    {
                        ZContraindications.ReloadDiagBlok();
                        _notifyManager.ShowNotify("Запись успешно добавлена.");
                        view.Close();
                    }

                };

                view.ShowDialog();
            }
            else
            { _notifyManager.ShowNotify("ОНКО случай отсутствует. "); }
        }

        public ICommand EditContraindicationsCommand
        {
            get
            {
                return _editContraindicationsCommand ?? (_editContraindicationsCommand = new RelayCommand(EditContraindications, CanContraindications));
            }
        }

        private bool CanContraindications()
        {
            return ZContraindications.IsNotNull() && ZContraindications.SelectedСontraindications.IsNotNull();
        }

        private void EditContraindications()
        {
            var contraindicationsResult = _repository.GetZContraindicationsById(ZContraindications.SelectedСontraindications.ZContraindicationsId);
            if (contraindicationsResult.HasError)
            {
                //TODO error log
                return;
            }
            if (contraindicationsResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Сведения об отказак и прот. ID {0} не найден", ZContraindications.SelectedСontraindications.ZContraindicationsId);
                return;
            }

            var model = new PropertyGridViewModel<EditZContraindicationskViewModel>(new EditZContraindicationskViewModel(contraindicationsResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactContraindications>(model.SelectedObject.Classifier);
                TransactionResult result = _repository.InsertOrUpdateZContraindications(tmp);
                if (result.Success)
                {
                    ZContraindications.ReloadDiagBlok();
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        public ICommand DeleteContraindicationsCommand
        {
            get
            {
                return _deleteContraindicationsCommand ??
                       (_deleteContraindicationsCommand = new RelayCommand(DeleteContraindications, CanDeleteContraindications));
            }
        }

        private bool CanDeleteContraindications()
        {
            return ZContraindications.IsNotNull() && ZContraindications.SelectedСontraindications.IsNotNull();

        }

        private void DeleteContraindications()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить сведения об отказах и прот. ID {0}",
                "Внимание", ZContraindications.SelectedСontraindications.ZContraindicationsId))
            {
                if (ZContraindications.SelectedСontraindications.ZContraindicationsId > 0)
                {
                    var deleteContraindications = _repository.DeleteZContraindications(ZContraindications.SelectedСontraindications.ZContraindicationsId);
                    if (deleteContraindications.Success)
                    {
                        ZContraindications.ReloadDiagBlok();
                    }

                }

            }
        }

        #endregion

        #region AnticancerDrug
        public ICommand AddAnticancerDrugCommand
        {
            get
            {
                return _addAnticancerDrugCommand ?? (_addAnticancerDrugCommand = new RelayCommand(AnticancerDrugBlok));
            }
        }

        private void AnticancerDrugBlok()
        {
            if (ZAnticancerDrug.AnticancerDrug.IsNotNull())
            {
                var model =
                    new PropertyGridViewModel<EditZAnticancerDrugViewModel>(new EditZAnticancerDrugViewModel(new ZFactAnticancerDrug
                    {
                        ZMedicalServiceOnkId = MserviceOnk.MedicalServiceOnk.ZmedicalServicesOnkId
                    }));
                var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<ZFactAnticancerDrug>(model.SelectedObject.Classifier);
                    TransactionResult result = _repository.InsertOrUpdateZAnticancerDrug(tmp);

                    if (result.Success)
                    {
                        ZAnticancerDrug.ReloadDiagBlok();
                        _notifyManager.ShowNotify("Запись успешно добавлена.");
                        view.Close();
                    }

                };

                view.ShowDialog();
            }
            else
            { _notifyManager.ShowNotify("ОНКО услука отсутствует. "); }
        }

        public ICommand EditAnticancerDrugCommand
        {
            get
            {
                return _editAnticancerDrugCommand ?? (_editAnticancerDrugCommand = new RelayCommand(EditAnticancerDrug, CanAnticancerDrug));
            }
        }

        private bool CanAnticancerDrug()
        {
            return ZAnticancerDrug.IsNotNull() && ZAnticancerDrug.SelectedAnticancerDrug.IsNotNull();
        }

        private void EditAnticancerDrug()
        {
            var anticancerDrugResult = _repository.GetZAnticancerDrugById(ZAnticancerDrug.SelectedAnticancerDrug.ZAnticancerDrugId);
            if (anticancerDrugResult.HasError)
            {
                //TODO error log
                return;
            }
            if (anticancerDrugResult.Data.IsNull())
            {
                _messageService.ShowWarningFormatted("Сведения о введенном противоопухолевом лекарственном препарате ID {0} не найден", ZAnticancerDrug.SelectedAnticancerDrug.ZAnticancerDrugId);
                return;
            }

            var model = new PropertyGridViewModel<EditZAnticancerDrugViewModel>(new EditZAnticancerDrugViewModel(anticancerDrugResult.Data));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<ZFactAnticancerDrug>(model.SelectedObject.Classifier);
                TransactionResult result = _repository.InsertOrUpdateZAnticancerDrug(tmp);
                if (result.Success)
                {
                    ZAnticancerDrug.ReloadDiagBlok();
                    view.Close();
                }
            };

            view.ShowDialog();
        }

        public ICommand DeleteAnticancerDrugCommand
        {
            get
            {
                return _deleteAnticancerDrugCommand ??
                       (_deleteAnticancerDrugCommand = new RelayCommand(DeleteAnticancerDrug, CanAnticancerDrugBlok));
            }
        }

        private bool CanAnticancerDrugBlok()
        {
            return ZAnticancerDrug.IsNotNull() && ZAnticancerDrug.SelectedAnticancerDrug.IsNotNull();

        }

        private void DeleteAnticancerDrug()
        {
            if (_messageService.AskQuestionFormatted("Вы действительно хотите удалить сведения о введенном противоопухолевом лекарственном препарате ID {0}",
                "Внимание", ZAnticancerDrug.SelectedAnticancerDrug.ZAnticancerDrugId))
            {
                if (ZAnticancerDrug.SelectedAnticancerDrug.ZAnticancerDrugId > 0)
                {
                    var deleteRefusal = _repository.DeleteZAnticancerDrug(ZAnticancerDrug.SelectedAnticancerDrug.ZAnticancerDrugId);
                    if (deleteRefusal.Success)
                    {
                        ZAnticancerDrug.ReloadDiagBlok();
                    }

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
