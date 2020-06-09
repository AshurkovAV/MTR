using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    public class CheckPolicyInTortillaViewModel : ViewModelBase
    {
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IProcessingService _processingService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly ITextService _textService;
        private readonly int _version;

        private ObservableCollection<PolicyCheckTortillaResultModel> _patientList;
        public ObservableCollection<PolicyCheckTortillaResultModel> PatientList
	    {
            get { return _patientList ?? (_patientList = new ObservableCollection<PolicyCheckTortillaResultModel>()); }
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
                    if (CurrentRow is PolicyCheckTortillaResultModel)
                    {
                        var policyModel = CurrentRow as PolicyCheckTortillaResultModel;
                        if (policyModel.IsNotNull() && policyModel.PatientId.HasValue)
                        {
                            var eventResult = _repository.GetEventShortViewByPatientId(policyModel.PatientId.Value);
                            if (eventResult.Success)
                            {
                                DetailsCollection = new ObservableCollection<object>(eventResult.Data);
                            }
                        }
                        
                    }
                }
            }
        }

        private readonly int _id;
        private readonly int _userId;

        private string _notes;

        private RelayCommand _checkCommand;
        private RelayCommand _applyCommand;
        private RelayCommand _selectAllCommand;
        private RelayCommand _unselectAllCommand;

        public CheckPolicyInTortillaViewModel(IUserService userService, 
            IMedicineRepository repository, 
            IProcessingService processingService, 
            INotifyManager notifyManager, 
            IDockLayoutManager dockLayoutManager,
            ITextService textService,
            int id,
            int version)
        {
            _id = id;
            _userId = userService.UserId;
            _repository = repository;
            _notifyManager = notifyManager;
            _processingService = processingService;
	        _dockLayoutManager = dockLayoutManager;
            _textService = textService;
            _version = version;
            PatientList = null;
            Init();
        }

        private void Init()
        {
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged(()=>Notes); }
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
	            var checkResult = Constants.ZmedicalVersion.Contains(_version) ? _processingService.CheckZPolicyInTortilla(_id, true): _processingService.CheckPolicyInTortilla(_id, true);
                if (checkResult.Success && checkResult.Data.Any())
                {
                    Notes = checkResult.Log;
                    PatientList = new ObservableCollection<PolicyCheckTortillaResultModel>(checkResult.Data);
                    _notifyManager.ShowNotify("Проверка выполнена успешна.");
                }
                else if (checkResult.HasError)
                {
                    Notes = checkResult.LastError?.Message;
                    _notifyManager.ShowNotify("Ошибка проверки в тортила. Смотрите журнал ошибок.");
                }
                else if (checkResult.Success && !checkResult.Data.Any())
                {
                    _notifyManager.ShowNotify("Данные успешно проверены. Данных в тортиле для проверки нет.");
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
            //TODO Apply policy check
            Console.WriteLine(_userId);
            var log = new StringBuilder();
            _dockLayoutManager.ShowOverlay(Constants.ApplyCheckDataResultTitleMsg, Constants.PleaseWaitMsg);
            Task.Factory.StartNew(() =>
            {
                var applyResult = _processingService.ApplyPolicyFromTortilla(PatientList.Where(p=>p.IsApply));
                if (applyResult.Success)
                {
                    _notifyManager.ShowNotify("Изменено {0} полисов".F(PatientList.Count(p => p.IsApply)));
                    foreach (var policy in PatientList.Where(p => p.IsApply))
                    {
                        log.AppendFormat("ID пациента {0}, ЕНП {1}, ОКАТО территории страхования {2}, комментарии {3}\r\n".F(
                            policy.PatientId,
                            policy.INP,
                            policy.TerritoryOkato,
                            policy.Comments));
                    }
                    Notes = log.ToString();
                }
                else
                {
                    _notifyManager.ShowNotify("Ошибка изменения данных полисов : {0}".F(applyResult.LastError));
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
