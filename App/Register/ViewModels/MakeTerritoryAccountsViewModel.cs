using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Core;
using Core.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    /// <summary>
    /// Формирование счетов на территорию
    /// </summary>
    public class MakeTerritoryAccountsViewModel : ViewModelBase
    {
        private readonly IMedicineRepository _repository;
        private readonly IFileService _fileService;
        private readonly INotifyManager _notifyManager;
        private readonly ITextService _textService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private OperatorMode _mode;
        //private int _version;

        private string _notes;

        private RelayCommand _saveLogCommand;
        private RelayCommand _createAccountCommand;
        

        public MakeTerritoryAccountsViewModel(IMedicineRepository repository,
            IFileService fileService, 
            INotifyManager notifyManager,
            ITextService textService,
            IDockLayoutManager dockLayoutManager,
            OperatorMode mode)
        {
            _repository = repository;
            _fileService = fileService;
            _notifyManager = notifyManager;
            _textService = textService;
            _dockLayoutManager = dockLayoutManager;
            _mode = mode;

            Init();
        }

        private void Init()
        {
            var culture = new CultureInfo("ru-RU");
            MonthList = new List<string>(culture.DateTimeFormat.MonthNames.Take(12));
            var result = _repository.GetMedicalAccountDate();
            if (result.Success)
            {
                YearsList = new List<int>(result.Data.Select(x => x));
            }
        }

        public int LocalVersion
        {
            get
            {
                switch (Version)
                {
                    case 0:
                        return TerritoryService.TerritoryDefaultOmsVersion;
                    case 1:
                        return Constants.Version30;
                    case 2:
                        return Constants.Version31;
                    case 3:
                        return Constants.Version32;
                    default:
                        return TerritoryService.TerritoryDefaultOmsVersion;
                }
            }
        }

        public int Version { get; set; }

        public StringBuilder ResultText { get; set; }
        public string Notes {
            get { return _notes; }
            set { _notes = value;RaisePropertyChanged(()=>Notes); }
        }
        public IEnumerable<string> MonthList { get; set; }
        public IEnumerable<int> YearsList { get; set; }
        public int? SelectedYear { get; set; }
        public int? SelectedMonth { get; set; }

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

        public ICommand CreateAccountCommand
        {
            get { return _createAccountCommand ?? (_createAccountCommand = new RelayCommand(CreateAccount, CanCreateAccount)); }
        }

        private bool CanCreateAccount()
        {
            return SelectedYear.HasValue && SelectedMonth.HasValue;
        }

        private void CreateAccount()
        {

            _dockLayoutManager.ShowOverlay(Constants.MakeTerritoryAccountTitleMsg, Constants.PleaseWaitMsg);

            var accounts = new List<int>();
            ResultText = new StringBuilder();

            if (!SelectedYear.HasValue || !SelectedMonth.HasValue)
            {
                ResultText.AppendFormat("Выберете отчетный год и месяц\n");
                return;
            }

            int year = YearsList.ToList()[SelectedYear.Value];
            int month = SelectedMonth.Value + 1;

            ResultText.AppendFormat("<b>Формирование счетов для отчетного периода</b><br>{0} {1} г.<br>", MonthList.ToList()[month - 1], year);

            if (_mode == OperatorMode.Zlocal)
            {
                CreateAccountZevent(accounts, year, month);
            }
            else
            {
                CreateAccountEvent(accounts, year, month);
            }

            ApplyNotes();
           
        }


        private void CreateAccountEvent(List<int> accounts, int year, int month)
        {
            try
            {
                //Разделяем случаи полностью отказанные и с полной или частичной оплатой
                var splitPatients = _repository.SplitPatientsWithFullRefusal(year, month);
                if (splitPatients.HasError)
                {
                    ResultText.AppendFormat("<b>Ошибка при разбивке случаев на полностью отказанные и с полной или частичной оплатой{0}<br>", splitPatients.LastError);
                    ApplyNotes();
                    return;
                }

                var patientsResult = _repository.GetUnmakePatientsByDate(year, month);
                if (patientsResult.HasError)
                {
                    ResultText.AppendFormat("<b>Ошибка при получении данных пациентов {0}<br>", patientsResult.LastError);
                    ApplyNotes();
                    return;
                }

                if (!patientsResult.Data.Any())
                {
                    ResultText.AppendFormat("<b>Все пациенты уже разбиты по территориям.");
                    ApplyNotes();
                    return;
                }

                var patients = patientsResult.Data;

                foreach (var territory in patients)
                {
                    var territoryCopy = territory;
                    var existAccountResult = _repository.GetTerritoryAccount(p =>
                        p.Destination == territoryCopy.Key.KOD_OKATO &&
                        p.Source == TerritoryService.TerritoryOkato &&
                        p.Type == (int)AccountType.GeneralPart &&
                        //проверка отчетного года
                        p.Date.Value.Year == year &&
                        //проверка отчетного месяца года
                        p.Date.Value.Month == month);
                    if (existAccountResult.Success)
                    {
                        var existAccount = existAccountResult.Data.FirstOrDefault();
                        int accountId = 0;
                        if (existAccount == null)
                        {
                            var packetNumberResult = _repository.GetTerritoryAccountLastPacketNumber(
                                p => p.Destination == territoryCopy.Key.KOD_OKATO &&
                                     p.Source == TerritoryService.TerritoryOkato &&
                                     p.Type == (int)AccountType.GeneralPart);
                            int packetNumber = 1;
                            if (packetNumberResult.Success)
                            {
                                packetNumber = packetNumberResult.Data;
                            }

                            var accountNumberResult = _repository.GetTerritoryAccountLastAccountNumber(
                                p => p.Source == TerritoryService.TerritoryOkato &&
                                    p.Date.Value.Year == year &&
                                    p.Type == (int)AccountType.GeneralPart);
                            int accountNumber = 1;
                            if (accountNumberResult.Success)
                            {
                                accountNumber = accountNumberResult.Data;
                            }

                            ResultText.AppendFormat("<b>Создание счета на территорию: {0}</b><br>", territoryCopy.Key.SUBNAME);
                            var createAccountResult = _repository.CreateTerritoryAccount(territoryCopy.Key.KOD_OKATO, year, month, packetNumber + 1, accountNumber + 1, Constants.Version21);
                            if (createAccountResult.Success)
                            {
                                accountId = createAccountResult.Id;
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("<b>Обновление счета на территорию: {0}</b><br>", territoryCopy.Key.SUBNAME);
                            accountId = existAccount.TerritoryAccountId;
                        }

                        if (accountId == 0)
                        {
                            ResultText.AppendFormat("<b>Ошибка создания счета на территорию: {0}</b><br>", territoryCopy.Key.SUBNAME);
                            continue;
                        }

                        accounts.Add(accountId);

                        var resultAttachPatients = _repository.AttachPatientsToTerritoryAccount(p => p.AccountId == null &&
                            p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                            p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                            p.TerritoryOkato.HasValue &&
                            p.TerritoryOkato == territoryCopy.Key.Id &&
                            p.FACTMEDIPATIENTIDFACTPATIs.All(s => s.AcceptPrice != null && s.AcceptPrice > 0),
                            accountId);

                        if (resultAttachPatients.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка присоединения пациентов к счету ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var enumeratePatientsResult = _repository.EnumeratePatientsOfTerritoryAccount(accountId);
                        if (enumeratePatientsResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка перенумерования пациентов счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var enumerateEventsResult = _repository.EnumerateEventsOfTerritoryAccount(accountId);
                        if (enumerateEventsResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка перенумерования случаев МП счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var updateHeaderResult = _repository.UpdateTerritoryAccount(accountId);
                        if (updateHeaderResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка обновления заголовка счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        ResultText.AppendFormat("<b>&nbsp;ID счета: {0}</b><br>", accountId);
                    }

                }
            }
            catch (Exception exception)
            {
                _dockLayoutManager.HideOverlay();
                ResultText.AppendFormat("При обработке данных произошло исключение\n{0}", exception);
            }
        }

        private void CreateAccountZevent(List<int> accounts, int year, int month)
        {
            try
            {
                
                //Разделяем случаи полностью отказанные и с полной или частичной оплатой
                var splitPatients = _repository.ZSplitPatientsWithFullRefusal(year, month); 
                if (splitPatients.HasError)
                {
                    ResultText.AppendFormat("<b>Ошибка при разбивке случаев на полностью отказанные и с полной или частичной оплатой{0}<br>", splitPatients.LastError);
                    ApplyNotes();
                    return;
                }

                var patientsResult = _repository.GetZUnmakePatientsByDate(year, month);
                if (patientsResult.HasError)
                {
                    ResultText.AppendFormat("<b>Ошибка при получении данных пациентов {0}<br>", patientsResult.LastError);
                    ApplyNotes();
                    return;
                }

                if (!patientsResult.Data.Any())
                {
                    ResultText.AppendFormat("<b>Все пациенты уже разбиты по территориям.");
                    ApplyNotes();
                    return;
                }

                var patients = patientsResult.Data;

                foreach (var territory in patients)
                {
                    var territoryCopy = territory;
                    var existAccountResult = _repository.GetTerritoryAccount(p =>
                        p.Destination == territoryCopy.Key.KOD_OKATO &&
                        p.Source == TerritoryService.TerritoryOkato &&
                        p.Type == (int)AccountType.GeneralPart &&
                        //проверка отчетного года
                        p.Date.Value.Year == year &&
                        //проверка отчетного месяца года
                        p.Date.Value.Month == month);
                    if (existAccountResult.Success)
                    {
                        var existAccount = existAccountResult.Data.FirstOrDefault();
                        int accountId = 0;
                        if (existAccount == null)//Если счета еще нет
                        {
                            var packetNumberResult = _repository.GetTerritoryAccountLastPacketNumber(
                                p => p.Destination == territoryCopy.Key.KOD_OKATO &&
                                     p.Source == TerritoryService.TerritoryOkato &&
                                     p.Type == (int)AccountType.GeneralPart);
                            int packetNumber = 1;
                            if (packetNumberResult.Success)
                            {
                                packetNumber = packetNumberResult.Data;
                            }

                            var accountNumberResult = _repository.GetTerritoryAccountLastAccountNumber(
                                p => p.Source == TerritoryService.TerritoryOkato &&
                                    p.Date.Value.Year == year &&
                                    p.Type == (int)AccountType.GeneralPart);
                            int accountNumber = 1;
                            if (accountNumberResult.Success)
                            {
                                accountNumber = accountNumberResult.Data;
                            }

                            ResultText.AppendFormat("<b>Создание счета на территорию: {0}</b><br>", territoryCopy.Key.SUBNAME);
                            var createAccountResult = _repository.CreateTerritoryAccount(territoryCopy.Key.KOD_OKATO, year, month, packetNumber + 1, accountNumber + 1, LocalVersion);
                            if (createAccountResult.Success)
                            {
                                accountId = createAccountResult.Id;
                            }
                        }
                        else
                        {
                            ResultText.AppendFormat("<b>Обновление счета на территорию: {0}</b><br>", territoryCopy.Key.SUBNAME);
                            accountId = existAccount.TerritoryAccountId;
                        }

                        if (accountId == 0)
                        {
                            ResultText.AppendFormat("<b>Ошибка создания счета на территорию: {0}</b><br>", territoryCopy.Key.SUBNAME);
                            continue;
                        }

                        accounts.Add(accountId);

                        var resultAttachPatients = _repository.AttachPatientsToTerritoryAccount(p => p.AccountId == null &&
                            p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                            p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                            p.TerritoryOkato.HasValue &&
                            p.TerritoryOkato == territoryCopy.Key.Id &&
                            p.ZSLFACTMEDIPATIENTIDFACTPATIs.All(s => s.AcceptPrice != null && s.AcceptPrice > 0),
                            accountId);

                        if (resultAttachPatients.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка присоединения пациентов к счету ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var enumeratePatientsResult = _repository.EnumeratePatientsOfTerritoryAccount(accountId);
                        if (enumeratePatientsResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка перенумерования пациентов счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var enumerateZslEventsResult = _repository.EnumerateZslEventsOfTerritoryAccount(accountId);
                        if (enumerateZslEventsResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка перенумерования законченных случаев МП счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var enumerateEventsResult = _repository.EnumerateZEventsOfTerritoryAccount(accountId);
                        if (enumerateEventsResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка перенумерования случаев МП счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        var updateHeaderResult = _repository.UpdateZTerritoryAccount(accountId);
                        if (updateHeaderResult.HasError)
                        {
                            ResultText.AppendFormat("<b>Ошибка обновления заголовка счета ID: {0}</b><br>", accountId);
                            continue;
                        }

                        ResultText.AppendFormat("<b>&nbsp;ID счета: {0}</b><br>", accountId);
                    }

                }
            }
            catch (Exception exception)
            {
                _dockLayoutManager.HideOverlay();
                ResultText.AppendFormat("При обработке данных произошло исключение\n{0}", exception);
            }
        }

        public void ApplyNotes()
        {
            _dockLayoutManager.HideOverlay();
            var toRtfResult = _textService.ToRtf(ResultText.ToString());
            if (toRtfResult.Success)
            {
                Notes = toRtfResult.Data;
            }
        }
    }
}
