using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BLToolkit.Data.Linq;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.CoreLayer.Validation.Attribute;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    /// <summary>
    /// </summary>
    public class DeleteByPeriodViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly ICommonService _commonService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly IMessageService _messageService;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        private int _selectedYear; 
        private int _selectedMonth;
        private ObservableCollection<CommonTuple> _allAccountList;
        private string _log;
        private ObservableCollection<CommonTuple> _years;
        private ObservableCollection<CommonTuple> _months;

        public ObservableCollection<CommonTuple> Years
        {
            get { return _years ?? (_years = new ObservableCollection<CommonTuple>()); }
        }
        public ObservableCollection<CommonTuple> Months { get { return _months ?? (_months = new ObservableCollection<CommonTuple>()); } }

        public ObservableCollection<CommonTuple> AllAccountList
        {
            get { return _allAccountList ?? (_allAccountList = new ObservableCollection<CommonTuple>()); }
            set
            {
                _allAccountList = value;
                RaisePropertyChanged(() => AllAccountList);
            }
        }

        [CustomValidation(typeof (LengthValidation), "ValidateCollection")]
        public List<object> SelectedAccountList { get; set; }

        public string Log
        {
            get { return _log; }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    _log = value;
                    RaisePropertyChanged(() => Log);
                });
            }
        }

        public int SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                RaisePropertyChanged(()=>Years);
                SelectedMonth = DateTime.Now.Month;
            }
        }

        public int SelectedMonth
        {
            get { return _selectedMonth; }
            set { 
                _selectedMonth = value; 
                RaisePropertyChanged(() => SelectedMonth);
                UpdateAccountList();
            }
        }

        private void UpdateAccountList()
        {
            var date = new DateTime(_selectedYear, _selectedMonth, 01);
            AllAccountList.Clear();

            TransactionResult<IEnumerable<MedicalAccountView>> accountResult =
                _repository.GetMedicalAccountView(account =>
                    Sql.DateDiff(Sql.DateParts.Year, account.Date, date) == 0 &&
                    Sql.DateDiff(Sql.DateParts.Month, account.Date, date) == 0);
            if (accountResult.Success)
            {
                if (accountResult.Data.Any())
                {
                    AllAccountList.AddRange(accountResult.Data.Select(p => new CommonTuple
                    {
                        DisplayField = _commonService.GetTitleByMedicalAccountView(p),
                        ValueField = p.MedicalAccountId,
                        DataField = p
                    }));
                }
                else
                {
                    _notifyManager.ShowNotify("Счета за указанный период не найдены.");
                }
            }
        }

        private RelayCommand _selectCommand;
        

        public DeleteByPeriodViewModel(IMedicineRepository repository,
            INotifyManager notifyManager,
            ICommonService commonService,
            IMessageService messageService,
            IDockLayoutManager dockLayoutManager)
        {
            _repository = repository;
            _notifyManager = notifyManager;
            _commonService = commonService;
            _messageService = messageService;
            _dockLayoutManager = dockLayoutManager;
            Init();
        }

        private void Init()
        {
            //TODO add only available accounts
            var result = _repository.GetMedicalAccountDate();
            if (result.Success) {
                Years.AddRange(new ObservableCollection<CommonTuple>(result.Data.Select(x => new CommonTuple{ DisplayField = "{0}г.".F(x), ValueField = x })));
            }
            Months.AddRange(_commonService.GetMonths());

            if (Years.Any())
            {
                SelectedYear = Years.Last().ValueField;
            }

            if (Months.Any())
            {
                SelectedMonth = Months.Last().ValueField;
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
            if (!_messageService.AskQuestion("Вы действительно хотите удалить все выбранные счета ({0}шт.)".F(SelectedAccountList.Count), "Внимание!"))
            {
                return;
            }
            Log = Constants.BreakUpAccountManyMsg + Constants.PleaseWaitMsg;
            _dockLayoutManager.ShowOverlay(Constants.DeleteAccountManyMsg, Constants.PleaseWaitMsg);
            Task.Factory.StartNew(() =>
            {
                var result = new StringBuilder();
                for (int i = 0; i < SelectedAccountList.Count; i++)
                {
                    int id = Convert.ToInt32(SelectedAccountList[i]);

                    _dockLayoutManager.SetOverlayMessage(Constants.DeleteAccountIdMsg.F(id));
                    TransactionResult breakUpResult = _repository.DeleteZMedicalAccount(id);
                    if (breakUpResult.Success)
                    {
                        result.AppendFormat("Счет ID {0} успешно удален\r\n", id);
                    }
                    else
                    {
                        //TODO log
                        result.AppendFormat("Ошибка удаления счета ID {0}\r\n", id);
                    }
                    _dockLayoutManager.SetOverlayProgress(SelectedAccountList.Count, i);
                }
                Log += result.ToString();
                _notifyManager.ShowNotify("Счета успешно удалены.");
            }).ContinueWith(p => _dockLayoutManager.HideOverlay());
        }
    }
}
