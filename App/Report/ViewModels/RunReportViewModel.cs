using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Core;
using Core.Extensions;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace Medical.AppLayer.Report.ViewModels
{
    public class RunReportViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly IReportService _reportService;
        private readonly int _version;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        private readonly int _id;
        private readonly int _scope;
        private readonly int _userId;

        private RelayCommand _selectCommand;

        public RunReportViewModel(IUserService userService, 
            IMessageService messageService, 
            IMedicineRepository repository, 
            IReportService reportService, 
            int scope, int id,
            int? version)
        {
            _id = id;
            _scope = scope;
            _userId = userService.UserId;
            _messageService = messageService;
            _repository = repository;
            _reportService = reportService;
            _version = version ?? Constants.Version21;
            Init();
        }

        private void Init()
        {
            
            var reportsResult = _repository.GetEnabledReportsByScopeByVersion(_scope, _version);
            if (reportsResult.Success)
            {
                AllReportList = new ObservableCollection<FactReport>(reportsResult.Data);
                FilteresReportList = new ObservableCollection<FactReport>(reportsResult.Data);
            }

            //TODO заменить на новую систему пользователей!!!
            var employeeResult = _repository.GetEmployee();
            if (employeeResult.Success)
            {
                AllEmployeeList = new ObservableCollection<localEmployee>(employeeResult.Data);
                foreach (localEmployee employee in AllEmployeeList)
                {
                    employee.Surname = string.Format("{0} {1}.{2}.",
                        employee.Surname,
                        string.IsNullOrEmpty(employee.EName) ? "Нет" : employee.EName.Substring(0, 1),
                        string.IsNullOrEmpty(employee.Patronymic) ? "Нет" : employee.Patronymic.Substring(0, 1));
                }
            }

            this.ApplyDefaultValues();
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public ObservableCollection<FactReport> AllReportList { get; set; }
        public ObservableCollection<FactReport> FilteresReportList { get; set; }
        public ObservableCollection<localEmployee> AllEmployeeList { get; set; }
        public List<object> SelectedReportsList { get; set; }

        [DefaultValue(true)]
        public bool IsPreview { get; set; }

        public ICommand SelectCommand => _selectCommand ?? (_selectCommand = new RelayCommand(DoSelect,CanDoSelect));

        private bool CanDoSelect()
        {
            return SelectedReportsList != null && SelectedReportsList.Count > 0;
        }

        public void DoSelect()
        {
            try
            {
                _reportService.PrepareReports(SelectedReportsList, _userId, _id, _scope, null, IsPreview);
                
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При выполнении отчетов произошло исключение", typeof(RunReportViewModel));
            }
        }

        string IDataErrorInfo.Error => _dataErrorInfoSupport.Error;

        string IDataErrorInfo.this[string memberName] => _dataErrorInfoSupport[memberName];

        [Browsable(false)]
        public string Error => _dataErrorInfoSupport.Error;
    }
}