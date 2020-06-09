using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Core;
using Core.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.PolicySearch;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Processing.ViewModels
{
    public class CheckPolicyViewModel : ViewModelBase
    {
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IProcessingService _processingService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly int _version;

        private ObservableCollection<PolicyCheckPatientModel> _patientList;
        public ObservableCollection<PolicyCheckPatientModel> PatientList
	    {
            get { return _patientList ?? (_patientList = new ObservableCollection<PolicyCheckPatientModel>()); }
	        set
	        {
	            _patientList = value;
                RaisePropertyChanged(() => PatientList);
	        }
	    }

        private ObservableCollection<object> _detailsCollection;
        public ObservableCollection<object> DetailsCollection
        {
            get { return _detailsCollection ?? (_detailsCollection = new ObservableCollection<object>()); }
            set
            {
                _detailsCollection = value; RaisePropertyChanged(()=>DetailsCollection);
            }
        }

        private object _currentRow;
        public object CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);

                if (CurrentRow.IsNotNull())
                {
                    if (CurrentRow is PolicyCheckPatientModel)
                    {
                        var policyModel = CurrentRow as PolicyCheckPatientModel;
                        if (policyModel.IsNotNull() && policyModel.PatientId.HasValue)
                        {
                            if (Constants.ZterritoryVersion.Contains(_version))
                            {
                                var eventResult = _repository.GetZslEventShortViewByPatientId(policyModel.PatientId.Value);
                                if (eventResult.Success)
                                {
                                    DetailsCollection = new ObservableCollection<object>(eventResult.Data);
                                }
                            }
                            else
                            {
                                var eventResult = _repository.GetEventShortViewByPatientId(policyModel.PatientId.Value);
                                if (eventResult.Success)
                                {
                                    DetailsCollection = new ObservableCollection<object>(eventResult.Data);
                                }
                            }
                            
                        }
                        
                    }
                    if (CurrentRow is PolicyCheckMedicalEventModel)
                    {
                        var policyModel = CurrentRow as PolicyCheckMedicalEventModel;
                        if (policyModel.IsNotNull() && policyModel.MedicalEventId.HasValue)
                        {
                            if (Constants.ZterritoryVersion.Contains(_version))
                            {
                                var eventResult =
                                    _repository.GetZslEventShortViewByMedicalEventId(policyModel.MedicalEventId.Value);
                                if (eventResult.Success)
                                {
                                    DetailsCollection = new ObservableCollection<object>(eventResult.Data);
                                }
                            }
                            else
                            {
                                var eventResult =
                                    _repository.GetEventShortViewByMedicalEventId(policyModel.MedicalEventId.Value);
                                if (eventResult.Success)
                                {
                                    DetailsCollection = new ObservableCollection<object>(eventResult.Data);
                                }
                            }
                        }

                    }
                }
            }
        }

        private readonly int _id;
        private readonly int _userId;

        private RelayCommand _checkCommand;
        private RelayCommand _applyCommand;
        private RelayCommand _selectAllCommand;
        private RelayCommand _unselectAllCommand;

        public CheckPolicyViewModel(IUserService userService, 
            IMedicineRepository repository, 
            IProcessingService processingService, 
            INotifyManager notifyManager, 
            IDockLayoutManager dockLayoutManager, 
            int id,
            int version)
        {
            _id = id;
            _userId = userService.UserId;
            _repository = repository;
            _notifyManager = notifyManager;
            _processingService = processingService;
	        _dockLayoutManager = dockLayoutManager;
            _version = version;
            Init();
        }

        private void Init()
        {
        }

        public ICommand CheckCommand
        {
            get { return _checkCommand ?? (_checkCommand = new RelayCommand(Check)); }
        }

        private void Check()
        {
            PatientList = null;

            _dockLayoutManager.ShowOverlay(Constants.RunCheckDataTitleMsg, Constants.PleaseWaitMsg);

            Task.Factory.StartNew(() => 
                {
                    var checkResult = (Constants.ZterritoryVersion.Contains(_version))
                        ? _processingService.CheckZAccountPolicy(_id)
                        : _processingService.CheckAccountPolicy(_id);
                    if (checkResult.Success)
                    {
                        PatientList = new ObservableCollection<PolicyCheckPatientModel>(checkResult.Data);
                        if(checkResult.Data.Any())
                            _notifyManager.ShowNotify("Проверка успешно выполнена.");
                        else
                        {
                            _notifyManager.ShowNotify("Проверка успешно выполнена. Ошибок не обнаружено.");
                        }
                    } 
                    else
                    {
                        _notifyManager.ShowNotify($"Ошибка выполнения проверки. {checkResult.LastError}");
                    }
                }).ContinueWith(p => _dockLayoutManager.HideOverlay());
	    }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply, CanApply)); }
        }

        private bool CanApply()
        {
            return PatientList.Count > 0;
        }

        private void Apply()
        {
            _dockLayoutManager.ShowOverlay(Constants.ApplyCheckDataResultTitleMsg, Constants.PleaseWaitMsg);
            Task.Factory.StartNew(() =>
            {
                var applyResult = _processingService.ApplyPolicy(_id, PatientList.Where(p=>p.IsApply), Constants.ScopeInterTerritorialAccount);
                if (applyResult.Success)
                {
                    _notifyManager.ShowNotify("МЭК успешно добавлен в базу");
                }
                else
                {
                    _notifyManager.ShowNotify("Ошибка применения результатов экспертиз. Смотрите журнал ошибок.");
                }
            }).ContinueWith(p => _dockLayoutManager.HideOverlay());
        }

        public ICommand SelectAllCommand
        {
            get { return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(SelectAll, CanSelectAll)); }
        }

	    private bool CanSelectAll()
	    {
            return PatientList.Any(p => !p.IsApply);
	    }

	    private void SelectAll()
	    {
            PatientList.ForEachAction(p => p.IsApply = true);
	    }

        public ICommand UnselectAllCommand
        {
            get { return _unselectAllCommand ?? (_unselectAllCommand = new RelayCommand(UnselectAll, CanUnselectAll)); }
        }

        private bool CanUnselectAll()
        {
            return PatientList.Any(p => p.IsApply);
        }

        private void UnselectAll()
        {
            PatientList.ForEachAction(p => { p.IsApply = false; });
        }
    }
}
