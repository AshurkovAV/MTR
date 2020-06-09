using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Core.Extensions;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Report.ViewModels
{
    public class PrintReportViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly IReportService _reportService;

        private DataErrorInfoSupport _dataErrorInfoSupport;

        private readonly int _id;
        private readonly int _scope;
        private readonly int? _subId;


        private RelayCommand<object> _selectCommand;

        public PrintReportViewModel(IMessageService messageService, 
            IMedicineRepository repository, 
            IReportService reportService, 
            int scope, int id, int? subId = null)
        {
            _id = id;
            _scope = scope;
            _subId = subId;
            
            _messageService = messageService;
            _repository = repository;
            _reportService = reportService;
            Init();
        }

        private void Init()
        {
            var preparedReportResult = _repository.GetPreparedReportByExternalIdScopeSubId(_id, _scope, _subId);
            if (preparedReportResult.Success)
            {
                AllReportList = new ObservableCollection<FactPreparedReport>(preparedReportResult.Data);
            }

            this.ApplyDefaultValues();
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public ObservableCollection<FactPreparedReport> AllReportList { get; set; }
        public List<object> SelectedReportsList { get; set; }

        [DefaultValue(true)]
        public bool? IsPreview { get; set; }

        [DefaultValue(false)]
        public bool? IsAppend { get; set; }

        [DefaultValue(false)]
        public bool? IsDbSave { get; set; }

        public ICommand SelectCommand => _selectCommand ?? (_selectCommand = new RelayCommand<object>(DoSelect,CanDoSelect));

        private bool CanDoSelect(object obj)
        {
            return SelectedReportsList != null && SelectedReportsList.Count > 0;
        }

        private void DoSelect(object obj)
        {
            try
            {
                _reportService.PrintReports(SelectedReportsList, IsPreview);
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При печати готовых отчетов произошло исключение", typeof(PrintReportViewModel));
            }
           
        }

        string IDataErrorInfo.Error => _dataErrorInfoSupport.Error;

        string IDataErrorInfo.this[string memberName] => _dataErrorInfoSupport[memberName];

        [Browsable(false)]
        public string Error => _dataErrorInfoSupport.Error;
    }
}