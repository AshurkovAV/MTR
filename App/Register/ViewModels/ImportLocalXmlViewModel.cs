using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Core;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using Medical.DataCore.v21K2.D;
using System.Threading.Tasks;
using System.Threading;
using Autofac;
using Core.Services;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using Medical.DataCore;
using Medical.DataCore.Interface;
using DispatcherHelper = GalaSoft.MvvmLight.Threading.DispatcherHelper;
using v30K1 = Medical.DataCore.v30K1;
using v21K2 = Medical.DataCore.v21K2;
using v31K1 = Medical.DataCore.v31K1;
using v32K1 = Medical.DataCore.v32K1;

namespace Medical.AppLayer.Register.ViewModels
{
    public class ImportLocalXmlViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly IFileService _fileService;
        private readonly INotifyManager _notifyManager;
        private readonly IAppRemoteSettings _remoteSettings;
        private readonly ITextService _textService;
        private readonly IMessageService _messageService;
        private readonly ICommonService _commonService;
        private readonly IDataService _dataService;
        private readonly IComplexReportRepository _complexReportRepository;
        private readonly IMedicineRepository _repository;
        private readonly IProcessingService _processingService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private FileHelpers _fileHelpers = new FileHelpers();

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private RelayCommand _importCommand;
        private RelayCommand _selectFileCommand;
        private RelayCommand _saveLogCommand;

        private string _fileName;
        private string _notes;
        private int _versionLocal;
        private string _typeSchet;


        #region IHash
        public string Hash
        {
            get { return typeof(ImportLocalXmlViewModel).FullName; }
        }
        #endregion

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

        public ImportLocalXmlViewModel(IFileService fileService, 
            INotifyManager notifyManager, 
            ITextService textService, 
            IMessageService messageService,
            ICommonService commonService,
            IDataService dataService,
            IComplexReportRepository complexReportRepository,
            IMedicineRepository repository,
            IProcessingService processingService,
            IDockLayoutManager dockLayoutManager,
            IAppRemoteSettings remoteSettings)
        {
            _fileService = fileService;
            _notifyManager = notifyManager;
            _textService = textService;
            _messageService = messageService;
            _commonService = commonService;
            _dataService = dataService;
            _complexReportRepository = complexReportRepository;
            _repository = repository;
            _processingService = processingService;
            _dockLayoutManager = dockLayoutManager;
            _remoteSettings = remoteSettings;

            _overlayManager = Di.I.Resolve<IOverlayManager>();

            this.ApplyDefaultValues();

            Init();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
            //ProgBar.StyleSettings = new ProgressBarMarqueeStyleSettings();
            //var metropolisDarkTheme = new Theme("Office2013") { IsStandard = true };
            //ProgBar.SetValue(ThemeManager.ThemeNameProperty, metropolisDarkTheme.Name);
            
        }

        public BaseEditStyleSettings styleSet
        {
            get;
            set;
        }

        private void Init()
        {
            AllOptionsList = new ObservableCollection<object>
            {
                new { Value = (int)ProcessingOperations.TortillaPolicyCheck, DisplayName = "Проверить документы ОМС в Тортилле"},
                new { Value = (int)ProcessingOperations.IgnoreScheme, DisplayName = "Игнорировать схему"},
                //new { Value = (int)ProcessingOperations.NotInTerr, DisplayName = "Загрузить только иногородних"},
            };

            var processingResult = _repository.GetEnabledProcessingByScopeAndVersion(Constants.ScopeLocalAccount, Constants.Version21K);
            if (processingResult.Success)
            {
                AllProcessingsList = new ObservableCollection<FactProcessing>(processingResult.Data);
            }

            
        }

        public string FileName {
            get { return _fileName; }
            set { _fileName = value; RaisePropertyChanged(()=>FileName); }
        }
        public StringBuilder ResultText { get; set; }

        [DefaultValue(false)]
        public bool IsTestLoad { get; set; }

        [DefaultValue(TypeLoad.File)]
        public TypeLoad SelectedTypeLoad { get; set; }
        [DefaultValue(0)]
        public int FileExt { get; set; }
        public string Notes {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged(()=>Notes); }
        }
        public int Version { get; set; }
        public int AccountId { get; set; }

        public ObservableCollection<object> AllOptionsList { get; set; }
        public ObservableCollection<FactProcessing> AllProcessingsList { get; set; }
        
        public List<object> SelectedOptionsList { get; set; }
        public List<object> SelectedProcessingsList { get; set; }

        public ICommand SelectFileCommand
        {
            get { return _selectFileCommand ?? (_selectFileCommand = new RelayCommand(SelectFile)); }
        }

        public ICommand ImportCommand
        {
            get { return _importCommand ?? (_importCommand = new RelayCommand(ImportFile, CanImportFile)); }
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

            if (result.IsCanceled)
            {
                _notifyManager.ShowNotify("Операция отменена.");
            }
        }


        private void SelectFile()
        {
            if (SelectedTypeLoad == TypeLoad.File)
            {
                var selectFileResult = _fileService.SelectFile("OMS файлы (.oms)|*.oms|XML файлы (.xml)|*.xml", ".oms");
                if (selectFileResult.Success)
                {
                    FileName = selectFileResult.Data;
                }
            }
            else
            {
                var selectFolderResult = _fileService.SelectFolder("Выберете папку содержащую файлы реестров");
                if (selectFolderResult.Success)
                {
                    FileName = selectFolderResult.Data;
                }
            }
        }

        private void ImportFile()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(()=>_dockLayoutManager.ShowOverlay(Constants.LoadDataMsg, Constants.PleaseWaitMsg));

            var task = _overlayManager.ShowOverlay(Constants.LoadDataMsg, Constants.PleaseWaitMsg);
            task.ContinueWith(t =>
            {
                ResultText = new StringBuilder();
                if (SelectedTypeLoad == TypeLoad.File)
                {
                    //_notifyManager.ShowNotify($"Начало импорта счета {FileName}");
                    ResultText.AppendFormat("<b>Начало импорта счета {0}</b><br>", FileName);
                    DoImportFile(FileName);
                }
                else
                {
                    var extension = FileExt == 0 ? "*.oms" : "*.xml";
                    if (SelectedTypeLoad == TypeLoad.Folder)
                    {
                        var files =
                            new List<string>(Directory.GetFiles(FileName, extension, SearchOption.TopDirectoryOnly));
                        if (files.Count > 0)
                        {
                            //_notifyManager.ShowNotify($"Начало импорта файлов *.oms {FileName}");
                            ResultText.AppendFormat("<b>Начало импорта файлов *.oms {0}</b><br>", FileName);

                            foreach (string file in files)
                            {
                                DoImportFile(file);
                            }

                        }
                        else
                        {
                            ResultText.Append("Файлы не найдены.");
                        }
                    }
                    else
                    {
                        var files =
                            new List<string>(Directory.GetFiles(FileName, extension, SearchOption.AllDirectories));
                        if (files.Count > 0)
                        {
                            //_notifyManager.ShowNotify($"Начало импорта файлов *.oms {FileName}");
                            ResultText.AppendFormat("<b>Начало импорта файлов *.xml {0}</b><br>", FileName);

                            foreach (string file in files)
                            {
                                DoImportFile(file);
                            }
                        }
                        else
                        {
                            ResultText.Append("Файлы не найдены.");
                        }
                    }
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => {
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

        private bool CanImportFile()
        {
            return File.Exists(FileName) || Directory.Exists(FileName);
        }
        private IOverlayManager _overlayManager;
        private void DoImportFile(string fileName)
        {
            _commonService.StartWatch();
            _overlayManager.SetOverlayMessage(Constants.UnpackDataMsg);

            var operations = SelectedOptionsList.ToEnumFlag<ProcessingOperations>();
            ResultText.AppendFormat("<u><br>Начало импорта файла {0}<br></u>", fileName);

            var versionResult = _dataService.GetOmsFileVersion(fileName);
            if (versionResult.HasError)
            {
                ResultText.AppendFormat("Ошибка при определении версии файла OMS<br>{0}", versionResult.LastError);
                return;
            }
            //AppRemoteSettings.ClientMo
            dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
            if ((versionResult.Data == Constants.Version30 || versionResult.Data == Constants.Version31
                                                           || versionResult.Data == Constants.Version32
                 ) && terCode.tf_code != 46)
            {
                versionResult.Data = versionResult.Data - 1;
            }
            switch (Version)
            {
                case 0:
                    _versionLocal = 0;
                    break;
                case 1:
                    _versionLocal = 3;
                    break;
                case 2:
                    _versionLocal = 4;
                    break;
                case 3:
                    _versionLocal = 6;
                    break;
                case 4:
                    _versionLocal = 8;
                    break;
            }

            if (_versionLocal > 0 && versionResult.Data != _versionLocal)
            {
                ResultText.AppendFormat("Выбранная версия не соответствует версии файла OMS<br>Выбрана: {0}, в файле: {1}", Version, versionResult.Data);
                return;
            }
           
            int version = versionResult.Data;

            var loaderResult = _dataService.GetLoaderByVersionD(version);
            if (loaderResult.HasError)
            {
                ResultText.AppendFormat("Ошибка инициализации загрузчика");
                return;
            }
            LoaderBase loader = loaderResult.Data;
           
            var files = new List<string> { fileName };
            var extension = Path.GetExtension(fileName);
            if (extension != null && extension.ToLowerInvariant().Equals(".oms"))
            {
                files = loader.Unpack<RegisterDInfo>(Path.GetFullPath(fileName));
                if (loader.ErrorCount != 0)
                {
                    ResultText.AppendFormat("При распаковке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                    return;
                }
            }
            else
            {
                if (!loader.LoadInfo<RegisterDInfo>(Path.GetFullPath(fileName)))
                {
                    ResultText.AppendFormat("При распаковке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                    return;
                }
            }

            _typeSchet =
                files.FirstOrDefault(p =>
                    p.ToUpperInvariant().Contains("DFM") ||
                    p.ToUpperInvariant().Contains("DPM") ||
                    p.ToUpperInvariant().Contains("DVM") ||
                    p.ToUpperInvariant().Contains("DOM"));

            var hFile =
                files.FirstOrDefault(p => 
                p.ToUpperInvariant().Contains("HM") || 
                p.ToUpperInvariant().Contains("DFM") || 
                p.ToUpperInvariant().Contains("\\CM") ||
                p.ToUpperInvariant().Contains("\\TM")  ||
                p.ToUpperInvariant().Contains("DPM") ||
                p.ToUpperInvariant().Contains("DVM") ||
                p.ToUpperInvariant().Contains("DOM")||
                p.ToUpperInvariant().Contains("DUM"));
            if (hFile.IsNullOrWhiteSpace())
            {
                ResultText.AppendFormat("H файл не найден");
                return;
            }

            var lFile =
                files.FirstOrDefault(p => 
                p.ToUpperInvariant().Contains("LM") || 
                p.ToUpperInvariant().Contains("LFM") || 
                p.ToUpperInvariant().Contains("LCM") ||
                p.ToUpperInvariant().Contains("LTM") ||
                p.ToUpperInvariant().Contains("LPM") ||
                p.ToUpperInvariant().Contains("LVM") ||
                p.ToUpperInvariant().Contains("LOM")||
                p.ToUpperInvariant().Contains("LUM"));
            if (lFile.IsNullOrWhiteSpace())
            {
                ResultText.AppendFormat("L файл не найден");
                return;
            }

            if (!operations.Has(ProcessingOperations.IgnoreScheme))
            {
                _overlayManager.SetOverlayMessage(Constants.ValidationXmlMsg);
                var validateResult = _dataService.ValidateDXml(version, loader,lFile, hFile);
                if (validateResult.HasError)
                {
                    ResultText.AppendFormat(validateResult.Log);
                    return;
                }
            }
            dynamic clientMo = _remoteSettings.Get(AppRemoteSettings.ClientMo);

            


            switch (version)
            {
                case Constants.Version21K:
                    DoImportFileVersion21K2(fileName,hFile, lFile,loader, operations);
                    break;
                case Constants.Version30K:
                    DoImportFileVersion30K1(fileName, hFile, lFile, loader, operations);
                    break;
                case Constants.Version31K:
                    if (clientMo.MO == true)
                    {
                        DoImportFileVersion31MoK1(fileName, hFile, lFile, loader, operations);
                    }
                    else
                    {
                        DoImportFileVersion31K1(fileName, hFile, lFile, loader, operations);
                    }
                    
                    break;
                case Constants.Version32K:
                    DoImportFileVersion32K1(fileName, hFile, lFile, loader, operations);
                    break;

            }
        }

        #region DoImportFileVersion21K2

        private void DoImportFileVersion21K2(string fileName, string hFile, string lFile, LoaderBase loader, ProcessingOperations operations)
        {
            using (var accountStream = new FileStream(hFile, FileMode.Open, FileAccess.Read))
            {
                using (var personStream = new FileStream(lFile, FileMode.Open, FileAccess.Read))
                {
                    Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(accountStream);
                    accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);

                    Stream personStreamCleen = XmlHelpers.RemoveEmptyOrNull(personStream);
                    personStreamCleen = XmlHelpers.ToUpperCase(personStreamCleen);
                    dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);

                    var resultAccount = loader.Load<AccountRegisterD>(accountStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultAccount == null ||
                        resultAccount.Account == null ||
                        resultAccount.Header == null ||
                        resultAccount.RecordsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        return;
                    }

                    var resultPerson = loader.Load<PersonalRegisterD>(personStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultPerson == null ||
                        resultPerson.Header == null ||
                        resultPerson.PersonalsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке персональных данных произошла ошибка<br>{0}",
                            loader.ErrorsAggregate);
                        return;
                    }

                    _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseMsg);
                    var resultLoad = _dataService.Load(resultAccount, resultPerson, IsTestLoad, operations);
                    if (resultLoad.Success)
                    {
                        _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseDoneMsg);
                        if (!IsTestLoad)
                        {
                            AccountId = resultLoad.Data;
                            string dest = Convert.ToString(terCode.tf_code);
                            var registerDInfo = new RegisterDInfo();
                            var packetResult =
                                _repository.GetRegisterDLastPacketNumber(resultAccount.Account.MedicalOrganizationCode, dest);
                            registerDInfo.SetInfo("HM{0}T{1}{2}{3:D2}{4:D4}".F(
                                resultAccount.Account.MedicalOrganizationCode,
                                dest,
                                resultAccount.Account.Year,
                                resultAccount.Account.Month,
                                packetResult.Data));

                            if (registerDInfo.Errors.IsNotNullOrWhiteSpace())
                            {
                                _messageService.ShowErrorFormatted("Имя файла информационного обмена неверное.\r\n{0}",
                                    registerDInfo.Errors);
                                //TODO log
                            }

                            WriteRegister(Tuple.Create(fileName, resultAccount, registerDInfo, (int) DirectionType.In));

                            if (operations.Has(ProcessingOperations.TortillaPolicyCheck))
                            {
                                _overlayManager.SetOverlayMessage(Constants.CheckPolicyInTortillaMsg);
                                var checkResult = _processingService.CheckPolicyInTortilla(AccountId, false);
                                if (checkResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Проверка полисов в Tortila выполнена.</b><br>");
                                    if (checkResult.Data.Any())
                                    {
                                        var applyResult = _processingService.ApplyPolicyFromTortilla(checkResult.Data);
                                        if (applyResult.Success)
                                        {
                                            ResultText.AppendFormat("Изменено {0} полисов<br>", checkResult.Data.Count());
                                            foreach (var policy in checkResult.Data)
                                            {
                                                ResultText.AppendFormat(
                                                    "ID пациента {0}, ЕНП {1}, ОКАТО территории страхования {2}, комментарии {3}<br>",
                                                    policy.PatientId,
                                                    policy.INP,
                                                    policy.TerritoryOkato,
                                                    policy.Comments);
                                            }
                                        }
                                        else
                                        {
                                            ResultText.AppendFormat("Ошибка изменения данных полисов : {0}<br>",
                                                applyResult.LastError);
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }

                            if (SelectedProcessingsList.IsNotNull() && SelectedProcessingsList.Any())
                            {
                                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                                var processingResult = _processingService.RunProcessing(AccountId,
                                    Constants.ScopeLocalAccount, Constants.Version21K, SelectedProcessingsList);
                                if (processingResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Обработка данных выполнена.</b><br>");
                                    if (processingResult.Data.Any() && processingResult.Data.Any(p => p.Affected > 0))
                                    {
                                        foreach (var result in processingResult.Data)
                                        {
                                            if (result.Affected > 0)
                                            {
                                                ResultText.AppendFormat("{0} - {1}<br>", result.Name, result.Affected);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var statResult = _complexReportRepository.DoRegisterDLoadStats(resultAccount);
                            if (statResult.Success)
                            {
                                ResultText.Append(statResult.Data);
                            }
                            ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}",
                            resultLoad.ErrorMessage);
                        return;
                    }
                }
            }
            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
        }

        #endregion

        #region DoImportFileVersion30K1
        private void DoImportFileVersion30K1(string fileName, string hFile, string lFile, LoaderBase loader, ProcessingOperations operations)
        {
            using (var accountStream = new FileStream(hFile, FileMode.Open, FileAccess.Read))
            {
                using (var personStream = new FileStream(lFile, FileMode.Open, FileAccess.Read))
                {
                    Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(accountStream);
                    accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);

                    Stream personStreamCleen = XmlHelpers.RemoveEmptyOrNull(personStream);
                    personStreamCleen = XmlHelpers.ToUpperCase(personStreamCleen);
                    dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);

                    var resultAccount = loader.Load<v30K1.D.AccountRegisterD>(accountStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultAccount == null ||
                        resultAccount.Account == null ||
                        resultAccount.Header == null ||
                        resultAccount.RecordsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    var resultPerson = loader.Load<v30K1.D.PersonalRegisterD>(personStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultPerson == null ||
                        resultPerson.Header == null ||
                        resultPerson.PersonalsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке персональных данных произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseMsg);
                    var resultLoad = _dataService.LoadZ(resultAccount, resultPerson, IsTestLoad, operations);
                    if (resultLoad.Success)
                    {
                        _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseDoneMsg);
                        if (!IsTestLoad)
                        {
                            AccountId = resultLoad.Data;

                            string dest = Convert.ToString(terCode.tf_code);

                            var registerDInfo = new RegisterDInfo();
                            var packetResult = _repository.GetRegisterDLastPacketNumber(resultAccount.Account.MedicalOrganizationCode, dest);
                            registerDInfo.SetInfo("HM{0}T{1}{2}{3:D2}{4:D4}".F(
                                resultAccount.Account.MedicalOrganizationCode,
                                dest,
                                resultAccount.Account.Year,
                                resultAccount.Account.Month,
                                packetResult.Data));

                            if (registerDInfo.Errors.IsNotNullOrWhiteSpace())
                            {
                                _messageService.ShowErrorFormatted("Имя файла информационного обмена неверное.\r\n{0}", registerDInfo.Errors);
                                //TODO log
                            }

                            WriteRegister(Tuple.Create(fileName, resultAccount, registerDInfo, (int)DirectionType.In));

                            if (operations.Has(ProcessingOperations.TortillaPolicyCheck))
                            {
                                _overlayManager.SetOverlayMessage(Constants.CheckPolicyInTortillaMsg);
                                var checkResult = _processingService.CheckZPolicyInTortilla(AccountId, false);
                                if (checkResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Проверка полисов в Tortila выполнена.</b><br>");
                                    if (checkResult.Data.Any())
                                    {
                                        var applyResult = _processingService.ApplyPolicyFromTortilla(checkResult.Data);
                                        if (applyResult.Success)
                                        {
                                            ResultText.AppendFormat("Изменено {0} полисов<br>", checkResult.Data.Count());
                                            foreach (var policy in checkResult.Data)
                                            {
                                                ResultText.AppendFormat("ID пациента {0}, ЕНП {1}, ОКАТО территории страхования {2}, комментарии {3}<br>",
                                                    policy.PatientId,
                                                    policy.INP,
                                                    policy.TerritoryOkato,
                                                    policy.Comments);
                                            }
                                        }
                                        else
                                        {
                                            ResultText.AppendFormat("Ошибка изменения данных полисов : {0}<br>", applyResult.LastError);
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }

                            if (SelectedProcessingsList.IsNotNull() && SelectedProcessingsList.Any())
                            {
                                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                                var processingResult = _processingService.RunProcessing(AccountId, Constants.ScopeLocalAccount, Constants.Version30K, SelectedProcessingsList);
                                if (processingResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Обработка данных выполнена.</b><br>");
                                    if (processingResult.Data.Any() && processingResult.Data.Any(p => p.Affected > 0))
                                    {
                                        foreach (var result in processingResult.Data)
                                        {
                                            if (result.Affected > 0)
                                            {
                                                ResultText.AppendFormat("{0} - {1}<br>", result.Name, result.Affected);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var statResult = _complexReportRepository.DoRegisterDLoadStats(resultAccount);
                            if (statResult.Success)
                            {
                                ResultText.Append(statResult.Data);
                            }
                            ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.ErrorMessage);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                }
            }
            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
            DelFile(hFile, lFile, fileName);
        }

        #endregion

        #region DoImportFileVersion31K1
        private void DoImportFileVersion31K1(string fileName, string hFile, string lFile, LoaderBase loader, ProcessingOperations operations)
        {
            using (var accountStream = new FileStream(hFile, FileMode.Open, FileAccess.Read))
            {
                using (var personStream = new FileStream(lFile, FileMode.Open, FileAccess.Read))
                {
                    Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(accountStream);
                    accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);
                    var inter = SelectedOptionsList.ToEnumFlag<ProcessingOperations>();

                    Stream personStreamCleen = XmlHelpers.RemoveEmptyOrNull(personStream);
                    personStreamCleen = XmlHelpers.ToUpperCase(personStreamCleen);
                    dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                    
                    var resultAccount = loader.Load<v31K1.D.AccountRegisterD>(accountStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultAccount == null ||
                        resultAccount.Account == null ||
                        resultAccount.Header == null ||
                        resultAccount.RecordsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                    if (resultAccount.Account.MedicalOrganizationCode.IsNull())
                    {
                        ResultText.AppendFormat("Код медицинской организации не определен либо не найден в кэше");
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    var resultPerson = loader.Load<v31K1.D.PersonalRegisterD>(personStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultPerson == null ||
                        resultPerson.Header == null ||
                        resultPerson.PersonalsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке персональных данных произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseMsg);
                    var resultLoad = _dataService.LoadZ(resultAccount, resultPerson, IsTestLoad, operations, inter);
                    if (resultLoad.Success)
                    {
                        _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseDoneMsg);
                        if (!IsTestLoad)
                        {
                            AccountId = resultLoad.Data;

                            string dest = Convert.ToString(terCode.tf_code);

                            var registerDInfo = new RegisterDInfo();
                            var packetResult = _repository.GetRegisterDLastPacketNumber(resultAccount.Account.MedicalOrganizationCode, dest);
                            registerDInfo.SetInfo("HM{0}T{1}{2}{3:D2}{4:D4}".F(
                                resultAccount.Account.MedicalOrganizationCode,
                                dest,
                                resultAccount.Account.Year,
                                resultAccount.Account.Month,
                                packetResult.Data));

                            if (registerDInfo.Errors.IsNotNullOrWhiteSpace())
                            {
                                _messageService.ShowErrorFormatted("Имя файла информационного обмена неверное.\r\n{0}", registerDInfo.Errors);
                                DelFile(hFile, lFile, fileName);
                                //TODO log
                            }

                            WriteRegister(Tuple.Create(fileName, resultAccount, registerDInfo, (int)DirectionType.In));

                            if (operations.Has(ProcessingOperations.TortillaPolicyCheck))
                            {
                                _overlayManager.SetOverlayMessage(Constants.CheckPolicyInTortillaMsg);
                                var checkResult = _processingService.CheckZPolicyInTortilla(AccountId, false);
                                if (checkResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Проверка полисов в Tortila выполнена.</b><br>");
                                    if (checkResult.Data.Any())
                                    {
                                        var applyResult = _processingService.ApplyPolicyFromTortilla(checkResult.Data);
                                        if (applyResult.Success)
                                        {
                                            ResultText.AppendFormat("Изменено {0} полисов<br>", checkResult.Data.Count());
                                            foreach (var policy in checkResult.Data)
                                            {
                                                ResultText.AppendFormat("ID пациента {0}, ЕНП {1}, ОКАТО территории страхования {2}, комментарии {3}<br>",
                                                    policy.PatientId,
                                                    policy.INP,
                                                    policy.TerritoryOkato,
                                                    policy.Comments);
                                            }
                                        }
                                        else
                                        {
                                            ResultText.AppendFormat("Ошибка изменения данных полисов : {0}<br>", applyResult.LastError);
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }

                            if (SelectedProcessingsList.IsNotNull() && SelectedProcessingsList.Any())
                            {
                                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                                var processingResult = _processingService.RunProcessing(AccountId, Constants.ScopeLocalAccount, Constants.Version31K, SelectedProcessingsList);
                                if (processingResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Обработка данных выполнена.</b><br>");
                                    if (processingResult.Data.Any() && processingResult.Data.Any(p => p.Affected > 0))
                                    {
                                        foreach (var result in processingResult.Data)
                                        {
                                            if (result.Affected > 0)
                                            {
                                                ResultText.AppendFormat("{0} - {1}<br>", result.Name, result.Affected);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var statResult = _complexReportRepository.DoRegisterDLoadStats(resultAccount);
                            if (statResult.Success)
                            {
                                ResultText.Append(statResult.Data);
                            }
                            ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.ErrorMessage);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                }
            }
            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
            DelFile(hFile, lFile, fileName);
        }

        #endregion

        #region DoImportFileVersion31MoK1
        private void DoImportFileVersion31MoK1(string fileName, string hFile, string lFile, LoaderBase loader, ProcessingOperations operations)
        {
            using (var accountStream = new FileStream(hFile, FileMode.Open, FileAccess.Read))
            {
                using (var personStream = new FileStream(lFile, FileMode.Open, FileAccess.Read))
                {
                    Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(accountStream);
                    accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);
                    var inter = SelectedOptionsList.ToEnumFlag<ProcessingOperations>();

                    Stream personStreamCleen = XmlHelpers.RemoveEmptyOrNull(personStream);
                    personStreamCleen = XmlHelpers.ToUpperCase(personStreamCleen);
                    dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);

                    var resultAccount = loader.Load<v31K1.DV.AccountRegisterD>(accountStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultAccount == null ||
                        resultAccount.Account == null ||
                        resultAccount.Header == null ||
                        resultAccount.RecordsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                    if (resultAccount.Account.MedicalOrganizationCode.IsNull())
                    {
                        ResultText.AppendFormat("Код медицинской организации не определен либо не найден в кэше");
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    var resultPerson = loader.Load<v31K1.DV.PersonalRegisterD>(personStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultPerson == null ||
                        resultPerson.Header == null ||
                        resultPerson.PersonalsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке персональных данных произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseMsg);
                    var resultLoad = _dataService.LoadZ(resultAccount, resultPerson, IsTestLoad, operations, inter);
                    if (resultLoad.Success)
                    {
                        _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseDoneMsg);
                        if (!IsTestLoad)
                        {
                            AccountId = resultLoad.Data;

                            string dest = Convert.ToString(terCode.tf_code);

                            var registerDInfo = new RegisterDInfo();
                            var packetResult = _repository.GetRegisterDLastPacketNumber(resultAccount.Account.MedicalOrganizationCode, dest);
                            registerDInfo.SetInfo("HM{0}T{1}{2}{3:D2}{4:D4}".F(
                                resultAccount.Account.MedicalOrganizationCode,
                                dest,
                                resultAccount.Account.Year,
                                resultAccount.Account.Month,
                                packetResult.Data));

                            if (registerDInfo.Errors.IsNotNullOrWhiteSpace())
                            {
                                _messageService.ShowErrorFormatted("Имя файла информационного обмена неверное.\r\n{0}", registerDInfo.Errors);
                                DelFile(hFile, lFile, fileName);
                                //TODO log
                            }

                            WriteRegister(Tuple.Create(fileName, resultAccount, registerDInfo, (int)DirectionType.In));

                            if (operations.Has(ProcessingOperations.TortillaPolicyCheck))
                            {
                                _overlayManager.SetOverlayMessage(Constants.CheckPolicyInTortillaMsg);
                                var checkResult = _processingService.CheckZPolicyInTortilla(AccountId, false);
                                if (checkResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Проверка полисов в Tortila выполнена.</b><br>");
                                    if (checkResult.Data.Any())
                                    {
                                        var applyResult = _processingService.ApplyPolicyFromTortilla(checkResult.Data);
                                        if (applyResult.Success)
                                        {
                                            ResultText.AppendFormat("Изменено {0} полисов<br>", checkResult.Data.Count());
                                            foreach (var policy in checkResult.Data)
                                            {
                                                ResultText.AppendFormat("ID пациента {0}, ЕНП {1}, ОКАТО территории страхования {2}, комментарии {3}<br>",
                                                    policy.PatientId,
                                                    policy.INP,
                                                    policy.TerritoryOkato,
                                                    policy.Comments);
                                            }
                                        }
                                        else
                                        {
                                            ResultText.AppendFormat("Ошибка изменения данных полисов : {0}<br>", applyResult.LastError);
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }

                            if (SelectedProcessingsList.IsNotNull() && SelectedProcessingsList.Any())
                            {
                                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                                var processingResult = _processingService.RunProcessing(AccountId, Constants.ScopeLocalAccount, Constants.Version31K, SelectedProcessingsList);
                                if (processingResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Обработка данных выполнена.</b><br>");
                                    if (processingResult.Data.Any() && processingResult.Data.Any(p => p.Affected > 0))
                                    {
                                        foreach (var result in processingResult.Data)
                                        {
                                            if (result.Affected > 0)
                                            {
                                                ResultText.AppendFormat("{0} - {1}<br>", result.Name, result.Affected);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var statResult = _complexReportRepository.DoRegisterDLoadStats(resultAccount);
                            if (statResult.Success)
                            {
                                ResultText.Append(statResult.Data);
                            }
                            ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.ErrorMessage);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                }
            }
            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
            DelFile(hFile, lFile, fileName);
        }

        #endregion

        #region DoImportFileVersion32K1
        private void DoImportFileVersion32K1(string fileName, string hFile, string lFile, LoaderBase loader, ProcessingOperations operations)
        {
            using (var accountStream = new FileStream(hFile, FileMode.Open, FileAccess.Read))
            {
                using (var personStream = new FileStream(lFile, FileMode.Open, FileAccess.Read))
                {
                    Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(accountStream);
                    accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);
                    var inter = SelectedOptionsList.ToEnumFlag<ProcessingOperations>();

                    Stream personStreamCleen = XmlHelpers.RemoveEmptyOrNull(personStream);
                    personStreamCleen = XmlHelpers.ToUpperCase(personStreamCleen);
                    dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);

                    var resultAccount = loader.Load<v32K1.D.AccountRegisterD>(accountStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultAccount == null ||
                        resultAccount.Account == null ||
                        resultAccount.Header == null ||
                        resultAccount.RecordsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                    if (resultAccount.Account.MedicalOrganizationCode.IsNull())
                    {
                        ResultText.AppendFormat("Код медицинской организации не определен либо не найден в кэше");
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    var resultPerson = loader.Load<v32K1.D.PersonalRegisterD>(personStreamCleen);
                    if (loader.ErrorCount != 0 ||
                        resultPerson == null ||
                        resultPerson.Header == null ||
                        resultPerson.PersonalsCollection.Count == 0)
                    {
                        ResultText.AppendFormat("При обработке персональных данных произошла ошибка<br>{0}", loader.ErrorsAggregate);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }

                    _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseMsg);
                    var resultLoad = _dataService.LoadZ(resultAccount, resultPerson, IsTestLoad, operations, inter);
                    if (resultLoad.Success)
                    {
                        _overlayManager.SetOverlayMessage(Constants.SaveToDatabaseDoneMsg);
                        if (!IsTestLoad)
                        {
                            AccountId = resultLoad.Data;

                            string dest = Convert.ToString(terCode.tf_code);

                            var registerDInfo = new RegisterDInfo();
                            var packetResult = _repository.GetRegisterDLastPacketNumber(resultAccount.Account.MedicalOrganizationCode, dest);
                            registerDInfo.SetInfo("HM{0}T{1}{2}{3:D2}{4:D4}".F(
                                resultAccount.Account.MedicalOrganizationCode,
                                dest,
                                resultAccount.Account.Year,
                                resultAccount.Account.Month,
                                packetResult.Data));

                            if (registerDInfo.Errors.IsNotNullOrWhiteSpace())
                            {
                                _messageService.ShowErrorFormatted("Имя файла информационного обмена неверное.\r\n{0}", registerDInfo.Errors);
                                DelFile(hFile, lFile, fileName);
                                //TODO log
                            }

                            WriteRegister(Tuple.Create(fileName, resultAccount, registerDInfo, (int)DirectionType.In));

                            if (operations.Has(ProcessingOperations.TortillaPolicyCheck))
                            {
                                _overlayManager.SetOverlayMessage(Constants.CheckPolicyInTortillaMsg);
                                var checkResult = _processingService.CheckZPolicyInTortilla(AccountId, false);
                                if (checkResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Проверка полисов в Tortila выполнена.</b><br>");
                                    if (checkResult.Data.Any())
                                    {
                                        var applyResult = _processingService.ApplyPolicyFromTortilla(checkResult.Data);
                                        if (applyResult.Success)
                                        {
                                            ResultText.AppendFormat("Изменено {0} полисов<br>", checkResult.Data.Count());
                                            foreach (var policy in checkResult.Data)
                                            {
                                                ResultText.AppendFormat("ID пациента {0}, ЕНП {1}, ОКАТО территории страхования {2}, комментарии {3}<br>",
                                                    policy.PatientId,
                                                    policy.INP,
                                                    policy.TerritoryOkato,
                                                    policy.Comments);
                                            }
                                        }
                                        else
                                        {
                                            ResultText.AppendFormat("Ошибка изменения данных полисов : {0}<br>", applyResult.LastError);
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }

                            if (SelectedProcessingsList.IsNotNull() && SelectedProcessingsList.Any())
                            {
                                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                                var processingResult = _processingService.RunProcessing(AccountId, Constants.ScopeLocalAccount, Constants.Version31K, SelectedProcessingsList);
                                if (processingResult.Success)
                                {
                                    ResultText.AppendFormat("<b>Обработка данных выполнена.</b><br>");
                                    if (processingResult.Data.Any() && processingResult.Data.Any(p => p.Affected > 0))
                                    {
                                        foreach (var result in processingResult.Data)
                                        {
                                            if (result.Affected > 0)
                                            {
                                                ResultText.AppendFormat("{0} - {1}<br>", result.Name, result.Affected);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var statResult = _complexReportRepository.DoRegisterDLoadStats(resultAccount);
                            if (statResult.Success)
                            {
                                ResultText.Append(statResult.Data);
                            }
                            ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.ErrorMessage);
                        DelFile(hFile, lFile, fileName);
                        return;
                    }
                }
            }
            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
            DelFile(hFile, lFile, fileName);
        }

        #endregion


        private void DelFile(string hFile, string lFile, string fileName)
        {
            dynamic v = _remoteSettings.Get(AppRemoteSettings.FileXmlOms);
            bool fileoms = v.delOms;
            bool filexml = v.delXml;
            if (filexml)
            {
                _fileHelpers.DeleteFile(hFile);
                _fileHelpers.DeleteFile(lFile);
            }
            if (fileoms)
            {
                _fileHelpers.DeleteFile(fileName);
            }
        }
        private void WriteRegister(Tuple<string,AccountRegisterD, RegisterDInfo, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3, 
                AccountId,
                data.Item2.RecordsCollection.SelectMany(p => p.EventCollection).Count(),
                data.Item2.Header.Date.HasValue ? data.Item2.Header.Date.Value : DateTime.Now,
                Constants.Version21K,
                (int)AccountType.GeneralPartMo,
                data.Item4);

            if (exchangeResult.Success)
            {
                var statResult = _complexReportRepository.DoRegisterDLoadStats(data.Item2);
                if (statResult.Success)
                {
                    ResultText.Append(statResult.Data);
                }
                
                _notifyManager.ShowNotify("Данные успешно загружены.");
            }
        }

        private void WriteRegister(Tuple<string, v30K1.D.AccountRegisterD, RegisterDInfo, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.RecordsCollection.SelectMany(p => p.EventCollection).Count(),
                data.Item2.Header.Date.HasValue ? data.Item2.Header.Date.Value : DateTime.Now,
                Constants.Version30K,
                (int)AccountType.GeneralPartMo,
                data.Item4);

            if (exchangeResult.Success)
            {
                var statResult = _complexReportRepository.DoRegisterDLoadStats(data.Item2);
                if (statResult.Success)
                {
                    ResultText.Append(statResult.Data);
                }

                _notifyManager.ShowNotify("Данные успешно загружены.");
            }
        }

        private void WriteRegister(Tuple<string, v31K1.D.AccountRegisterD, RegisterDInfo, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.RecordsCollection.SelectMany(p => p.EventCollection).Count(),
                data.Item2.Header.Date.HasValue ? data.Item2.Header.Date.Value : DateTime.Now,
                Constants.Version31K,
                (int)AccountType.GeneralPartMo,
                data.Item4);

            if (exchangeResult.Success)
            {
                var statResult = _complexReportRepository.DoRegisterDLoadStats(data.Item2);
                if (statResult.Success)
                {
                    ResultText.Append(statResult.Data);
                }

                _notifyManager.ShowNotify("Данные успешно загружены.");
            }
        }

        private void WriteRegister(Tuple<string, v31K1.DV.AccountRegisterD, RegisterDInfo, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.RecordsCollection.SelectMany(p => p.EventCollection).Count(),
                data.Item2.Header.Date.HasValue ? data.Item2.Header.Date.Value : DateTime.Now,
                Constants.Version31K,
                (int)AccountType.GeneralPartMo,
                data.Item4);

            if (exchangeResult.Success)
            {
                var statResult = _complexReportRepository.DoRegisterDLoadStats(data.Item2);
                if (statResult.Success)
                {
                    ResultText.Append(statResult.Data);
                }

                _notifyManager.ShowNotify("Данные успешно загружены.");
            }
        }

        private void WriteRegister(Tuple<string, v32K1.D.AccountRegisterD, RegisterDInfo, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.RecordsCollection.SelectMany(p => p.EventCollection).Count(),
                data.Item2.Header.Date.HasValue ? data.Item2.Header.Date.Value : DateTime.Now,
                Constants.Version32K,
                (int)AccountType.GeneralPartMo,
                data.Item4);

            if (exchangeResult.Success)
            {
                var statResult = _complexReportRepository.DoRegisterDLoadStats(data.Item2);
                if (statResult.Success)
                {
                    ResultText.Append(statResult.Data);
                }

                _notifyManager.ShowNotify("Данные успешно загружены.");
            }
        }

        public string Error
        {
            get
            {
                RaisePropertyChanged("Visibility");
                return _dataErrorInfoSupport.Error;
            }
        }
    }
}