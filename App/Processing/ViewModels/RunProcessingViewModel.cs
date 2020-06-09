using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using Core;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.CoreLayer.Validation.Attribute;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Processing.ViewModels
{
	/// <summary>
    /// Description of SelectReportViewModel.
	/// </summary>
    public class RunProcessingViewModel : ViewModelBase, IDataErrorInfo
	{
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly IProcessingService _processingService;
        private readonly IMessageService _messageService;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        public ObservableCollection<FactProcessing> AllProcessingList { get; set; }
        [CustomValidation(typeof(LengthValidation), "ValidateCollection")]
        public List<object> SelectedProcessingList { get; set; }

        private ObservableCollection<ProcessingResultModel> _resultList;

	    public ObservableCollection<ProcessingResultModel> ResultList
	    {
            get { return _resultList ?? (_resultList = new ObservableCollection<ProcessingResultModel>()); }
	        set
	        {
	            _resultList = value;
	            RaisePropertyChanged(() => ResultList);
	        }
	    }

	    private ProcessingResultModel _currentRow;
        public ProcessingResultModel CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        private readonly int _id;
        private readonly int _scope;
        private readonly int _userId;
	    private readonly int _version;

        private RelayCommand _selectCommand;

	    public RunProcessingViewModel(IUserService userService, 
            IMedicineRepository repository, 
            INotifyManager notifyManager, 
            IDockLayoutManager dockLayoutManager, 
            IProcessingService processingService,
            IMessageService messageService,
            int scope, int id, int version)
        {
            _id = id;
            _scope = scope;
            //Для старых счетов версия 1.0
            _version = Math.Max(version, 1);
            _userId = userService.UserId;
            _repository = repository;
            _notifyManager = notifyManager;
	        _dockLayoutManager = dockLayoutManager;
            _processingService = processingService;
	        _messageService = messageService;
            Init();
        }

        private void Init()
        {
            var processingResult = _repository.GetEnabledProcessingByScopeAndVersion(_scope, _version);
            if (processingResult.Success)
            {
                AllProcessingList = new ObservableCollection<FactProcessing>(processingResult.Data);
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
                var processingResult = _processingService.RunProcessing(_id, _scope, _version, SelectedProcessingList);
	            if (processingResult.Success)
	            {
	                ResultList = new ObservableCollection<ProcessingResultModel>(processingResult.Data);
	                _notifyManager.ShowNotify("Обработка данных успешно выполнена.");
	            }
	            else
	            {
	                _messageService.ShowErrorFormatted("При обработке данных произошла ошибка.\r\n{0}",processingResult.LastError);
	            }
	        }).ContinueWith(p => _dockLayoutManager.HideOverlay());
	    }
	}
}
