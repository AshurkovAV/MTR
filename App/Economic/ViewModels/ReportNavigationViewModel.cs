using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Input;
using Core;
using Core.Services;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class ReportNavigationViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly dynamic _databaseSettings;
        private readonly IAppRemoteSettings _remoteSettings;
        private string _tf_code;

        private string _selectedTerritory;
        private DateTime? _beginDate;
        private DateTime? _endDate;
        private bool _isDesign;

        private RelayCommand _createReportCommand;
        

        public ReportNavigationViewModel
            (IMessageService messageService,
            IAppShareSettings shareSettings,
            IAppRemoteSettings _remoteSettings)
        {
            _messageService = messageService;
            _databaseSettings = shareSettings.Get("database");
            dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
            _tf_code = Convert.ToString(terCode.tf_code);
        }

        public ICommand CreateReportCommand
        {
            get { return _createReportCommand ?? (_createReportCommand = new RelayCommand(CreateReport, CanCreateReport)); }
        }

        private void CreateReport()
        {
            try
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();
                var report = new FastReport.Report();
                //report.Load(_assembly.GetManifestResourceStream("Medical.AppLayer.Economic.Reports.Debet25.frx"));
                report.Load(GlobalConfig.BaseDirectory + @"\Economic\Reports\" + "Debet25.frx");
                //report.Load(GlobalConfig.BaseDirectory + @"\Economic\Reports\" + "Debet25.frx");
                report.Dictionary.Connections[0].ConnectionString = (string) _databaseSettings.ConnectionString;
                report.SetParameterValue("@region", _tf_code);
                report.Prepare();
                report.Show();
                report.Dispose();

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При формировании отчета произошло исключение",
                    typeof(EcoReportForm2PrefilterViewModel));
            }
        }

        private bool CanCreateReport()
        {
            return true;
        }

        string IDataErrorInfo.Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }

        string IDataErrorInfo.this[string memberName]
        {
            get { return _dataErrorInfoSupport[memberName]; }
        }

        [Browsable(false)]
        public string Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }


        
    }
}
