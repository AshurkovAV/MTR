using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Helpers;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;
using Medical.DataCore.Interface;
using v10 = Medical.DataCore.v10;
using v21 = Medical.DataCore.v21;
using v30 = Medical.DataCore.v30;
using v31 = Medical.DataCore.v31;
using v32 = Medical.DataCore.v32;
using v10PL = Medical.DataCore.v10PL;
using GalaSoft.MvvmLight.Threading;
using System.Threading.Tasks;
using Autofac;
using Core.Services;
using Medical.CoreLayer.Models.Common;
using Medical.CoreLayer.Validation.Attribute;
using Medical.DataCore.v10PL.PL;

namespace Medical.AppLayer.Register.ViewModels
{
    public class ExportXmlViewModel : ViewModelBase
    {
        private readonly IAppRemoteSettings _remoteSettings;
        private readonly IDataService _dataService;
        private readonly IFileService _fileService;
        private readonly INotifyManager _notifyManager;
        private readonly ITextService _textService;
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly ICacheRepository _cache;
        private readonly ICommonService _commonService;
        private readonly IComplexReportRepository _complexReportRepository;
        private readonly IDockLayoutManager _dockLayoutManager;

        private int _id;
        private RelayCommand _exportCommand;
        private RelayCommand _saveLogCommand;
        private RelayCommand _selectFileCommand;
        private RelayCommand _showFileCommand;

        private string _fileName;
        private string _notes;
        private string _omsFileName;

        public ExportXmlViewModel(IAppRemoteSettings remoteSettings,
            IDataService dataService,
            IFileService fileService,
            INotifyManager notifyManager,
            ITextService textService,
            IMessageService messageService,
            IMedicineRepository repository,
            ICacheRepository cache,
            ICommonService commonService,
            IComplexReportRepository complexReportRepository,
            IDockLayoutManager dockLayoutManager,
            int id)
        {
            this.ApplyDefaultValues();

            _remoteSettings = remoteSettings;
            _dataService = dataService;
            _fileService = fileService;
            _textService = textService;
            _messageService = messageService;
            _notifyManager = notifyManager;
            _repository = repository;
            _cache = cache;
            _commonService = commonService;
            _complexReportRepository = complexReportRepository;
            _dockLayoutManager = dockLayoutManager;
            _id = id;

            _overlayManager = Di.I.Resolve<IOverlayManager>();

            Init();
        }

        private void Init()
        {
            var versionResult = _repository.GetGlobalVersion();
            if (versionResult.Success)
            {
                VersionList = new ObservableCollection<globalVersion>(versionResult.Data);
                dynamic v = _remoteSettings.Get(AppRemoteSettings.DefaultOmsVersion);
                var version = _repository.GetTerritoryAccountVersionById(_id);
                Version = version.Success ? version.Data : v.Version;
            }

            AllOptionsList = new ObservableCollection<CommonTuple>
            {
                new CommonTuple {ValueField = (int) ExportOptions.ExportServices, DisplayField = "Выгружать услуги"},
            };

            TypeLoad = _dataService.GetAccountType(_id);
            dynamic pathA = _remoteSettings.Get(AppRemoteSettings.FileXmlOms);
            string fileA = pathA.fileA;
            string fileD = pathA.fileD;
            string fileR = pathA.fileR;
            if (TypeLoad == 0)
            {
                FileName = fileR;
            }
            if (TypeLoad == 1)
            {
                //Если выгружается Протокол обработки и версия счета 3,2 то меняем версию счета на 3,1
                //т.к. в приказе версия протокола обработки не менялась 
                if (Version == Constants.Version32)
                {
                    Version = Constants.Version31;
                }
                TypeLoadEx = 3;
                FileName = fileA;
            }
            if (TypeLoad == 2)
            {
                FileName = fileD;
            }

            if (CommonGridControl<FactTerritoryAccount>._selectTerritoryAccount.IsNotNull())
                if (CommonGridControl<FactTerritoryAccount>._selectTerritoryAccount.Any())
                {
                    AllAccountList.AddRange(
                        CommonGridControl<FactTerritoryAccount>._selectTerritoryAccount.Select(p => new CommonTuple
                        {
                            DisplayField = _commonService.GetTitleByTerritoryAccountView(p),
                            ValueField = p.TerritoryAccountId,
                            DataField = p
                        }));
                    
                }

        }

        public bool? DialogResult { get; set; }
        public string FileName { get { return _fileName; }
            set { _fileName = value; RaisePropertyChanged(()=>FileName); }
        }
        public StringBuilder ResultText { get; set; }

        [DefaultValue(0)]
        public int TypeLoad { get; set; }

        public int TypeLoadEx { get; set; }
        [DefaultValue(0)]   
        public int FileExt { get; set; }
        public string Notes {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged(()=>Notes); }
        }
        public int Version { get; set; }
        public string OmsFileName
        {
            get { return _omsFileName; }
            set { _omsFileName = value; RaisePropertyChanged(() => OmsFileName); }
        }

        public int Direction { get; set; }

        public ObservableCollection<globalVersion> VersionList { get; set; }
        private ObservableCollection<CommonTuple> _allAccountList;
        public ObservableCollection<CommonTuple> AllAccountList
        {
            get { return _allAccountList ?? (_allAccountList = new ObservableCollection<CommonTuple>()); }
            set
            {
                _allAccountList = value;
                RaisePropertyChanged(() => AllAccountList);
            }
        }

        public ObservableCollection<CommonTuple> AllOptionsList { get; set; }
        public List<object> SelectedOptionsList { get; set; }
        [CustomValidation(typeof(LengthValidation), "ValidateCollection")]
        public List<object> SelectedAccountList { get; set; }

        public ICommand SelectFileCommand
        {
            get { return _selectFileCommand ?? (_selectFileCommand = new RelayCommand(SelectFile)); }
        }

        public ICommand ExportCommand
        {
            get { return _exportCommand ?? (_exportCommand = new RelayCommand(ExportFile, CanExportFile)); }
        }

        public ICommand ShowFileCommand
        {
            get { return _showFileCommand ?? (_showFileCommand = new RelayCommand(ShowFile, CanShowFile)); }
        }

        public ICommand SaveLogCommand
        {
            get { return _saveLogCommand ?? (_saveLogCommand = new RelayCommand(SaveLog, CanSaveLog)); }
        }

        private bool CanSaveLog()
        {
            return Notes.IsNotNullOrWhiteSpace();
        }

        private void SaveLog()
        {
            var result = _fileService.SaveFileWithDialog(".rtf", "Rtf файлы (.rtf)|*.rtf", Notes, FileType.Text);
            if (result.Success)
            {
                _notifyManager.ShowNotify("Данные успешно сохранены.");
            }
        }


        private void SelectFile()
        {
            var result = _fileService.SelectFolder("Выберете папку в которую необходимо выгрузить файлы реестров");
            if (result.Success)
            {
                FileName = result.Data;
            }
        }
        private IOverlayManager _overlayManager;
        private void ExportFile()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => _dockLayoutManager.ShowOverlay(Constants.FlushDataMsg, Constants.PleaseWaitMsg));
            var task = _overlayManager.ShowOverlay(Constants.FlushDataMsg, Constants.PleaseWaitMsg);
            task.ContinueWith(t =>
            {
                ResultText = new StringBuilder();

                if (SelectedAccountList.IsNotNull())
                {
                    foreach (var o in SelectedAccountList)
                    {
                        _id = (int)o;
                        _overlayManager.ShowOverlay(Constants.FlushDataMsg, $"Выгружается счет номер {o}");
                        DoExportFile();
                        ResultText.AppendFormat("<br>");
                    }
                }
                else
                {
                    DoExportFile();
                }
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var result = _textService.ToRtf(ResultText.ToString());
                    if (result.Success)
                    {
                        Notes = result.Data;
                    }
                    else
                    {
                        _messageService.ShowException(result.LastError, "Исключение при конвертации html в rtf", typeof(ImportLocalXmlViewModel));
                    }
                });

            }).ContinueWith(t => _overlayManager.HideOverlay());
        }

        private bool CanShowFile()
        {
            return OmsFileName.IsNotNullOrWhiteSpace();
        }

        private void ShowFile()
        {
            _fileService.ShowFileInExplorer(OmsFileName);
        }

        private bool CanExportFile()
        {
            return FileName.IsNotNullOrWhiteSpace();
        }

        private void DoExportFile()
        {
            if (TypeLoad == 1)
            {
                TypeLoadEx = 3;
            }
            OmsFileName = string.Empty;

            _commonService.StartWatch();

            

            var options = SelectedOptionsList.ToEnumFlag<ExportOptions>();

            var versionCache = _cache.Get(CacheRepository.VersionCache);
            if (versionCache.IsNull())
            {
                ResultText.AppendLine("Не найден кэш версии обмена");
                return;
            }

            var versionString = versionCache.GetString(Version);

            ResultText.AppendLine("<b>Версия: {0}</b><br>".F(versionString));

            _dockLayoutManager.SetOverlayMessage(Constants.ReadDataMsg);

            switch (TypeLoad)
            {
                case Constants.GeneralPart:
                    ResultText.AppendLine("<b>Начало экспорта реестра счетов (основная часть) в {0}</b><br>".F(FileName));
                    switch (Version)
                    {
                        case Constants.Version10:
                            var result10 = _dataService.Write<v10.E.RegisterE, 
                                v10.E.AccountE, 
                                v10.E.HeaderE,
                                v10.E.RecordsE,
                                v10.E.PatientE,
                                v10.E.EventE>(_id,Version, TypeLoad, options);
                            if (result10.Success)
                            {
                                WriteRegister(result10.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result10.ErrorMessage);
                            }

                            
                            break;
                        case Constants.Version21:
                            var resultv21 = _dataService.Write<v21.E.RegisterE, 
                                v21.E.AccountE,
                                v21.E.HeaderE,
                                v21.E.RecordsE,
                                v21.E.PatientE,
                                v21.E.EventE>(_id, Version, TypeLoad, options);
                            if (resultv21.Success)
                            {
                                WriteRegister(resultv21.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv21.ErrorMessage);
                            }
                            break;
                        case Constants.Version30:
                            var resultv30 = _dataService.WriteZ<v30.E.RegisterE,
                                v30.E.AccountE,
                                v30.E.HeaderE,
                                v30.E.RecordsE,
                                v30.E.PatientE,
                                v30.E.ZslEventE>(_id, Version, TypeLoad, options);
                            if (resultv30.Success)
                            {
                                WriteZRegister(resultv30.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv30.ErrorMessage);
                            }
                            break;
                        case Constants.Version31:
                            var resultv31 = _dataService.WriteZOnk<v31.E.RegisterE,
                                v31.E.AccountE,
                                v31.E.HeaderE,
                                v31.E.RecordsE,
                                v31.E.PatientE,
                                v31.E.ZslEventE,
                                v31.E.EventE,
                                v31.E.DirectionOnkE,
                                v31.E.ConsultationsOnkE,
                                v31.E.EventOnkE,
                                v31.E.DiagBlokOnkE,
                                v31.E.ContraindicationsOnkE,
                                v31.E.ServiceOnkE,
                                v31.E.AnticancerDrugOnkE,
                                v31.E.KsgKpgE,
                                v31.E.SlKoefE,
                                v31.E.RefusalE,
                                v31.E.ServiceE>(_id, Version, TypeLoad, options);
                            if (resultv31.Success)
                            {
                                WriteZRegister(resultv31.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv31.ErrorMessage);
                            }
                            break;
                        case Constants.Version32:
                            var resultv32 = _dataService.WriteZOnk<v32.E.RegisterE,
                                v32.E.AccountE,
                                v32.E.HeaderE,
                                v32.E.RecordsE,
                                v32.E.PatientE,
                                v32.E.ZslEventE, 
                                v32.E.EventE,
                                v32.E.DirectionOnkE,
                                v32.E.ConsultationsOnkE,
                                v32.E.EventOnkE,
                                v32.E.DiagBlokOnkE,
                                v32.E.ContraindicationsOnkE,
                                v32.E.ServiceOnkE,
                                v32.E.AnticancerDrugOnkE,
                                v32.E.KsgKpgE,
                                v32.E.SlKoefE,
                                v32.E.RefusalE,
                                v32.E.ServiceE>(_id, Version, TypeLoad, options);
                            if (resultv32.Success)
                            {
                                WriteZRegister(resultv32.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv32.ErrorMessage);
                            }
                            break;
                    }
                    
                    break;
                case Constants.LogPart:
                    ResultText.AppendLine("<b>Начало экспорта реестра счетов (протокол обработки) в {0}</b><br>".F(FileName));
                    switch (Version)
                    {
                        case Constants.Version10:
                            var result10 = _dataService.WriteAnswer<v10.EAnswer.RegisterEAnswer, 
                                v10.EAnswer.AccountEAnswer,
                                v10.EAnswer.HeaderEAnswer,
                                v10.EAnswer.RecordsEAnswer,
                                v10.EAnswer.PatientEAnswer,
                                v10.EAnswer.EventEAnswer>(_id, Version, TypeLoadEx);
                            if (result10.Success)
                            {
                                WriteRegisterAnswer(result10.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result10.ErrorMessage);
                            }
                            break;
                        case Constants.Version21:
                            var result21 = _dataService.WriteAnswer<v21.EAnswer.RegisterEAnswer,
                                v21.EAnswer.AccountEAnswer,
                                v21.EAnswer.HeaderEAnswer,
                                v21.EAnswer.RecordsEAnswer,
                                v21.EAnswer.PatientEAnswer,
                                v21.EAnswer.EventEAnswer>(_id, Version, TypeLoadEx);
                            if (result21.Success)
                            {
                                WriteRegisterAnswer(result21.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result21.ErrorMessage);
                            }
                            break;
                        case Constants.Version30:
                            var result30 = _dataService.WriteAnswerZ<v30.EAnswer.RegisterEAnswer,
                                v30.EAnswer.AccountEAnswer,
                                v30.EAnswer.HeaderEAnswer,
                                v30.EAnswer.RecordsEAnswer,
                                v30.EAnswer.PatientEAnswer,
                                v30.EAnswer.ZslEventEAnswer,
                                v30.EAnswer.EventEAnswer> (_id, Version, TypeLoadEx);
                            if (result30.Success)
                            {
                                WriteZRegisterAnswer(result30.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result30.ErrorMessage);
                            }
                            break;
                        case Constants.Version31:
                            var result31 = _dataService.WriteAnswerZ<
                                v31.EAnswer.RegisterEAnswer,
                                v31.EAnswer.AccountEAnswer,
                                v31.EAnswer.HeaderEAnswer,
                                v31.EAnswer.RecordsEAnswer,
                                v31.EAnswer.PatientEAnswer,
                                v31.EAnswer.ZslEventEAnswer,
                                v31.EAnswer.EventEAnswer>(_id, Version, TypeLoadEx);
                            if (result31.Success)
                            {
                                WriteZRegisterAnswer(result31.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result31.ErrorMessage);
                            }
                            break;
                        default:
                            ResultText.AppendLine("<b>Версия выгрузки не определена</b><br>");
                            break;
                            
                    }
                    
                    break;
                case Constants.CorrectedPart:
                    ResultText.AppendLine("<b>Начало экспорта реестра счетов (исправленная часть) в {0}</b><br>".F(FileName));
                    switch (Version)
                    {
                        case Constants.Version10:
                             var result10 = _dataService.WriteBack<v10.E.RegisterE, 
                                v10.E.AccountE, 
                                v10.E.HeaderE,
                                v10.E.RecordsE,
                                v10.E.PatientE,
                                v10.E.EventE>(_id,Version, TypeLoad, options);
                            if (result10.Success)
                            {
                                WriteRegister(result10.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result10.ErrorMessage);
                            }
                            break;
                        case Constants.Version21:
                            var resultv21 = _dataService.WriteBack<v21.E.RegisterE, 
                                v21.E.AccountE,
                                v21.E.HeaderE,
                                v21.E.RecordsE,
                                v21.E.PatientE,
                                v21.E.EventE>(_id, Version, TypeLoad, options);
                            if (resultv21.Success)
                            {
                                WriteRegister(resultv21.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv21.ErrorMessage);
                            }
                            break;
                        case Constants.Version30:
                            var resultv30 = _dataService.WriteBackZ<v30.E.RegisterE,
                                v30.E.AccountE,
                                v30.E.HeaderE,
                                v30.E.RecordsE,
                                v30.E.PatientE,
                                v30.E.ZslEventE>(_id, Version, TypeLoad, options);
                            if (resultv30.Success)
                            {
                                WriteZRegister(resultv30.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv30.ErrorMessage);
                            }
                            break;
                        case Constants.Version31:
                            var resultv31 = _dataService.WriteBackZOnk<v31.E.RegisterE,
                                v31.E.AccountE,
                                v31.E.HeaderE,
                                v31.E.RecordsE,
                                v31.E.PatientE,
                                v31.E.ZslEventE,
                                v31.E.EventE,
                                v31.E.DirectionOnkE,
                                v31.E.ConsultationsOnkE,
                                v31.E.EventOnkE,
                                v31.E.DiagBlokOnkE,
                                v31.E.ContraindicationsOnkE,
                                v31.E.ServiceOnkE,
                                v31.E.AnticancerDrugOnkE,
                                v31.E.KsgKpgE,
                                v31.E.SlKoefE,
                                v31.E.RefusalE,
                                v31.E.ServiceE>(_id, Version, TypeLoad, options);
                            if (resultv31.Success)
                            {
                                WriteZRegister(resultv31.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv31.ErrorMessage);
                            }
                            break;
                        case Constants.Version32:
                            var resultv32 = _dataService.WriteBackZOnk<v32.E.RegisterE,
                                v32.E.AccountE,
                                v32.E.HeaderE,
                                v32.E.RecordsE,
                                v32.E.PatientE,
                                v32.E.ZslEventE,
                                v32.E.EventE,
                                v32.E.DirectionOnkE,
                                v32.E.ConsultationsOnkE,
                                v32.E.EventOnkE,
                                v32.E.DiagBlokOnkE,
                                v32.E.ContraindicationsOnkE,
                                v32.E.ServiceOnkE,
                                v32.E.AnticancerDrugOnkE,
                                v32.E.KsgKpgE,
                                v32.E.SlKoefE,
                                v32.E.RefusalE,
                                v32.E.ServiceE>(_id, Version, TypeLoad, options);
                            if (resultv32.Success)
                            {
                                WriteZRegister(resultv32.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(resultv32.ErrorMessage);
                            }
                            break;
                    }
                    
                    break;
                default:
                    ResultText.AppendLine("<b>Неизвестный тип файла {0}</b><br>".F(TypeLoad));
                    break;
            }

            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
        }

        private void WriteRegister<T>(Tuple<T,RegisterEInfo, int> data) where T : IRegister
        {
            var xmlFilename = Path.Combine(FileName, data.Item2.FileNameXml);
            var omsFilename = Path.Combine(FileName, data.Item2.FileNameOms);

            var result = _dataService.SerializeToFile(xmlFilename, data.Item1, XmlOperations.RemoveEmptyOrNull);
            if (result.Success)
            {
                var packResult = _fileService.Pack(omsFilename, new List<string> { xmlFilename }, false);
                if (packResult.Success)
                {
                    var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                        omsFilename,
                        data.Item2, 
                        _id,
                        data.Item1.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                        data.Item1.InnerHeader.Date.HasValue ? data.Item1.InnerHeader.Date.Value : DateTime.Now,
                        Version,
                        TypeLoad,
                        data.Item3);
                    if (exchangeResult.Success)
                    {
                        var statResult = _complexReportRepository.DoRegisterEStats(data.Item1);
                        if (statResult.Success)
                        {
                            ResultText.Append(statResult.Data);
                        }
                        OmsFileName = omsFilename;
                        ResultText.Append("<b>Файл: {0}</b>".F(omsFilename));
                    }
                }
            }
        }

        private void WriteZRegister<T>(Tuple<T, RegisterEInfo, int> data) where T : IZRegister
        {
            var xmlFilename = Path.Combine(FileName, data.Item2.FileNameXml);
            var omsFilename = Path.Combine(FileName, data.Item2.FileNameOms);

            var result = _dataService.SerializeToFile(xmlFilename, data.Item1, XmlOperations.RemoveEmptyOrNull);
            if (result.Success)
            {
                var packResult = _fileService.Pack(omsFilename, new List<string> { xmlFilename }, false);
                if (packResult.Success)
                {
                    var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                        omsFilename,
                        data.Item2,
                        _id,
                        data.Item1.InnerRecordCollection.SelectMany(p => p.InnerZslEventCollection).Count(),
                        data.Item1.InnerHeader.Date.HasValue ? data.Item1.InnerHeader.Date.Value : DateTime.Now,
                        Version,
                        TypeLoad,
                        data.Item3);
                    if (exchangeResult.Success)
                    {
                        var statResult = _complexReportRepository.DoZRegisterEStats(data.Item1);
                        if (statResult.Success)
                        {
                            ResultText.Append(statResult.Data);
                        }
                        OmsFileName = omsFilename;
                        ResultText.Append("<b>Файл: {0}</b>".F(omsFilename));
                    }
                }
            }
        }

        private void WriteRegisterAnswer<T>(Tuple<T, RegisterEInfo, int> data) where T : IRegisterAnswer
        {
            var xmlFilename = Path.Combine(FileName, data.Item2.FileNameXml);
            var omsFilename = Path.Combine(FileName, data.Item2.FileNameOms);

            var result = _dataService.SerializeToFile(xmlFilename, data.Item1, XmlOperations.RemoveEmptyOrNull);
            if (result.Success)
            {
                var packResult = _fileService.Pack(omsFilename, new List<string> { xmlFilename }, false);
                if (packResult.Success)
                {
                    var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                        omsFilename,
                        data.Item2,
                        _id,
                        data.Item1.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                        data.Item1.InnerHeader.Date.HasValue ? data.Item1.InnerHeader.Date.Value : DateTime.Now,
                        Version,
                        TypeLoadEx,
                        data.Item3);
                    if (exchangeResult.Success)
                    {
                        var statResult = _complexReportRepository.DoRegisterEAnswerStats(data.Item1);
                        if (statResult.Success)
                        {
                            ResultText.Append(statResult.Data);
                        }
                        OmsFileName = omsFilename;
                        ResultText.Append("<b>Файл: {0}</b>".F(omsFilename));
                    }
                }
            }
        }

        private void WriteZRegisterAnswer<T>(Tuple<T, RegisterEInfo, int> data) where T : IZRegisterAnswer
        {
            var xmlFilename = Path.Combine(FileName, data.Item2.FileNameXml);
            var omsFilename = Path.Combine(FileName, data.Item2.FileNameOms);

            var result = _dataService.SerializeToFile(xmlFilename, data.Item1, XmlOperations.RemoveEmptyOrNull);
            if (result.Success)
            {
                var packResult = _fileService.Pack(omsFilename, new List<string> { xmlFilename }, false);
                if (packResult.Success)
                {
                    var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                        omsFilename,
                        data.Item2,
                        _id,
                        data.Item1.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                        data.Item1.InnerHeader.Date.HasValue ? data.Item1.InnerHeader.Date.Value : DateTime.Now,
                        Version,
                        TypeLoadEx,
                        data.Item3);
                    if (exchangeResult.Success)
                    {
                        var statResult = _complexReportRepository.DoZRegisterEAnswerStats(data.Item1);
                        if (statResult.Success)
                        {
                            ResultText.Append(statResult.Data);
                        }
                        OmsFileName = omsFilename;
                        ResultText.Append("<b>Файл: {0}</b>".F(omsFilename));
                    }
                }
            }
        }
    }
}