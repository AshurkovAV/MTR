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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore;
using Medical.DataCore.Interface;
using v10 = Medical.DataCore.v10;
using v21 = Medical.DataCore.v21;
using v30 = Medical.DataCore.v30;
using v31 = Medical.DataCore.v31;
using v32 = Medical.DataCore.v32;
using Core.DataStructure;
using System.Threading.Tasks;
using Autofac;
using Core.Services;
using GalaSoft.MvvmLight.Threading;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    public class ImportXmlViewModel : ViewModelBase, IHash, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        public static IAppRemoteSettings _remoteSettings;

        private readonly IFileService _fileService;
        private readonly INotifyManager _notifyManager;
        private readonly ITextService _textService;
        private readonly IMessageService _messageService;
        private readonly ICommonService _commonService;
        private readonly IDataService _dataService;
        private readonly IComplexReportRepository _complexReportRepository;
        private readonly IDockLayoutManager _dockLayoutManager;
        private IOverlayManager _overlayManager;
        private readonly IExaminationService _examinationService;
        private readonly ICacheRepository _cacheRepository;
        private readonly IProcessingService _processingService;
        private FileHelpers _fileHelpers = new FileHelpers();

        private RelayCommand _importCommand;
        private RelayCommand _selectFileCommand;
        private RelayCommand _saveLogCommand;
        private int _versionLoadFiles;

        private string _fileName;
        private string _notes;

        #region IHash
        public string Hash
        {
            get { return typeof(ImportXmlViewModel).FullName; }
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

        public ImportXmlViewModel(IFileService fileService, 
            INotifyManager notifyManager, 
            ITextService textService, 
            IMessageService messageService,
            ICommonService commonService,
            IDataService dataService,
            IComplexReportRepository complexReportRepository,
            IDockLayoutManager dockLayoutManager,
            IExaminationService examinationService,
            ICacheRepository cacheRepository,
            IProcessingService processingService,
            string fileName = null)
        {
            FileName = fileName;

            _fileService = fileService;
            _notifyManager = notifyManager;
            _textService = textService;
            _messageService = messageService;
            _commonService = commonService;
            _dataService = dataService;
            _complexReportRepository = complexReportRepository;
            _examinationService = examinationService;
            _dockLayoutManager = dockLayoutManager;
            _cacheRepository = cacheRepository;
            _processingService = processingService;
            _overlayManager = Di.I.Resolve<IOverlayManager>();
            _remoteSettings = Di.I.Resolve<IAppRemoteSettings>();

            Init();
            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Init()
        {
            AllOptionsList = new ObservableCollection<CommonTuple>
            {
                new CommonTuple{ ValueField = (int)ProcessingOperations.PrimaryCheck, DisplayField = "Провести первичный МЭК"},
                new CommonTuple{ ValueField = (int)ProcessingOperations.PolicyCheck, DisplayField = "Проверить документы ОМС"},
                new CommonTuple{ ValueField = (int)ProcessingOperations.DuplicateCheck, DisplayField = "Проверить дублирование"},
                new CommonTuple{ ValueField = (int)ProcessingOperations.IgnoreScheme, DisplayField = "Игнорировать схему"}
            };

            this.ApplyDefaultValues();
        }

        public string FileName {
            get { return _fileName; }
            set { _fileName = value; RaisePropertyChanged(()=>FileName); }
        }
        public StringBuilder ResultText { get; set; }
        public bool IsTestLoad { get; set; }

        [DefaultValue(TypeLoad.File)]
        public TypeLoad SelectedTypeLoad { get; set; }
        public int FileExt { get; set; }
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged(() => Notes); }
        }
        public int AccountId { get; set; }
        public int Version { get; set; }

        public ObservableCollection<CommonTuple> AllOptionsList { get; set; }
        public List<object> SelectedOptionsList { get; set; }

        #region Commands
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
        #endregion

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
            var task = _overlayManager.ShowOverlay(Constants.LoadDataMsg, Constants.PleaseWaitMsg);
            task.ContinueWith(t => _dockLayoutManager.ShowOverlay(Constants.LoadDataMsg, Constants.PleaseWaitMsg))
                .ContinueWith(p =>
                {
                    ResultText = new StringBuilder();
                    if (SelectedTypeLoad == TypeLoad.File)
                    {
                       // _notifyManager.ShowNotify($"Начало импорта счета {FileName}");
                        ResultText.AppendFormat("<b>Начало импорта счета {0}</b><br>", FileName);
                        DoImportFile(FileName);
                    }
                    else
                    {
                        var extension = FileExt == 0 ? "*.oms" : "*.xml";
                        if (SelectedTypeLoad == TypeLoad.Folder)
                        {
                            var files = new List<string>(Directory.GetFiles(FileName, extension, SearchOption.TopDirectoryOnly));
                            if (files.Count > 0)
                            {
                               // _notifyManager.ShowNotify($"Начало импорта файлов *.oms {FileName}");
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
                            var files = new List<string>(Directory.GetFiles(FileName, extension, SearchOption.AllDirectories));
                            if (files.Count > 0)
                            {
                               // _notifyManager.ShowNotify($"Начало импорта файлов *.xml {FileName}");
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

                }).ContinueWith(p => _overlayManager.HideOverlay());
        }

        private bool CanImportFile()
        {
            return File.Exists(FileName) || Directory.Exists(FileName);
        }

        private void DoImportFile(string fileName)
        {
            _commonService.StartWatch();
            var operations = SelectedOptionsList.ToEnumFlag<ProcessingOperations>();
            _overlayManager.SetOverlayMessage(Constants.UnpackDataMsg);
            ResultText.AppendFormat("<u><br>Начало импорта файла {0}<br></u>", fileName);

            var versionResult = _dataService.GetOmsFileVersion(fileName);
            if (versionResult.HasError)
            {
                ResultText.AppendFormat("Ошибка при определении версии файла OMS<br>{0}", versionResult.LastError);
                return;
            }

            var versionLocResult = _dataService.GetVersion(Version);
            if (versionLocResult.HasError)
            {
                ResultText.AppendFormat("Ошибка при определении выбранной версии на форме<br>{0}", versionLocResult.LastError);
                return;
            }
            if (Version > 0 && versionResult.Data != versionLocResult.Data)
            {
                ResultText.AppendFormat("Выбранная версия не соответствует версии файла OMS<br>Выбрана: {0}, в файле: {1}", Version, versionResult.Data);
                return;
            }

            int version = versionResult.Data;
            _versionLoadFiles = versionResult.Data;

            var loaderResult = _dataService.GetLoaderByVersion(version);
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
                files = loader.Unpack<RegisterEInfo>(Path.GetFullPath(fileName));
                if (loader.ErrorCount != 0)
                {
                    ResultText.AppendFormat("При распаковке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                    return;
                }
            }
            else
            {
                if (!loader.LoadInfo<RegisterEInfo>(Path.GetFullPath(fileName)))
                {
                    ResultText.AppendFormat("При распаковке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                    return;
                }
            }

            if (!files.Any())
            {
                ResultText.AppendFormat("Отсутствует имя XML файла<br>");
                return;
            }

            var file = files.First();

            if (!operations.Has(ProcessingOperations.IgnoreScheme))
            {
                _overlayManager.SetOverlayMessage(Constants.ValidationXmlMsg);
                var validateResult = _dataService.ValidateXml(version, loader, file);
                if (validateResult.HasError)
                {
                    ResultText.AppendFormat(validateResult.Log);
                    return;
                }
            }
            switch (version)
            {
                case Constants.Version21:
                    DoImportFileVersion21(file, loader, operations, fileName, version);
                    break;
                case Constants.Version30:
                    DoImportFileVersion30(file, loader, operations, fileName, version);
                    break;
                case Constants.Version31:
                    DoImportFileVersion31(file, loader, operations, fileName, version);
                    break;
                case Constants.Version32:
                    DoImportFileVersion31(file, loader, operations, fileName, version);
                    break;

            }


            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
        }

        private void DoImportFileVersion21(string file, LoaderBase loader, ProcessingOperations operations, string fileName, int version)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(stream);
                accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);

                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                dynamic result = null;
                switch (((RegisterEInfo)loader.Info).Type.ToLowerInvariant())
                {
                    case "r":
                        switch (version)
                        {
                            case 1:
                                //result = loader.Load<v10.E.RegisterE>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case 2:
                                result = loader.Load<v21.E.RegisterE>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            result == null ||
                            result.Account == null ||
                            result.Header == null ||
                            result.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                            return;
                        }

                        var resultLoad = _dataService.Load(result, loader.Info.NumberXXXX, IsTestLoad, operations);
                        if (resultLoad.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoad.Data;

                                if (operations.Has(ProcessingOperations.PrimaryCheck))
                                {
                                    RunPrimaryCheck();
                                }

                                if (operations.Has(ProcessingOperations.DuplicateCheck))
                                {
                                    RunDuplicationCheck();
                                }

                                if (operations.Has(ProcessingOperations.PolicyCheck))
                                {
                                    RunPolicyCheck();

                                }

                                WriteRegister(Tuple.Create(fileName, result as IRegister, loader.Info as RegisterEInfo, (int)AccountType.GeneralPart, (int)DirectionType.In, version));
                                DoStats(result);
                                DoDbStats(AccountId);
                                ResultText.AppendFormat("<b>Реестр успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(result);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.LastError);
                            return;
                        }
                        break;
                    case "d":
                        dynamic resultBack = null;
                        switch (version)
                        {
                            case 1:
                                //resultBack = loader.Load<v10.E.RegisterE>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case 2:
                                resultBack = loader.Load<v21.E.RegisterE>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            resultBack == null ||
                            resultBack.Account == null ||
                            resultBack.Header == null ||
                            resultBack.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                            return;
                        }

                        var resultLoadBack = _dataService.LoadBack(resultBack, loader.Info.NumberXXXX, IsTestLoad, operations);
                        if (resultLoadBack.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoadBack.Data;//ashurkova

                                if (operations.Has(ProcessingOperations.PrimaryCheck))
                                {
                                    RunPrimaryCheck();
                                }

                                if (operations.Has(ProcessingOperations.PolicyCheck))
                                {
                                    RunPolicyCheck();
                                }

                                WriteRegister(Tuple.Create(fileName, resultBack as IRegister, loader.Info as RegisterEInfo, (int)AccountType.CorrectedPart, (int)DirectionType.In, version));
                                DoStats(resultBack);
                                DoDbStats(AccountId);
                                ResultText.AppendFormat("<b>Реестр(исправленная часть) успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(resultBack);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoadBack.LastError);
                            return;
                        }
                        break;
                    case "a":
                        dynamic resultAnswer = null;
                        switch (version)
                        {
                            case 1:
                                //resultAnswer = loader.Load<v10.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case 2:
                                resultAnswer = loader.Load<v21.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            resultAnswer == null ||
                            resultAnswer.Account == null ||
                            resultAnswer.Header == null ||
                            resultAnswer.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке протокола произошла ошибка<br>{0}", loader.ErrorsAggregate);

                            return;
                        }

                        var resultLoadAnswer = _dataService.LoadAnswer(resultAnswer, loader.Info as RegisterEInfo, IsTestLoad, operations);
                        if (resultLoadAnswer.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoadAnswer.Data; //ashurkova

                                WriteRegister(Tuple.Create(fileName, resultAnswer as IRegisterAnswer, loader.Info as RegisterEInfo, (int)AccountType.LogPart, (int)DirectionType.In, version));
                                DoStats(resultAnswer);
                                DoDbStats(AccountId);
                                ResultText.AppendFormat("<b>Реестр(протокол обработки) успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(resultAnswer);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoadAnswer.LastError);
                            return;
                        }
                        break;
                    default:
                        ResultText.AppendFormat("Неизвестный тип файла xml {0}<br>", ((RegisterEInfo)loader.Info).Type);
                        return;
                }
            }
        }

        private void DoImportFileVersion30(string file, LoaderBase loader, ProcessingOperations operations, string fileName, int version)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(stream);
                accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);

                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                dynamic result = null;
                switch (((RegisterEInfo)loader.Info).Type.ToLowerInvariant())
                {
                    case "r":
                        switch (version)
                        {
                            case 5:
                                result = loader.Load<v30.E.RegisterE>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            result == null ||
                            result.Account == null ||
                            result.Header == null ||
                            result.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                            return;
                        }

                        var resultLoad = _dataService.LoadZ(result, loader.Info.NumberXXXX, IsTestLoad, operations);
                        if (resultLoad.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoad.Data;

                                if (operations.Has(ProcessingOperations.PrimaryCheck))
                                {
                                    RunPrimaryCheck();
                                }

                                if (operations.Has(ProcessingOperations.DuplicateCheck))
                                {
                                    RunDuplicationCheck();
                                }

                                if (operations.Has(ProcessingOperations.PolicyCheck))
                                {
                                    RunPolicyCheck();

                                }

                                WriteRegisterZ(Tuple.Create(fileName, result as IZRegister, loader.Info as RegisterEInfo, (int)AccountType.GeneralPart, (int)DirectionType.In, version));
                                DoStats(result);
                                DoDbStatsZ(AccountId);
                                ResultText.AppendFormat("<b>Реестр успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(result);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.LastError);
                            return;
                        }
                        break;
                    case "d":
                        dynamic resultBack = null;
                        switch (version)
                        {
                            case 1:
                                //resultBack = loader.Load<v10.E.RegisterE>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case 5:
                                resultBack = loader.Load<v30.E.RegisterE>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            resultBack == null ||
                            resultBack.Account == null ||
                            resultBack.Header == null ||
                            resultBack.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                            return;
                        }

                        var resultLoadBack = _dataService.LoadBackZ(resultBack, loader.Info.NumberXXXX, IsTestLoad, operations);
                        if (resultLoadBack.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoadBack.Data;//ashurkova

                                if (operations.Has(ProcessingOperations.PrimaryCheck))
                                {
                                    RunPrimaryCheck();
                                }

                                if (operations.Has(ProcessingOperations.PolicyCheck))
                                {
                                    RunPolicyCheck();
                                }

                                WriteRegisterZ(Tuple.Create(fileName, resultBack as IZRegister, loader.Info as RegisterEInfo, (int)AccountType.CorrectedPart, (int)DirectionType.In, version));
                                DoStats(resultBack);
                                DoDbStatsZ(AccountId);
                                ResultText.AppendFormat("<b>Реестр(исправленная часть) успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(resultBack);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoadBack.LastError);
                            return;
                        }
                        break;
                    case "a":
                        dynamic resultAnswer = null;
                        switch (version)
                        {
                            case 1:
                                //resultAnswer = loader.Load<v10.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case 2:
                                resultAnswer = loader.Load<v21.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                            case 5:
                                resultAnswer = loader.Load<v30.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            resultAnswer == null ||
                            resultAnswer.Account == null ||
                            resultAnswer.Header == null ||
                            resultAnswer.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке протокола произошла ошибка<br>{0}", loader.ErrorsAggregate);

                            return;
                        }

                        var resultLoadAnswer = _dataService.LoadAnswer(resultAnswer, loader.Info as RegisterEInfo, IsTestLoad, operations);
                        if (resultLoadAnswer.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoadAnswer.Data; //ashurkova

                                WriteRegisterZ(Tuple.Create(fileName, resultAnswer as IZRegisterAnswer, loader.Info as RegisterEInfo, (int)AccountType.LogPart, (int)DirectionType.In, version));
                                DoStats(resultAnswer);
                                DoDbStatsZ(AccountId);
                                ResultText.AppendFormat("<b>Реестр(протокол обработки) успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(resultAnswer);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoadAnswer.LastError);
                            return;
                        }
                        break;
                    default:
                        ResultText.AppendFormat("Неизвестный тип файла xml {0}<br>", ((RegisterEInfo)loader.Info).Type);
                        return;
                }
            }
            DelFile(file, fileName);
        }

        private void DoImportFileVersion31(string file, LoaderBase loader, ProcessingOperations operations, string fileName, int version)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Stream accountStreamCleen = XmlHelpers.RemoveEmptyOrNull(stream);
                accountStreamCleen = XmlHelpers.ToUpperCase(accountStreamCleen);

                _overlayManager.SetOverlayMessage(Constants.DataProcessingMsg);
                dynamic result = null;
                switch (((RegisterEInfo)loader.Info).Type.ToLowerInvariant())
                {
                    case "r":
                        switch (version)
                        {
                            case Constants.Version31:
                                result = loader.Load<v31.E.RegisterE>(accountStreamCleen);
                                break;
                            case Constants.Version32:
                                result = loader.Load<v32.E.RegisterE>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            result == null ||
                            result.Account == null ||
                            result.Header == null ||
                            result.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                            return;
                        }

                        var resultLoad = _dataService.LoadZ(result, loader.Info.NumberXXXX, IsTestLoad, operations);
                        if (resultLoad.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoad.Data;

                                if (operations.Has(ProcessingOperations.PrimaryCheck))
                                {
                                    RunPrimaryCheck();
                                }

                                if (operations.Has(ProcessingOperations.DuplicateCheck))
                                {
                                    RunDuplicationCheck();
                                }

                                if (operations.Has(ProcessingOperations.PolicyCheck))
                                {
                                    RunPolicyCheck();

                                }

                                WriteRegisterZ(Tuple.Create(fileName, result as IZRegister, loader.Info as RegisterEInfo, (int)AccountType.GeneralPart, (int)DirectionType.In, version));
                                DoStats(result);
                                DoDbStatsZ(AccountId);
                                ResultText.AppendFormat("<b>Реестр успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(result);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoad.LastError);
                            return;
                        }
                        break;
                    case "d":
                        dynamic resultBack = null;
                        switch (version)
                        {
                            case 1:
                                //resultBack = loader.Load<v10.E.RegisterE>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case Constants.Version30:
                                resultBack = loader.Load<v30.E.RegisterE>(accountStreamCleen);
                                break;
                            case Constants.Version31:
                                resultBack = loader.Load<v31.E.RegisterE>(accountStreamCleen);
                                break;
                            case Constants.Version32:
                                resultBack = loader.Load<v32.E.RegisterE>(accountStreamCleen);
                                break;
                        }

                        if (loader.ErrorCount != 0 ||
                            resultBack == null ||
                            resultBack.Account == null ||
                            resultBack.Header == null ||
                            resultBack.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке реестра произошла ошибка<br>{0}", loader.ErrorsAggregate);
                            return;
                        }

                        var resultLoadBack = _dataService.LoadBackZ(resultBack, loader.Info.NumberXXXX, IsTestLoad, operations);
                        if (resultLoadBack.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoadBack.Data;//ashurkova

                                if (operations.Has(ProcessingOperations.PrimaryCheck))
                                {
                                    RunPrimaryCheck();
                                }

                                if (operations.Has(ProcessingOperations.PolicyCheck))
                                {
                                    RunPolicyCheck();
                                }

                                WriteRegisterZ(Tuple.Create(fileName, resultBack as IZRegister, loader.Info as RegisterEInfo, (int)AccountType.CorrectedPart, (int)DirectionType.In, version));
                                DoStats(resultBack);
                                DoDbStatsZ(AccountId);
                                ResultText.AppendFormat("<b>Реестр(исправленная часть) успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(resultBack);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoadBack.LastError);
                            return;
                        }
                        break;
                    case "a":
                        dynamic resultAnswer = null;
                        switch (version)
                        {
                            case 1:
                                //resultAnswer = loader.Load<v10.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                throw new NotImplementedException("Не реализовано");
                            case Constants.Version21:
                                resultAnswer = loader.Load<v21.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                            case Constants.Version30:
                                resultAnswer = loader.Load<v30.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                            case Constants.Version31:
                                resultAnswer = loader.Load<v31.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                            case Constants.Version32:
                                resultAnswer = loader.Load<v32.EAnswer.RegisterEAnswer>(accountStreamCleen);
                                break;
                        }


                        if (loader.ErrorCount != 0 ||
                            resultAnswer == null ||
                            resultAnswer.Account == null ||
                            resultAnswer.Header == null ||
                            resultAnswer.RecordsCollection.Count == 0)
                        {
                            ResultText.AppendFormat("При обработке протокола произошла ошибка<br>{0}", loader.ErrorsAggregate);

                            return;
                        }

                        var resultLoadAnswer = _dataService.LoadAnswer(resultAnswer, loader.Info as RegisterEInfo, IsTestLoad, operations);
                        if (resultLoadAnswer.Success)
                        {
                            if (!IsTestLoad)
                            {
                                AccountId = resultLoadAnswer.Data; //ashurkova

                                WriteRegisterZ(Tuple.Create(fileName, resultAnswer as IZRegisterAnswer, loader.Info as RegisterEInfo, (int)AccountType.LogPart, (int)DirectionType.In, version));
                                DoStats(resultAnswer);
                                DoDbStatsZ(AccountId);
                                ResultText.AppendFormat("<b>Реестр(протокол обработки) успешно загружен.</b><br>");
                            }
                            else
                            {
                                DoStats(resultAnswer);
                                ResultText.AppendFormat("<b>Тестовая загрузка успешно выполнена.</b><br>");
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("При попытке записи реестра в БД произошла ошибка<br>{0}", resultLoadAnswer.LastError);
                            return;
                        }
                        break;
                    default:
                        ResultText.AppendFormat("Неизвестный тип файла xml {0}<br>", ((RegisterEInfo)loader.Info).Type);
                        return;
                }
            }
            DelFile(file, fileName);
        }

        private void DelFile(string file, string fileName)
        {
            dynamic v = _remoteSettings.Get(AppRemoteSettings.FileXmlOms);
            bool fileoms = v.delOms;
            bool filexml = v.delXml;
            if (filexml)
            {
                _fileHelpers.DeleteFile(file);
            }
            if (fileoms)
            {
                _fileHelpers.DeleteFile(fileName);
            }
        }


        private void RunPolicyCheck()
        {
            _overlayManager.SetOverlayMessage(Constants.CheckPolicyMsg);
            var checkPolicyResult = Constants.ZterritoryVersion.Contains(_versionLoadFiles) ? _processingService.CheckZAccountPolicy(AccountId): _processingService.CheckAccountPolicy(AccountId);
            if (checkPolicyResult.Success)
            {
                ResultText.AppendFormat("<b>Проверка документов ОМС выполнена.</b><br>");
                if (checkPolicyResult.Data.Any())
                {
                    ResultText.AppendFormat("<b>Кол-во найденых пациентов с ошибками в документах ОМС {0}.</b><br>".F(checkPolicyResult.Data.Count()));
                    _processingService.ApplyPolicy(AccountId, checkPolicyResult.Data, Constants.ScopeInterTerritorialAccount);
                }
            }
        }

        private void RunDuplicationCheck()
        {
            _overlayManager.SetOverlayMessage(Constants.RunDuplicationExaminationMsg);
            var examsIdsResult = _examinationService.GetExamsIds(Constants.ScopeInterTerritorialAccount, _versionLoadFiles, new List<int?>
                                    {
                                        ExaminationGroup.Duplication.ToInt32()
                                    });
            if (examsIdsResult.HasError)
            {
                ResultText.AppendFormat("<b>Ошибка получения списка доступных экспертиз.</b><br>{0}".F(examsIdsResult.LastError));
            }
            else if (examsIdsResult.Data.Any())
            {
                var examinationsResult = _examinationService.RunExams(AccountId, Constants.ScopeInterTerritorialAccount, _versionLoadFiles, examsIdsResult.Data.Select(p => p as object));
                if (examinationsResult.Success)
                {
                    ResultText.AppendFormat("<b>Экспертизы выполнены.</b><br>");
                    if (examinationsResult.Data.Any())
                    {
                        var applyResult = Constants.ZterritoryVersion.Contains(_versionLoadFiles) ? 
                            _examinationService.ApplyZslExams(AccountId, Constants.ScopeInterTerritorialAccount, examinationsResult.Data) :
                            _examinationService.ApplyExams(AccountId, Constants.ScopeInterTerritorialAccount, examinationsResult.Data);
                        if (applyResult.Success)
                        {
                            var f014Cache = _cacheRepository.Get(CacheRepository.F014aCache);
                            foreach (var resultsByReason in examinationsResult.Data.GroupBy(p => p.Reason))
                            {
                                ResultText.AppendFormat("{0} - {1}<br>", f014Cache.GetString(resultsByReason.Key), resultsByReason.Count());
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("Ошибка применения результатов экспертиз : {0}<br>", applyResult.LastError);
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                    }
                }
            }
        }

        private void RunPrimaryCheck()
        {
            _overlayManager.SetOverlayMessage(Constants.RunExaminationsMsg);
            var examsIdsResult = _examinationService.GetExamsIds(Constants.ScopeInterTerritorialAccount, _versionLoadFiles, new List<int?>
                                    {
                                        ExaminationGroup.MandatoryFields.ToInt32(),
                                        ExaminationGroup.LogicAnalysis.ToInt32()
                                    });
            if (examsIdsResult.HasError)
            {
                ResultText.AppendFormat("<b>Ошибка получения списка доступных экспертиз.</b><br>{0}".F(examsIdsResult.LastError));
            }
            else if (examsIdsResult.Data.Any())
            {
                var examinationsResult = _examinationService.RunExams(AccountId, Constants.ScopeInterTerritorialAccount, _versionLoadFiles, examsIdsResult.Data.Select(p => p as object));
                if (examinationsResult.Success)
                {
                    ResultText.AppendFormat("<b>Экспертизы выполнены.</b><br>");
                    if (examinationsResult.Data.Any())
                    {
                        var applyResult = Constants.ZterritoryVersion.Contains(_versionLoadFiles) ?
                            _examinationService.ApplyZslExams(AccountId, Constants.ScopeInterTerritorialAccount, examinationsResult.Data) :
                            _examinationService.ApplyExams(AccountId, Constants.ScopeInterTerritorialAccount, examinationsResult.Data);
                        if (applyResult.Success)
                        {
                            var f014Cache = _cacheRepository.Get(CacheRepository.F014aCache);
                            foreach (var resultsByReason in examinationsResult.Data.GroupBy(p => p.Reason))
                            {
                                ResultText.AppendFormat("{0} - {1}<br>", f014Cache.GetString(resultsByReason.Key), resultsByReason.Count());
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("Ошибка применения результатов экспертиз : {0}<br>", applyResult.LastError);
                        }
                    }
                    else
                    {
                        ResultText.AppendFormat("Данные не изменены (и так все хорошо).<br>");
                    }
                }
            }
        }

        private void WriteRegister(Tuple<string, IRegister, RegisterEInfo, int, int, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                data.Item2.InnerHeader.Date.HasValue ? data.Item2.InnerHeader.Date.Value : DateTime.Now,
                data.Item6,
                data.Item4,
                data.Item5);
        }

        private void WriteRegisterZ(Tuple<string, IZRegister, RegisterEInfo, int, int, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.InnerRecordCollection.SelectMany(p => p.InnerZslEventCollection).Count(),
                data.Item2.InnerHeader.Date.HasValue ? data.Item2.InnerHeader.Date.Value : DateTime.Now,
                data.Item6,
                data.Item4,
                data.Item5);
        }

        private void WriteRegister(Tuple<string, IRegisterAnswer, RegisterEInfo, int, int, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                data.Item2.InnerHeader.Date.HasValue ? data.Item2.InnerHeader.Date.Value : DateTime.Now,
                data.Item6,
                data.Item4,
                data.Item5);
        }

        private void WriteRegisterZ(Tuple<string, IZRegisterAnswer, RegisterEInfo, int, int, int> data)
        {
            var exchangeResult = _dataService.ExchangeCreateOrUpdate(
                data.Item1,
                data.Item3,
                AccountId,
                data.Item2.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                data.Item2.InnerHeader.Date.HasValue ? data.Item2.InnerHeader.Date.Value : DateTime.Now,
                data.Item6,
                data.Item4,
                data.Item5);
        }

        private void DoStats(IRegister data)
        {
            var statResult = _complexReportRepository.DoRegisterELoadStats(data);
            if (statResult.Success)
            {
                ResultText.Append(statResult.Data);
            }
        }

        private void DoStats(IZRegister data)
        {
            var statResult = _complexReportRepository.DoRegisterELoadStats(data);
            if (statResult.Success)
            {
                ResultText.Append(statResult.Data);
            }
        }

        private void DoStats(IRegisterAnswer data)
        {
            var statResult = _complexReportRepository.DoRegisterELoadStats(data);
            if (statResult.Success)
            {
                ResultText.Append(statResult.Data);
            }
        }

        private void DoStats(IZRegisterAnswer data)
        {
            var statResult = _complexReportRepository.DoRegisterELoadStats(data);
            if (statResult.Success)
            {
                ResultText.Append(statResult.Data);
            }
        }

        private void DoDbStatsZ(int id)
        {
            var dbStatResult = _complexReportRepository.DoZRegisterEDbStats(id);
            if (dbStatResult.Success)
            {
                ResultText.Append(dbStatResult.Data);
            }
        }

        private void DoDbStats(int id)
        {
            var dbStatResult = _complexReportRepository.DoRegisterEDbStats(id);
            if (dbStatResult.Success)
            {
                ResultText.Append(dbStatResult.Data);
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