using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Core
{
    public enum SexEnum
    {
        [Display(ShortName = @"Мужской")]
        Male = 1,
        [Display(ShortName = @"Женский")]
        Female = 2,
    }

    public enum BindingType
    {
        [Display(ShortName = @"Транспорт HTTP")]
        BasicHttpBinding,
        [Display(ShortName = @"Транспорт TCP")]
        NetTcpBinding
    }

    public enum TypeLoad
    {
        [Display(ShortName = @"Файл")]
        File,
        [Display(ShortName = @"Папка")]
        Folder,
        [Display(ShortName = @"Папка рекурсивно")]
        FolderRecursive
    }

    public enum OperatorMode
    {
        [Display(ShortName = @"Счёт")]
        InterTerritorial = 0,
        [Display(ShortName = @"Счёт(ошибки)")]
        InterTerritorialError = 1,
        [Display(ShortName = @"Счёт(СРЗ)")]
        InterTerritorialSrzQuery = 2,
        [Display(ShortName = @"Счёт МО")]
        Local = 3,
        [Display(ShortName = @"Счёт МО(ошибки)")]
        LocalError = 4,
        [Display(ShortName = @"Счёт МО(СРЗ)")]
        LocalSrzQuery = 5,
        [Display(ShortName = @"Пациент")]
        Patient = 6,
        [Display(ShortName = @"Счёт МО(законченный случай)")]
        Zlocal = 7,
        [Display(ShortName = @"Счёт (законченный случай)")]
        ZInterTerritorial = 8,
        [Display(ShortName = @"Счёт(ошибки законченный случай)")]
        ZInterTerritorialError = 9,
        [Display(ShortName = @"Счёт МО(ошибки законченный случай)")]
        ZLocalError = 10

    }


    public enum TypeSank
    {
        [Display(ShortName = @"FactExternalRefuse")]
        FactExternalRefuse = 1,
        [Display(ShortName = @"ZFactSank")]
        ZFactSank = 2
    }

    public enum ProcessingType
    {
        [Display(ShortName = @"Встроенный")]
        BuiltIn = 1,
        [Display(ShortName = @"Sql")]
        Sql = 2,
        [Display(ShortName = @"С#")]
        CSharp = 3
        
    }

    public enum PolicyType 
    {
        [Display(ShortName = @"Отсутствует")]
        None = 0,
        [Display(ShortName = @"Полис старого образца")]
        Old = 1,
        [Display(ShortName = @"Временное свидетельство")]
        Temporary = 2,
        [Display(ShortName = @"ЕНП")]
        INP = 3,
    }

    public enum DirectionType
    {
        [Display(ShortName = @"Исходящии")]
        Out = 0,
        [Display(ShortName = @"Входящии")]
        In = 1
    }

    public enum RefusalSource
    {
        /// <summary>
        /// ТФОМС1 к МО
        /// </summary>
        [Display(ShortName = @"ТФОМС1 к МО")]
        Local = 1,
        /// <summary>
        /// ТФОМС2 к ТФОМС1
        /// </summary>
        [Display(ShortName = @"ТФОМС2 к ТФОМС1")]
        InterTerritorial = 2,
        /// <summary>
        /// Уточнённые санкции ТФОМС1 к МО
        /// </summary>
        [Display(ShortName = @"Уточнённые санкции ТФОМС1 к МО")]
        LocalCorrected = 3,
        /// <summary>
        /// Итоговые санкции ТФОМС2 к ТФОМС1
        /// </summary>
        [Display(ShortName = @"Итоговые санкции ТФОМС2 к ТФОМС1")]
        InterTerritorialTotal = 4
    }
    //20, 21, 22, 23, 24, 25, 26
    public enum RefusalType
    {
        [Display(ShortName = @"Неизвестно")]
        Unknown = 0,
        [Display(ShortName = @"МЭК")]
        MEC = 1,
        [Display(ShortName = @"МЭЭ")]
        MEE = 2,
        [Display(ShortName = @"ЭКМП")]
        EQMA = 3,
        [Display(ShortName = @"МЭК территории")]
        ExternalMEC = 4,
        [Display(ShortName = @"МЭЭ территории")]
        ExternalMEE = 5,
        [Display(ShortName = @"ЭКМП территории")]
        ExternalEQMA = 6,
        [Display(ShortName = @"Отказы территории")]
        External = 7,
    }

    [Flags]
    public enum StatusActExpertise
    {
        /// <summary>
        /// Новое
        /// </summary>
        [Display(ShortName = @"Новое")]
        New = 1,
        /// <summary>
        /// Утвержден
        /// </summary>
        [Display(ShortName = @"Утвержден")]
        Approved = 2,
    }

    [Flags]
    public enum VidControls
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        [Display(ShortName = @"Неизвестно")]
        Unknown = 0,
        /// <summary>
        /// МЭК
        /// </summary>
        [Display(ShortName = @"МЭК")]
        MEC = 1,
        /// <summary>
        /// МЭЭ
        /// </summary>
        [Display(ShortName = @"МЭЭ")]
        MEE = 2,
        /// <summary>
        /// ЭКМП
        /// </summary>
        [Display(ShortName = @"ЭКМП")]
        EQMA = 3,
        /// <summary>
        /// Реэкспертиза МЭК
        /// </summary>
        [Display(ShortName = @"Реэкспертиза МЭК")]
        ExternalMEC = 4,
        /// <summary>
        /// Реэкспертиза МЭЭ
        /// </summary>
        [Display(ShortName = @"МРеэкспертиза МЭЭ")]
        ExternalMEE = 5,
        /// <summary>
        /// Реэкспертиза ЭКМП
        /// </summary>
        [Display(ShortName = @"Реэкспертиза ЭКМП")]
        ExternalEQMA = 6,
    }

    [Flags]
    public enum RefusalStatusFlag
    {
        [Display(ShortName = @"Пусто")]
        None = 0,
        [Display(ShortName = @"Блокировка отказа")]
        Lock = 1,
        [Display(ShortName = @"Согласны с отказом")]
        Apply = 2,
        [Display(ShortName = @"Несогласны с отказом")]
        Dismiss = 4,
    }

    [Flags]
    public enum RefusalDest
    {
        [Display(ShortName = @"Отсутствует")]
        None = 0,
        [Display(ShortName = @"Исходящий")]
        Out = 1,
        [Display(ShortName = @"Входящий")]
        In = 2
    }

    [Flags]
    public enum SearchPolicyFlag
    {
        [Display(ShortName = @"ЕНП")]
        Inp = 1,
        [Display(ShortName = @"Номер документа ОМС")]
        Number = 2,
        [Display(ShortName = @"Серия документа ОМС")]
        Series = 4,
        [Display(ShortName = @"Фамилия")]
        Surname = 8,
        [Display(ShortName = @"Имя")]
        Name = 16,
        [Display(ShortName = @"Отчество")]
        Patronymic = 32,
        [Display(ShortName = @"Дата рождения")]
        Birthday = 64,
        [Display(ShortName = @"Пол")]
        Sex = 128,
    }

    public enum SearchParameters
    {
        InsuranceNumber,
        Id,
        Name,
        Surname,
        Patronymic,
        BirthDate,
        Sex,
        EventBeginDate,
        EventEndDate,
        EndEventBeginDate,
        EndEventEndDate,
        IsUnderpayment
    }

    public enum OperatorActionType
    {
        GoToPatient,
        GoToMedicalEvent
    }

    public enum AccountType
    {
        /// <summary>
        /// Основная часть
        /// </summary>
        [Display(ShortName = @"Основная часть")]
        GeneralPart = 1,
        [Display(ShortName = @"Исправленная часть")]
        CorrectedPart = 2,
        [Display(ShortName = @"Протокол обработки")]
        LogPart = 3,
        [Display(ShortName = @"Проверка полисов")]
        PolicyCheck = 4,
        [Display(ShortName = @"Основная часть МО")]
        GeneralPartMo = 5,
        [Display(ShortName = @"Исправленная часть МО")]
        LogPartMo = 6,
        [Display(ShortName = @"Протокол обработки МО")]
        CorrectedPartMo = 7
        
    }

    public enum AccountSourceType
    {
        [Display(ShortName = @"Источник территория")]
        InterTerritorial = 1,
        [Display(ShortName = @"Источник МО")]
        Local = 2
    }

    public enum AssistanceConditionType
    {
        [Display(ShortName = @"Стационар")]
        Hospital = 1,
        [Display(ShortName = @"Дневной стационар")]
        Dayhospital = 2,
        [Display(ShortName = @"Поликлиника")]
        Polyclinic = 3,
        [Display(ShortName = @"Скорая помошь")]
        Ambulance = 4
        
    }

    public enum FileDataExchangeStatus
    {
        [Display(ShortName = @"R файлы")]
        UnloadR = 1,
        [Display(ShortName = @"Загруженые R файлы")]
        LoadedR = 2,
        [Display(ShortName = @"D файлы")]
        UnloadD = 3,
        [Display(ShortName = @"Загруженые D файлы")]
        LoadedD = 4,
        [Display(ShortName = @"A файлы")]
        UnloadA = 5,
        [Display(ShortName = @"Загруженые A файлы")]
        LoadedA = 6,
    }

    public enum AccountStatus
    {
        [Display(ShortName = @"Не отработан")]
        NotProcessed = 1,
        [Display(ShortName = @"Отработан")]
        Processed = 2,
        [Display(ShortName = @"Отправлен")]
        Sended = 3,
        [Display(ShortName = @"Не оплачен")]
        NotPay = 4,
        [Display(ShortName = @"Частично оплачен")]
        PartlyPay = 5,
        [Display(ShortName = @"Полностью оплачен")]
        Pay = 6,
    }

    public enum ErrorScope
    {
        [Display(ShortName = @"Счет МО")]
        MedicalAccount = 1,
        [Display(ShortName = @"Счет")]
        Account = 2,
        [Display(ShortName = @"Пациент")]
        Patient = 3,
        [Display(ShortName = @"Случай")]
        MedicalEvent = 4,
        [Display(ShortName = @"Услуга")]
        MedicalService = 5
    }

    public enum ExaminationGroup
    {
        [Display(ShortName = @"Обязательные поля")]
        MandatoryFields = 1,
        [Display(ShortName = @"Корректность заполнения полей")]
        LogicAnalysis = 2,
        [Display(ShortName = @"Дублирование")]
        Duplication = 3
    }
    
    
    public static class Constants
    {
        public static List<int?> Mee = new List<int?> { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29};
        public static List<int?> Eqma = new List<int?> {30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41,42,43,44,45,46,47,48,49};
        public static List<int?> Zversion = new List<int?> { Version30K, Version30, Version31K, Version31, Version32K, Version32 };
        public static List<int> ZterritoryVersion = new List<int> { Version30, Version31, Version32 };
        public static List<int?> ZterritoryVersionNull = new List<int?> { Version30, Version31, Version32 };
        public static List<int> ZmedicalVersion = new List<int> { Version30K, Version31K, Version32K };
        public const int ScopeLocalAccount = 1;
        public const int ScopeInterTerritorialAccount = 2;
        public const int ScopePatient = 3;
        public const int ScopeActMee = 7;
        public const int ScopeActEqma = 8;

        public const int RefusalSourceFromTfoms1ToMo = 1;
        public const int RefusalSourceFromTfoms2ToTfoms1 = 2;

        public const int Version10 = 1;
        public const int Version21 = 2;
        public const int Version21K = 3;
        public const int Version30K = 4;
        /// <summary>
        /// Версия территориального счета 3.0
        /// </summary>
        public const int Version30 = 5;
        public const int Version31K = 6;
        /// <summary>
        /// Версия территориального счета 3.1
        /// </summary>
        public const int Version31 = 7;

        public const int Version32K = 8;
        /// <summary>
        /// Версия территориального счета 3.2
        /// </summary>
        public const int Version32 = 9;

        public const int GeneralPart = 0;
        public const int LogPart = 1;
        public const int CorrectedPart = 2;
        /// <summary>
        /// Сведения об оплате
        /// </summary>
        public const int InformationPaymentPart = 3;
        

        public static string DefaultDateFormat = "yyyy-MM-dd";

        public static readonly List<int> InterTerritorialAccountTypes = new List<int> { 1, 2, 3 };
        public static readonly List<int> LocalAccountTypes = new List<int> {5, 6, 7};

        public static int? PolicyRefusalId = 57;
        public static int SystemAccountId = 99999;

        public const string OperationBeginTitleMsg = "Подготовка операции.";
        public const string FileProcessingTitleMsg = "Обработка файлов.";
        public const string RunCheckDataTitleMsg = "Запуск проверки данных.";
        public const string ApplyCheckDataResultTitleMsg = "Применение результатов проверки данных.";
        public const string UserAuthTitleMsg = "Проверка данных пользователя.";
        public const string UpdateMtrTitleMsg = "Поиск обновлений";
        public const string SearchDataTitleMsg = "Поиск данных.";
        public const string AccountBreakUpTitleMsg = "Расформирование счета.";
        public const string AccountDeleteTitleMsg = "Удаление счета.";
        public const string RemainMsg = "До завершения операции осталось ~{0}.";
        public const string LoadDataMsg = "Загрузка данных.";
        public const string UnpackDataMsg = "Распаковка данных";
        public const string ValidationXmlMsg = "Валидация xml";
        public const string SaveToDatabaseMsg = "Запись в базу данных";
        public const string FlushDataMsg = "Выгрузка данных";
        public const string ReadDataMsg = "Чтение данных";
        public const string DataProcessingMsg = "Обработка данных";
        public const string MakeTerritoryAccountTitleMsg = "Формирование счетов на территорию.";
        public const string BreakUpAccountIdMsg = "Расформирование счета ID {0}.";
        public const string DeleteAccountIdMsg = "Удаление счета ID {0}.";
        public const string BreakUpAccountManyMsg = "Расформирование счетов.";
        public const string DeleteAccountManyMsg = "Удаление счетов.";
        public const string RunExaminationsMsg = "Запуск экспертиз.";
        public const string RunDuplicationExaminationMsg = "Запуск проверки дублирования.";
        public const string CheckPolicyMsg = "Запуск проверки документов ОМС.";
        public const string RemoveRefusalsMsg = "Удаление отказов.";

        public const string PleaseWaitMsg = "Пожалуйста подождите...";
        /// <summary>
        /// При {0} {1} произошла ошибка.\r\n{2} - 
        /// При загрузке счета произошла ошибка SQL ....
        /// </summary>
        public static string DbErrorCommonMsg = "При {0} {1} произошла ошибка.\r\n{2}";
        public const string SaveToDatabaseDoneMsg = "Запись в базу данных завершена";
        public const string CheckPolicyInTortillaMsg = "Проверка документов ОМС в Тортиле";

        public static string ServiceUrl()
        {
            if (ConfigurationManager.AppSettings["ServiceUrl"] == null)
                return "http://91.240.209.20:42079/updateservice/";
            return ConfigurationManager.AppSettings["ServiceUrl"];
        }

    }


}
