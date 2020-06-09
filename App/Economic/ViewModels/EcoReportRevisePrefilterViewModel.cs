using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Core;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EcoReportRevisePrefilterViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IComplexReportRepository _complexReportRepository;
        public bool? DialogResult { get; set; }
        private DataErrorInfoSupport _dataErrorInfoSupport;

        private string _selectedTerritory;
        private DateTime? _beginDate;
        private DateTime? _endDate;
        private bool _isDesign;

        private RelayCommand _createReportCommand;
        

        public EcoReportRevisePrefilterViewModel(IMessageService messageService, IComplexReportRepository complexReportRepository)
        {
            _messageService = messageService;
            _complexReportRepository = complexReportRepository;
            Init();
        }

        private void Init()
        {
            //По-умолчанию начало отчетного периода 1.09.текущий_год - 1
            BeginDate = new DateTime(DateTime.Now.Year - 1, 10, 1);
            //По-умолчанию конец отчетного периода 30.09.текущий_год
            EndDate = new DateTime(DateTime.Now.Year, 9, 30);

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        [Required(ErrorMessage = @"Поле 'Территория' обязательно для заполнения")]
        public string SelectedTerritory
        {
            get { return _selectedTerritory; }
            set
            {
                _selectedTerritory = value;
                RaisePropertyChanged("SelectedTerritory");
            }
        }

        [Required(ErrorMessage = @"Поле 'Начало отчетного периода' обязательно для заполнения")]
        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set
            {
                _beginDate = value;
                RaisePropertyChanged("BeginDate");
            }
        }

        [Required(ErrorMessage = @"Поле 'Конец отчетного периода' обязательно для заполнения")]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                RaisePropertyChanged("EndDate");
            }
        }

        public bool IsDesign
        {
            get { return _isDesign; }
            set
            {
                _isDesign = value;
                RaisePropertyChanged("IsDesign");
            }
        }

        public ICommand CreateReportCommand
        {
            get { return _createReportCommand ?? (_createReportCommand = new RelayCommand(CreateReport, CanCreateReport)); }
        }

        /// <summary>
        /// Создание отчета "Акт сверки счетов за МП"
        /// </summary>
        /// <remarks>
        /// Генерируем отчет обходя систему отчетов
        /// </remarks>
        private void CreateReport()
        {
            try
            {
                if (!BeginDate.HasValue)
                {
                    _messageService.ShowError("Начальная дата не задана.");
                    return;
                }

                if (!EndDate.HasValue)
                {
                    _messageService.ShowError("Конечная дата не задана.");
                    return;
                }

                var report = new FastReport.Report();
                report.Load(GlobalConfig.BaseDirectory + @"\Economic\Reports\" + "ActRevise.frx");

                var result = _complexReportRepository.CreateActReviseReport(BeginDate.Value, EndDate.Value, SelectedTerritory);
                if (result.Success)
                {
                    report.RegisterData(result.Data.Item1, "dataSrc");
                    report.RegisterData(result.Data.Item2, "dataDst");
                    report.SetParameterValue("BeginDate", BeginDate);
                    report.SetParameterValue("EndDate", EndDate);
                    report.SetParameterValue("TerritorySrcRegionName", result.Data.Item3.RegionName);
                    report.SetParameterValue("territoryDstRegionName", result.Data.Item4.RegionName);
                    report.SetParameterValue("TerritorySrcShortName", result.Data.Item3.ShortName);
                    report.SetParameterValue("territoryDstShortName", result.Data.Item4.ShortName);

                    report.SetParameterValue("TerritorySrcPositionName", result.Data.Item3.PositionName);
                    report.SetParameterValue("territoryDstPositionName", result.Data.Item4.PositionName);
                    report.SetParameterValue("TerritorySrcFullName", string.Format("{0}.{1}. {2}", result.Data.Item3.Name[0], result.Data.Item3.Patronymic[0], result.Data.Item3.Surname));
                    report.SetParameterValue("territoryDstFullName", string.Format("{0}.{1}. {2}", result.Data.Item4.Name[0], result.Data.Item4.Patronymic[0], result.Data.Item4.Surname));

                    report.Prepare();
                    if (IsDesign)
                    {
                        report.Design();
                    }
                    else
                    {
                        report.Show();
                    }

                    report.Dispose();
                }

            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При формировании отчета произошло исключение", typeof(EcoReportForm2PrefilterViewModel));
            }
        }

        private bool CanCreateReport()
        {
            return _dataErrorInfoSupport.Error.Length == 0;
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
