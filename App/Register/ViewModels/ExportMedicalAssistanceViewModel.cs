using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;
using Medical.DataCore.ffoms;

namespace Medical.AppLayer.Register.ViewModels
{
    public class ExportMedicalAssistanceViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly INotifyManager _notifyManager;
        private readonly IFileService _fileService;
        private readonly IMedicineRepository _repository;
        private readonly IFfomsReportRepository _ffomsReportRepository;
        private readonly ICacheRepository _cache;
        private readonly IDataService _dataService;
        private readonly ITextService _textService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly ICommonService _commonService;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private string _pathName;
        private int _typeExport;
        private int _selectedMonth;
        private string _acceptNumber;
        private DateTime? _acceptDate;
        private string _tableName;
        private string _fileName;
        private int _number;
        private string _outFileName;
        private string _notes;

        public string Notes {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged(()=>Notes); }
        }
        public string PathName {
            get { return _pathName; }
            set { _pathName = value;RaisePropertyChanged(()=>PathName); }
        }
        public StringBuilder ResultText { get; set; }

        private RelayCommand _exportCommand;
        private RelayCommand _saveLogCommand;
        private RelayCommand _selectPathCommand;
        private RelayCommand _showFileCommand;

        public ObservableCollection<string> TableNameList { get; set; }
        public ObservableCollection<object> ExportConfig { get; set; }
        public ObservableCollection<CommonTuple> MonthList { get; set; } 

        #region IHash
        public string Hash
        {
            get { return typeof(ExportMedicalAssistanceViewModel).FullName; }
        }
        #endregion

        public ExportMedicalAssistanceViewModel(INotifyManager notifyManager, 
            IFileService fileService,
            IMedicineRepository repository,
            IFfomsReportRepository ffomsReportRepository, 
            ICacheRepository cache, 
            IDataService dataService,
            ITextService textService,
            ICommonService commonService,
            IDockLayoutManager dockLayoutManager)
        {
            _notifyManager = notifyManager;
            _fileService = fileService;
            _repository = repository;
            _ffomsReportRepository = ffomsReportRepository;
            _cache = cache;
            _dataService = dataService;
            _textService = textService;
            _commonService = commonService;
            _dockLayoutManager = dockLayoutManager;

            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            ExportConfig = new ObservableCollection<object>
            {
                new { Name = "Основной файл со сведениями об утверждённых объёмах",             FileNameFormat = "OI{0:D2}{1:D2}",          Id = 1},
                new { Name = "Файлы с изменениями сведений об утверждённых объёмах",            FileNameFormat = "OJ{0:D2}{1:D2}{2:D4}",    Id = 2},
                new { Name = "Файлы с уточнёнными сведениями об утверждённых объёмах",          FileNameFormat = "OU{0:D2}{1:D2}{2:D4}",    Id = 3},
                new { Name = "Файлы с изменениями уточнённых сведений об утверждённых объёмах", FileNameFormat = "OV{0:D2}{1:D2}{2:D4}",    Id = 4},
                new { Name = "Файл со сведениями об оказанной медицинской помощи",              FileNameFormat = "OR{0:D2}{1:D2}{2:D4}",    Id = 5},
                new { Name = "Файлы с изменениями сведений об оказанной медицинской помощи",    FileNameFormat = "OS{0:D2}{1:D2}{2:D4}",    Id = 6},
            };

            MonthList = new ObservableCollection<CommonTuple>( _commonService.GetMonths());

            var tableResult = _ffomsReportRepository.GetTableNameList();
            if (tableResult.Success)
            {
                TableNameList = new ObservableCollection<string>(tableResult.Data);
            }

            TypeExport = 0;
        }

        public int TypeExport
        {
            get { return _typeExport; }
            set
            {
                _typeExport = value;
                RaisePropertyChanged("TypeExport");
                RaisePropertyChanged("VisibilityType1");
                RaisePropertyChanged("VisibilityType2");
                UpdateFileName();
            }
        }

        public int SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                _selectedMonth = value;
                RaisePropertyChanged("SelectedMonth");
                UpdateFileName();
            }
        }

        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                RaisePropertyChanged("TableName");
            }
        }

        public string AcceptNumber
        {
            get { return _acceptNumber; }
            set
            {
                _acceptNumber = value;
                RaisePropertyChanged("AcceptNumber");
            }
        }

        public DateTime? AcceptDate
        {
            get { return _acceptDate; }
            set
            {
                _acceptDate = value;
                RaisePropertyChanged("AcceptDate");
            }
        }

        public string VisibilityType1
        {
            get
            {
                switch (TypeExport)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return "Visible";
                    case 4:
                    case 5:
                        return "Collapsed";
                    default:
                        return "Collapsed";
                }
            }
        }
        public string VisibilityType2
        {
            get
            {
                switch (TypeExport)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return "Collapsed";
                    case 4:
                    case 5:
                        return "Visible";
                    default:
                        return "Collapsed";
                }
            }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; RaisePropertyChanged("Number"); UpdateFileName(); }
        }

        public string FileName {
            get { return _fileName; }
            set { _fileName = value; RaisePropertyChanged("FileName"); }
        }
        public string OutFileName
        {
            get { return _outFileName; }
            set { _outFileName = value; RaisePropertyChanged(()=>OutFileName); }
        }

        private void UpdateFileName()
        {
            dynamic config = ExportConfig[TypeExport];
            FileName = string.Format(config.FileNameFormat, 46, DateTime.Now.ToString("yy"), _number);
        }

        public ICommand ExportCommand
        {
            get { return _exportCommand ?? (_exportCommand = new RelayCommand(Export, CanExport)); }
        }

        private void Export()
        {
            OutFileName = null;
            ResultText = new StringBuilder();
            _dockLayoutManager.ShowOverlay(Constants.FlushDataMsg, Constants.PleaseWaitMsg);
            
            dynamic config = ExportConfig[TypeExport];
                
            ResultText.AppendLine("<b>Тип выгрузки {0}</b><br>".F((string)config.Name));

            int id = config.Id;

            switch (id)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    ExportMedicalAssistanceVolume();
                    break;
                case 5:
                case 6:
                    ExportMedicalAssistanceVolumeMonth();
                    break;
            }

            var logResult = _textService.ToRtf(ResultText.ToString());
            if (logResult.Success)
            {
                Notes = logResult.Data;
            }
            _dockLayoutManager.HideOverlay();
        }

        private bool CanExport()
        {
            return Directory.Exists(PathName);
        }

        private void ExportMedicalAssistanceVolume()
        {
            OutFileName = Path.Combine(PathName, FileName + ".oms");
            var xmlFilename = Path.Combine(PathName, FileName + ".xml");

            var register = new MedicalAssistancePlanned
            {
                Header = new MedicalAssistanceHeader {Version = "1.0", Date = DateTime.Now, Filename = FileName},
                CommonInfo =
                    new MedicalAssistanceCommonInfo
                    {
                        Code = Number,
                        Year = DateTime.Now.Year,
                        AcceptNumber = AcceptNumber,
                        AcceptDate = AcceptDate,
                        MedicalOrganizationExists = 1
                    },
                DetailsCollection = new List<MedicalAssistanceDetails>()
            };

            var assistanceValueResult = _repository.GetMedicalAssistanceVolume();
            if (assistanceValueResult.Success)
            {
                var groupedAssistanceVolume = assistanceValueResult.Data.GroupBy(p => p.MedicalOrganization);
                var position = 1;

                foreach (var item in groupedAssistanceVolume)
                {
                    var details = new MedicalAssistanceDetails
                    {
                        IndicatorsCollection = new List<MedicalAssistanceIndicators>(),
                        PositionNumber = position
                    };

                    var f003Cache = _cache.Get(CacheRepository.F003Cache);
                    if (f003Cache != null)
                    {
                        details.MedicalOrganization = f003Cache.GetString(item.Key);
                    }

                    foreach (var value in item.ToList())
                    {

                        var indicator = new MedicalAssistanceIndicators
                        {
                            IndicatorCode = value.globalMedAssisVolIndicator.Code,
                            ProfilesCollection = new List<MedicalAssistanceProfiles>()
                        };
                        var profile = new MedicalAssistanceProfiles
                        {
                            IndicatorValueTotal = value.Volume,
                            IndicatorPriceTotal = value.Amount,
                            ProfileCode = 0
                        };

                        indicator.ProfilesCollection.Add(profile);
                        details.IndicatorsCollection.Add(indicator);
                    }
                    register.DetailsCollection.Add(details);
                    position++;
                }
            }

           
            var serializeResult = _dataService.SerializeToFile(xmlFilename, register);
            if (serializeResult.Success)
            {
                var packResult = _fileService.Pack(OutFileName, new List<string> { xmlFilename }, false);
                if (packResult.Success)
                {
                    _notifyManager.ShowNotify("Данные успешно выгружены.");
                }
            }
            ResultText.AppendLine("<b>Готово! Файл {0}</b><br>".F(OutFileName));
        }

        private void ExportMedicalAssistanceVolumeMonth()
        {
            OutFileName = Path.Combine(PathName, FileName + ".oms");
            var xmlFilename = Path.Combine(PathName, FileName + ".xml");

            var register = new MedicalAssistanceCompleted
            {
                Header = new MedicalAssistanceHeader {Version = "1.0", Date = DateTime.Now, Filename = FileName},
                CommonInfo =
                    new MedicalAssistanceCommonInfo
                    {
                        Code = Number,
                        Year = DateTime.Now.Year,
                        Month = SelectedMonth,
                        MedicalOrganizationExists = 1
                    },
                DetailsCollection = new List<MedicalAssistanceDetails>(),
                RecordsCollection = new List<MedicalAssistanceRecords>()
            };

            var dataResult = _ffomsReportRepository.GetDataByTableName<MedicalEventCompleted>(TableName);
            if (dataResult.Success)
            {
                //копируем данные для возможности манипуляций с ними
                var convertedData = dataResult.Data.ToList();
                ResultText.AppendLine("<b>Найдено всего записей {0}</b><br>".F(convertedData.Count()));

                var queries = new Dictionary<int, IEnumerable<MedicalEventCompleted>>
                    {
                        { 1, convertedData.Where(p => p.AssistanceConditions == 3 && p.EventType == 2)},
                        { 2, convertedData.Where(p => p.AssistanceConditions == 3 && p.EventType == 3)},
                        { 3, convertedData.Where(p => p.AssistanceConditions == 3 && p.EventType == 1)},
                        { 4, convertedData.Where(p => p.AssistanceConditions == 2)},
                        { 5, convertedData.Where(p => p.AssistanceConditions == 2)},
                        { 8, convertedData.Where(p => p.AssistanceConditions == 1 )},
                        { 9, convertedData.Where(p => p.AssistanceConditions == 1 )},
                        { 10, convertedData.Where(p => p.AssistanceConditions == 1 && p.Price == 86837.5M)},
                        { 11, convertedData.Where(p => p.AssistanceConditions == 1 && p.Price == 86837.5M)},
                        { 12, convertedData.Where(p => p.AssistanceConditions == 4)},
                    };

                var indicatorCount = 1;
                var recordCount = 1;
                foreach (KeyValuePair<int, IEnumerable<MedicalEventCompleted>> query in queries)
                {
                    var details = new MedicalAssistanceDetails
                    {
                        PositionNumber = indicatorCount++,
                        MedicalOrganization = "460001",
                        IndicatorsCollection = new List<MedicalAssistanceIndicators>()
                    };
                    var indicator = new MedicalAssistanceIndicators
                    {
                        IndicatorCode = query.Key,
                        ProfilesCollection = new List<MedicalAssistanceProfiles>()
                    };
                    var profile = new MedicalAssistanceProfiles
                    {
                        ProfileCode = 0,
                        IndicatorValue = query.Value.Where(p => p.InsuranceOkato == TerritoryService.TerritoryOkato).Sum(p => p.IndicatorValue) ?? 0,
                        IndicatorPrice = query.Value.Where(p => p.InsuranceOkato == TerritoryService.TerritoryOkato).Sum(p => p.IndicatorPrice) ?? 0,
                        IndicatorTerritoryValue = query.Value.Where(p => p.InsuranceOkato != TerritoryService.TerritoryOkato).Sum(p => p.IndicatorValue) ?? 0,
                        IndicatorTerritoryPrice = query.Value.Where(p => p.InsuranceOkato != TerritoryService.TerritoryOkato).Sum(p => p.IndicatorPrice) ?? 0
                    };

                    if (profile.IndicatorPrice == 0 && profile.IndicatorTerritoryPrice == 0)
                    {
                        continue;
                    }

                    indicator.ProfilesCollection.Add(profile);
                    details.IndicatorsCollection.Add(indicator);
                    register.DetailsCollection.Add(details);

                    ResultText.AppendLine("<b>Индикатор {0}</b><br>".F(query.Key));

                    int? days = 0;
                    int? daysTerritory = 0;
                    foreach (var data in query.Value)
                    {
                        var record = new MedicalAssistanceRecords
                        {
                            PositionNumber = recordCount++,
                            Patient = new MedicalAssistancePatient
                            {
                                InsuranceOkato = data.InsuranceOkato,
                                Sex = data.Sex,
                                Age = data.Age
                            },
                            MedicalEvent = new MedicalAssistanceEvent
                            {
                                ExternalId = data.ExternalId,
                                AssistanceConditions = data.AssistanceConditions,
                                AssistanceType = data.AssistanceType,
                                AssistanceForm = data.EventType == 3 ? 2 : 3,
                                Target = data.EventType == 3 ? 0 : data.EventType,
                                MedicalOrganizationCode = data.MedicalOrganizationCode,
                                Profile = data.Profile
                            }
                        };

                        //record.MedicalService.HighTechAssistanceType = "6";
                        //record.MedicalService.HighTechAssistanceMethod = 7;
                        if (data.EventBegin.HasValue && data.EventEnd.HasValue)
                        {
                            record.MedicalEvent.Days = 0;
                            if (data.EventEnd.Value < data.EventBegin.Value)
                            {
                                record.MedicalEvent.Days = 1;
                                days += record.MedicalEvent.Days;
                            }
                            else
                            {
                                switch (query.Key)
                                {
                                    case 9:
                                        record.MedicalEvent.Days++;
                                        break;
                                    case 8:
                                    case 10:
                                        record.MedicalEvent.Days = Math.Max(1, (data.EventEnd.Value - data.EventBegin.Value).Days);
                                        if (data.InsuranceOkato == TerritoryService.TerritoryOkato)
                                        {
                                            days += record.MedicalEvent.Days;
                                        }
                                        else
                                        {
                                            daysTerritory += record.MedicalEvent.Days;
                                        }

                                        break;
                                    case 11:
                                        record.MedicalEvent.Days++;
                                        break;
                                    case 4:
                                        //Не трансформировать в лямбду, перестает работать выражение
                                        record.MedicalEvent.Days = Enumerable.Range(0, (data.EventEnd.Value - data.EventBegin.Value).Days + 1).Where(p => Enumerable.Range(1, 5).Contains((int)data.EventBegin.Value.AddDays(p).DayOfWeek)).Count();
                                        if (data.InsuranceOkato == TerritoryService.TerritoryOkato)
                                        {
                                            days += record.MedicalEvent.Days;
                                        }
                                        else
                                        {
                                            daysTerritory += record.MedicalEvent.Days;
                                        }
                                        break;
                                    case 5:
                                        record.MedicalEvent.Days++;
                                        break;
                                    case 1:
                                    case 2:
                                    case 3:
                                        record.MedicalEvent.Days = 1;
                                        break;
                                    case 12:
                                        record.MedicalEvent.Days = 1;
                                        break;
                                }
                            }

                        }


                        record.MedicalEvent.AcceptPrice = data.IndicatorPrice;

                        if (query.Key != 5 && query.Key != 9)
                        {
                            register.RecordsCollection.Add(record);
                        }
                        
                    }

                    switch (query.Key)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 5:
                        case 9:
                        case 11:
                        case 12:
                            ResultText.AppendLine("кол-во без иногородних {0}<br>".F(query.Value.Count(p => p.InsuranceOkato == TerritoryService.TerritoryOkato)));
                            ResultText.AppendLine("цена без иногородних {0}<br>".F(profile.IndicatorPrice));
                            ResultText.AppendLine("кол-во иногородних {0}<br>".F(query.Value.Count(p => p.InsuranceOkato != TerritoryService.TerritoryOkato)));
                            ResultText.AppendLine("цена иногородних {0}<br>".F(profile.IndicatorTerritoryPrice));
                            break;
                        case 4:
                        case 8:
                        case 10:
                            profile.IndicatorPrice = 0;
                            profile.IndicatorTerritoryPrice = 0;
                            profile.IndicatorValue = days;
                            profile.IndicatorTerritoryValue = daysTerritory;
                            ResultText.AppendLine("кол-во койко дней без иногородних {0}<br>".F(days));
                            ResultText.AppendLine("цена без иногородних {0}<br>".F(0));
                            ResultText.AppendLine("кол-во койко дней включая иногородних {0}<br>".F(daysTerritory));
                            ResultText.AppendLine("цена иногородних {0}<br>".F(0));

                            break;
                    }
                }
            }

            var serializeResult = _dataService.SerializeToFile(xmlFilename, register);
            if (serializeResult.Success)
            {
                var packResult = _fileService.Pack(OutFileName, new List<string> { xmlFilename }, false);
                if (packResult.Success)
                {
                    _notifyManager.ShowNotify("Данные успешно выгружены.");
                }
            }
            ResultText.AppendLine("<b>Готово! Файл {0}</b><br>".F(OutFileName));
        }

        public ICommand SelectPathCommand
        {
            get { return _selectPathCommand ?? (_selectPathCommand = new RelayCommand(SelectPath)); }
        }

        private void SelectPath()
        {
            var result = _fileService.SelectFolder("Выберете папку в которую необходимо выгрузить файлы");
            if (result.Success)
            {
                PathName = result.Data;
            }

            if (result.IsCanceled)
            {
                _notifyManager.ShowNotify("Операция отменена.");
            }
        }

        public ICommand SaveLogCommand
        {
            get { return _saveLogCommand ?? (_saveLogCommand = new RelayCommand(SaveLog, CanSaveLog)); }
        }

        private bool CanSaveLog()
        {
            return Notes.IsNotNullOrEmpty();
        }

        private void SaveLog()
        {
            var result = _fileService.SaveFileWithDialog(".rtf", "Rtf файлы (.rtf)|*.rtf", Notes, FileType.Text);
            if (result.Success)
            {
                _notifyManager.ShowNotify("Данные успешно сохранены.");
            }

            if (result.IsCanceled)
            {
                _notifyManager.ShowNotify("Операция отменена.");
            }
        }

        public ICommand ShowFileCommand
        {
            get { return _showFileCommand ?? (_showFileCommand = new RelayCommand(ShowFile, CanShowFile)); }
        }

        private bool CanShowFile()
        {
            return OutFileName.IsNotNullOrWhiteSpace();
        }

        private void ShowFile()
        {
            _fileService.ShowFileInExplorer(OutFileName);
        }

        public string Error
        {
            get
            {
                RaisePropertyChanged("Visibility");
                return _dataErrorInfoSupport.Error;
            }
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
    }
}