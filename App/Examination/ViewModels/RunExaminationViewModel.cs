using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Core;
using Core.Validation;
using DataModel;
using DevExpress.XtraPrinting.Native;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Validation.Attribute;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Examination.ViewModels
{
	/// <summary>
    /// 
	/// </summary>
    public class RunExaminationViewModel : ViewModelBase, IDataErrorInfo
	{
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IExaminationService _examinationService;
        private readonly IDockLayoutManager _dockLayoutManager;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        public ObservableCollection<FactExpertCriterion> AllExamsList { get; set; }
        [CustomValidation(typeof(LengthValidation), "ValidateCollection")]
        public List<object> SelectedExamsList { get; set; }

        private ObservableCollection<ExamResultModel> _errorList;

	    public ObservableCollection<ExamResultModel> ErrorList
	    {
            get { return _errorList ?? (_errorList = new ObservableCollection<ExamResultModel>()); }
	        set
	        {
	            _errorList = value;
	            RaisePropertyChanged(() => ErrorList);
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

	    private ExamResultModel _currentRow;
        public ExamResultModel CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
                
                switch (CurrentRow.ErrorScope)
                {
                    //счет МО
                    case 1:
                        var medicalAccountResult = _repository.GetMedicalAccountView(p=> p.MedicalAccountId == CurrentRow.Id);
                        if (medicalAccountResult.Success)
                        {
                            DetailsCollection = new ObservableCollection<object>(medicalAccountResult.Data);
                        }
                        break;
                    //счет
                    case 2:
                        var accountResult = _repository.GetFactTerritoryAccount(p => p.TerritoryAccountId == CurrentRow.Id);
                        if (accountResult.Success)
                        {
                            DetailsCollection = new ObservableCollection<object>(accountResult.Data);
                        }
                        break;
                    //пациент
                    case 3:
                        var patientResult = _repository.GetPatientShortView(p => p.PatientId == CurrentRow.Id);
                        if (patientResult.Success)
                        {
                            DetailsCollection = new ObservableCollection<object>(patientResult.Data);
                        }
                        break;
                    //случай
                    case 4:
                        var eventResult = _repository.GetEventShortView(p=>p.EventId == CurrentRow.Id);
                        if (eventResult.Success)
                        {
                            DetailsCollection = new ObservableCollection<object>(eventResult.Data);
                        }
                        break;
                    //услуга
                    case 5:
                        DetailsCollection = new ObservableCollection<object>
                        {
                            new 
                            {
                                Результат = "Not implement yet"
                            }
                        };
                        break;
                    //неопределено
                    default:
                        DetailsCollection = new ObservableCollection<object>
                        {
                            new 
                            {
                                Результат = "Ошибка, область ошибок не определена"
                            }
                        };
                        break;
                }
                
                
            }
        }

        private readonly int _id;
        private readonly int _scope;
	    private readonly int _version;

        private RelayCommand _selectCommand;
        private RelayCommand _applyCommand;
        private RelayCommand _selectAllCommand;
        private RelayCommand _unselectAllCommand;
	    

	    public RunExaminationViewModel(IMedicineRepository repository, IExaminationService examinationService, INotifyManager notifyManager, IDockLayoutManager dockLayoutManager, int scope, int id, int version)
        {
            _id = id;
            _scope = scope;
            //Для старых счетов версия 1.0
            _version = Math.Max(version, 1);
    
            _repository = repository;
            _notifyManager = notifyManager;
	        _examinationService = examinationService;
	        _dockLayoutManager = dockLayoutManager;
            Init();

        }

        private void Init()
        {
            var examsResult = _repository.GetEnabledExpertCriterionByScopeAndVersion(_scope, _version);
            if (examsResult.Success)
            {
                AllExamsList = new ObservableCollection<FactExpertCriterion>(examsResult.Data);
            }

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
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

        public ICommand SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand(DoSelect, CanDoSelect)); }
        }

	    private bool CanDoSelect()
	    {
            return _dataErrorInfoSupport.Error.Length == 0;
	    }

	    private void DoSelect()
	    {
            _dockLayoutManager.ShowOverlay(Constants.RunCheckDataTitleMsg, Constants.PleaseWaitMsg);
	        Task.Factory.StartNew(() =>
	        {
	            var examsResult = _examinationService.RunExams(_id, _scope, _version, SelectedExamsList);
	            if (examsResult.Success)
	            {
	                ErrorList = new ObservableCollection<ExamResultModel>(examsResult.Data);
	                _notifyManager.ShowNotify("Экспертизы успешно выполнены.");
	            }
	        }).ContinueWith(p => _dockLayoutManager.HideOverlay());
	    }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply, CanApply)); }
        }

        private bool CanApply()
        {
            return ErrorList.Count > 0;
        }

        private void Apply()
        {
            _dockLayoutManager.ShowOverlay(Constants.ApplyCheckDataResultTitleMsg, Constants.PleaseWaitMsg);
            Task.Factory.StartNew(() =>
            {
                var applyExams = Constants.Zversion.Contains(_version) ? _examinationService.ApplyZslExams(_id, _scope, ErrorList.ToList()):_examinationService.ApplyExams( _id, _scope, ErrorList.ToList());
                if (applyExams.Success)
                {
                    _notifyManager.ShowNotify("Результаты экспертизы успешно применены.");
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
	        return ErrorList.Any(p => !p.IsApply);
	    }

	    private void SelectAll()
	    {
	        ErrorList.ForEach(p=>p.IsApply=true);
	    }

        public ICommand UnselectAllCommand
        {
            get { return _unselectAllCommand ?? (_unselectAllCommand = new RelayCommand(UnselectAll, CanUnselectAll)); }
        }

        private bool CanUnselectAll()
        {
            return ErrorList.Any(p => p.IsApply);
        }

        private void UnselectAll()
        {
            ErrorList.ForEach(p => { p.IsApply = false;});
        }
	}
}
