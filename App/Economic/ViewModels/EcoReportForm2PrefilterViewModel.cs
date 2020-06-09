using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Core;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Models.Common;
using Medical.CoreLayer.Service;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EcoReportForm2PrefilterViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IComplexReportRepository _complexReportRepository;

        public bool? DialogResult { get; set; }
        private DataErrorInfoSupport _dataErrorInfoSupport;

        public ObservableCollection<CommonItem> FieldsList { get; set; }
        public int SelectedRange { get; set; }

        private int _selectedDirection;
        private DateTime? _beginDate;
        private bool _isDesign;

        private RelayCommand _createReportCommand;

        public EcoReportForm2PrefilterViewModel(IMessageService messageService, IComplexReportRepository complexReportRepository)
        {
            _messageService = messageService;
            _complexReportRepository = complexReportRepository;
            Init();
        }

        private void Init()
        {
            FieldsList = new ObservableCollection<CommonItem>
            {
                new CommonItem { DisplayField = "1 месяц", ValueField = 1 },
                new CommonItem { DisplayField = "3 месяца", ValueField = 3 },
                new CommonItem { DisplayField = "6 месяцев", ValueField = 6 },
                new CommonItem { DisplayField = "9 месяцев", ValueField = 9 },
                new CommonItem { DisplayField = "1 год", ValueField = 12 },
                new CommonItem { DisplayField = "3 года (для тестов)", ValueField = 36 },
            };
            SelectedRange = FieldsList.First().ValueField;
            
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        [Required(ErrorMessage = @"Поле 'Направление счета' обязательно для заполнения")]
        public int SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                RaisePropertyChanged("SelectedDirection");
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

        private void CreateReport()
        {
            try
            {
                if (!BeginDate.HasValue)
                {
                    _messageService.ShowError("Начальная дата не задана.");
                    return;
                }

                var endDate = BeginDate.Value.AddMonths(SelectedRange);

                var report = new FastReport.Report();
                report.Load(GlobalConfig.BaseDirectory + @"\Economic\Reports\" + "Form2.frx");

                var result = _complexReportRepository.CreateForm2Report(BeginDate.Value, endDate, SelectedDirection);
                if (result.Success)
                {
                    report.RegisterData(result.Data.Item1, "result");
                    report.SetParameterValue("BeginDate", BeginDate);
                    report.SetParameterValue("EndDate", endDate);
                    report.SetParameterValue("Direction", SelectedDirection);
                    report.SetParameterValue("Total", result.Data.Item2);

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
