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
using v10PL = Medical.DataCore.v10PL;
using GalaSoft.MvvmLight.Threading;
using System.Threading.Tasks;
using Autofac;
using Core.Services;
using Medical.AppLayer.Economic.Models;
using Medical.CoreLayer.Models.Common;
using Medical.DataCore.v10PL.PL;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EcExportXmlViewModel : ViewModelBase
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
        private readonly int _economicAccountId;
        private RelayCommand _exportCommand;
        private RelayCommand _saveLogCommand;
        private RelayCommand _selectFileCommand;
        private RelayCommand _showFileCommand;

        private string _fileName;
        private string _notes;
        private string _omsFileName;

        public EcExportXmlViewModel(IAppRemoteSettings remoteSettings,
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
            int id, int economicAccountId)
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
            _economicAccountId = economicAccountId;

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
                Version = v.version;
            }

            TypeLoad = 0;


            if (CommonGridControl<EconomicAccountCustomModel>._selectEconomicAccount.IsNotNull())
                if (CommonGridControl<EconomicAccountCustomModel>._selectEconomicAccount.Any())
                {
                    AllAccountList.AddRange(
                        CommonGridControl<EconomicAccountCustomModel>._selectEconomicAccount.Select(p => new CommonTuple
                        {
                            DisplayField = _commonService.GetTitleByEconomicAccountView(p),
                            ValueField = p.Account.AccountId,
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
        public int TypeLoadPl { get; set; }
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

        public ObservableCollection<CommonTuple> AllOptionsList { get; set; }
        public List<object> SelectedOptionsList { get; set; }

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
        public List<object> SelectedAccountList { get; set; }
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
                       // _messageService.ShowException(result.LastError, "Исключение при конвертации html в rtf", typeof(ImportLocalXmlViewModel));
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
            if (TypeLoad == 0) TypeLoadPl = 6;
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
                //Ashurkova begin
                case Constants.GeneralPart:
                    ResultText.AppendLine("<b>Начало экспорта сведений об оплате в {0}</b><br>".F(FileName));
                    switch (Version)
                    {
                        case Constants.Version10:
                            var result10 = _dataService.WriteAnswer1<v10PL.PL.RegisterPlAnswer,
                                v10PL.PL.InformationPlAnswer,
                                v10.EAnswer.HeaderEAnswer,
                                v10.EAnswer.RecordsEAnswer,
                                v10.EAnswer.PatientEAnswer,
                                v10.EAnswer.EventEAnswer>(_id, _economicAccountId, Version, TypeLoadPl);
                            if (result10.Success)
                            {
                                WriteRegisterAnswerPl(result10.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result10.ErrorMessage);
                            }
                            break;
                        case Constants.Version21:
                            var result21 = _dataService.WriteAnswer1<v10PL.PL.RegisterPlAnswer,
                                v10PL.PL.InformationPlAnswer,
                                v10.EAnswer.HeaderEAnswer,
                                v10.EAnswer.RecordsEAnswer,
                                v10.EAnswer.PatientEAnswer,
                                v10.EAnswer.EventEAnswer>(_id, _economicAccountId, Version, TypeLoadPl);
                            if (result21.Success)
                            {
                                WriteRegisterAnswerPl(result21.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result21.ErrorMessage);
                            }
                            break;
                        case Constants.Version30:
                        case Constants.Version31:
                            var result30 = _dataService.WritePaymentOrder<v10PL.PL.RegisterPlAnswer,
                                v10PL.PL.InformationPlAnswer,
                                v10.EAnswer.HeaderEAnswer,
                                v10.EAnswer.RecordsEAnswer,
                                v10.EAnswer.PatientEAnswer,
                                v10.EAnswer.EventEAnswer>(_id, _economicAccountId, Version, TypeLoadPl);
                            if (result30.Success)
                            {
                                WriteRegisterAnswerPl(result30.Data);
                            }
                            else
                            {
                                ResultText.AppendLine(result30.ErrorMessage);
                            }
                            break;
                    }
                    break;
                    //Ashurkova end
                default:
                    ResultText.AppendLine("<b>Неизвестный тип файла {0}</b><br>".F(TypeLoad));
                    break;
            }

            ResultText.AppendLine("<br>Прошло {0} сек.<br>".F(_commonService.StopWatchAndGetSeconds()));
        }
        

        private void WriteRegisterAnswerPl<T>(Tuple<T, RegisterEInfo, int> data) where T : IRegisterPlAnswer
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
                        1,//data.Item1.InnerRecordCollection.SelectMany(p => p.InnerEventCollection).Count(),
                        DateTime.Now, 
                        Version,
                        TypeLoadPl,
                        data.Item3);
                    if (exchangeResult.Success)
                    {
                        var statResult = _complexReportRepository.DoRegisterPlAnswerStats(data.Item1);
                        if (statResult.Success)
                        {
                            ResultText.Append(statResult.Data);
                        }
                        OmsFileName = omsFilename;
                        ResultText.Append("<b>Файл: {0}</b>".F(omsFilename));
                    }
                }
                else
                {
                    ResultText.Append("<b>Ошибка: {0}</b>".F(packResult.LastError?.Message));
                }
            }
        }


    }
}