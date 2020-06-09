using System;
using System.ComponentModel.DataAnnotations;
using BLToolkit.Mapping;
using Core.Extensions;
using DevExpress.XtraEditors.DXErrorProvider;

namespace DataModel
{
    [MetadataType(typeof(GeneralEventShortView))]
    public partial class GeneralEventShortView
    {
        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string PatientData
        {
            get
            {
                return "{0} {1} {2}/ДР: {3}/{4}/Полис: {5},{6}".F(
              Surname,
              Name,
              Patronymic,
              Birthday.HasValue ? Birthday.Value.ToString("dd.MM.yyyy") : "нет",
              Sex,
              InsuranceDocType == 3 ? INP : "{0} {1}".F(InsuranceDocNumber, InsuranceDocSeries),
              PaymentStatus);
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventData
        {
            get
            {
                return "начало:{0} окончание:{1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventDataShort
        {
            get
            {
                return "{0} - {1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }
    }

    [MetadataType(typeof (EventShortViewMeta))]
    public partial class EventShortView
    {
        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string PatientData
        {
            get { return "{0} {1} {2}/ДР: {3}/{4}/Полис: {5},{6}".F(
                Surname, 
                Name, 
                Patronymic,
                Birthday.HasValue ? Birthday.Value.ToString("dd.MM.yyyy") : "нет", 
                Sex, 
                InsuranceDocType == 3 ? INP : "{0} {1}".F(InsuranceDocNumber, InsuranceDocSeries),
                PaymentStatus); }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventData
        {
            get
            {
                return "начало:{0} окончание:{1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventDataShort
        {
            get
            {
                return "{0} - {1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }
    }

    [MetadataType(typeof(ZslEventShortViewMeta))]
    public partial class ZslEventShortView
    {
        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string PatientData
        {
            get
            {
                return "{0} {1} {2}/ДР: {3}/{4}/Полис: {5},{6}".F(
              Surname,
              Name,
              Patronymic,
              Birthday.HasValue ? Birthday.Value.ToString("dd.MM.yyyy") : "нет",
              Sex,
              InsuranceDocType == 3 ? INP : "{0} {1}".F(InsuranceDocNumber, InsuranceDocSeries),
              PaymentStatus);
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventData
        {
            get
            {
                return "начало:{0} окончание:{1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventDataShort
        {
            get
            {
                return "{0} - {1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }
    }

    [MetadataType(typeof(ZslEventViewMeta))]
    public partial class ZslEventView
    {
        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string PatientData
        {
            get
            {
                return "{0} {1} {2}/ДР: {3}/{4}/Полис: {5},{6}".F(
              Surname,
              Name,
              Patronymic,
              Birthday.HasValue ? Birthday.Value.ToString("dd.MM.yyyy") : "нет",
              Sex,
              InsuranceDocType == 3 ? INP : "{0} {1}".F(InsuranceDocNumber, InsuranceDocSeries),
              PaymentStatus);
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventData
        {
            get
            {
                return "начало:{0} окончание:{1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }

        [Display(AutoGenerateField = false)]
        [MapIgnore(true)]
        public string MeventDataShort
        {
            get
            {
                return "{0} - {1}".F(
                    EventBegin.HasValue ? EventBegin.Value.ToString("dd.MM.yyyy") : "нет",
                    EventEnd.HasValue ? EventEnd.Value.ToString("dd.MM.yyyy") : "нет");
            }
        }
    }

    public class EventShortViewMeta
    {
        [Display(Name = "Внешний ID")]
        public object ExternalId { get; set; }
        [Display(Name = "ID счета")]
        public int? AccountId { get; set; } // int(10)
        [Display(Name = "ID счета МО")]
        public int? MedicalAccountId { get; set; } // int(10)
        [Display(Name = "ID случая")]
        public int EventId { get; set; } // int(10)
        [Display(Name = "ID пациента")]
        public int? PatientId { get; set; } // int(10)
        [Display(Name = "Фамилия")]
        public string Surname { get; set; } // nvarchar(40)
        [Display(Name = "Имя")]
        public string Name { get; set; } // nvarchar(40)
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; } // nvarchar(40)
        [Display(Name = "Дата рождения")]
        public DateTime? Birthday { get; set; } // datetime(3)
        [Display(Name = "Пол")]
        public string Sex { get; set; } // nvarchar(7)
        [Display(Name = "Место рождения")]
        public string BirthPlace { get; set; } // nvarchar(100)
        [Display(Name = "Серия УДЛ")]
        public string DocSeries { get; set; } // nvarchar(10)
        [Display(Name = "Номер УДЛ")]
        public string DocNum { get; set; } // nvarchar(20)
        [Display(Name = "ЕНП")]
        public string INP { get; set; } // nvarchar(20)
        [Display(Name = "Номер ОМС")]
        public string InsuranceDocNumber { get; set; } // nvarchar(20)
        [Display(Name = "Серия ОМС")]
        public string InsuranceDocSeries { get; set; } // nvarchar(10)
        [Display(Name = "СМО")]
        public string Insurance { get; set; } // nvarchar(5)
        [Display(Name = "Условия МП")]
        public decimal? AssistanceConditions { get; set; } // numeric(2)
        [Display(Name = "Диагноз")]
        public string Diagnosis { get; set; } // nvarchar(10)
        [Display(Name = "Стоимость"), DataType(DataType.Currency)]
        public decimal? Price { get; set; } // money(19,4)
        [Display(Name = "Тариф"), DataType(DataType.Currency)]
        public decimal? Rate { get; set; } // money(19,4)
        [Display(Name = "Кол-во")]
        public decimal? Quantity { get; set; } // money(19,4)
        [Display(Name = "Принято к оплате"), DataType(DataType.Currency)]
        public decimal? AcceptPrice { get; set; } // money(19,4)
        [Display(Name = "Начало случая")]
        public DateTime? EventBegin { get; set; } // datetime(3)
        [Display(Name = "Окончание случая")]
        public DateTime? EventEnd { get; set; } // datetime(3)
        [Display(Name = "Профиль")]
        public decimal? Profile { get; set; } // numeric(3)
        [Display(Name = "Специальность")]
        public decimal? Speciality { get; set; } // numeric(9)
        [Display(Name = "Результат")]
        public decimal? Result { get; set; } // numeric(3)
        [Display(Name = "Исход")]
        public decimal? Outcome { get; set; } // numeric(3)
        [Display(Name = "Статус оплаты")]
        public string PaymentStatus { get; set; } // nvarchar(254)
        [Display(Name = "МЭК"), DataType(DataType.Currency)]
        public decimal? MEC { get; set; } // money(19,4)
        [Display(Name = "МЭЭ"), DataType(DataType.Currency)]
        public decimal? MEE { get; set; } // money(19,4)
        [Display(Name = "ЭКМП"), DataType(DataType.Currency)]
        public decimal? EQMA { get; set; } // money(19,4)
        [Display(Name = "Комментарий к случаю")]
        public string EventComments { get; set; } // nvarchar(250)

        [Display(AutoGenerateField = false)]
        public decimal? AssistanceType { get; set; }
        [Display(AutoGenerateField = false)]
        public int? InsuranceDocType { get; set; } 
        [Display(AutoGenerateField = false)]
        public int? SexCode { get; set; } 
        [Display(AutoGenerateField = false)]
        public string TerritoryOkato { get; set; }
        [Display(AutoGenerateField = false)]
        public int? DocType { get; set; }

    }

    public class ZslEventShortViewMeta
    {
        [Display(Name = "Внешний ID")]
        public object ExternalId { get; set; }
        [Display(Name = "ID счета")]
        public int? AccountId { get; set; } // int(10)
        [Display(Name = "Номер счета")]
        public int? AccountNumber { get; set; } // int(10)
        [Display(Name = "Дата счета")]
        public DateTime? AccountDate { get; set; } // datetime(3)
        [Display(Name = "ID счета МО")]
        public int? MedicalAccountId { get; set; } // int(10)
        [Display(Name = "ID законченного случая")]
        public int ZslMedicalEventId { get; set; } // int(10)
        [Display(Name = "ID случая")]
        public int EventId { get; set; } // int(10)
        [Display(Name = "ID пациента")]
        public int? PatientId { get; set; } // int(10)
        [Display(Name = "Фамилия")]
        public string Surname { get; set; } // nvarchar(40)
        [Display(Name = "Имя")]
        public string Name { get; set; } // nvarchar(40)
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; } // nvarchar(40)
        [Display(Name = "Дата рождения")]
        public DateTime? Birthday { get; set; } // datetime(3)
        [Display(Name = "Пол")]
        public string Sex { get; set; } // nvarchar(7)
        [Display(Name = "Место рождения")]
        public string BirthPlace { get; set; } // nvarchar(100)
        [Display(Name = "Серия УДЛ")]
        public string DocSeries { get; set; } // nvarchar(10)
        [Display(Name = "Номер УДЛ")]
        public string DocNum { get; set; } // nvarchar(20)
        [Display(Name = "ЕНП")]
        public string INP { get; set; } // nvarchar(20)
        [Display(Name = "Номер ОМС")]
        public string InsuranceDocNumber { get; set; } // nvarchar(20)
        [Display(Name = "Серия ОМС")]
        public string InsuranceDocSeries { get; set; } // nvarchar(10)
        [Display(Name = "СМО")]
        public string Insurance { get; set; } // nvarchar(5)
        [Display(Name = "Условия МП")]
        public decimal? AssistanceConditions { get; set; } // numeric(2)
        [Display(Name = "Условия МП наименование")]
        public string AssistanceConditionsName { get; set; } 
        [Display(Name = "Диагноз")]
        public string Diagnosis { get; set; } // nvarchar(10)
        [Display(Name = "Стоимость"), DataType(DataType.Currency)]
        public decimal? Price { get; set; } // money(19,4)
        [Display(Name = "Тариф"), DataType(DataType.Currency)]
        public decimal? Rate { get; set; } // money(19,4)
        [Display(Name = "Кол-во")]
        public decimal? Quantity { get; set; } // money(19,4)
        [Display(Name = "Принято к оплате"), DataType(DataType.Currency)]
        public decimal? AcceptPrice { get; set; } // money(19,4)
        [Display(Name = "Начало случая")]
        public DateTime? EventBegin { get; set; } // datetime(3)
        [Display(Name = "Окончание случая")]
        public DateTime? EventEnd { get; set; } // datetime(3)
        [Display(Name = "Профиль")]
        public decimal? Profile { get; set; } // numeric(3)
        [Display(Name = "Специальность")]
        public decimal? Speciality { get; set; } // numeric(9)
        [Display(Name = "Результат")]
        public decimal? Result { get; set; } // numeric(3)
        [Display(Name = "КСГ")]
        public string Kksg { get; set; } // nvarchar(254)
        [Display(Name = "Исход")]
        public decimal? Outcome { get; set; } // numeric(3)
        [Display(Name = "Статус оплаты")]
        public string PaymentStatus { get; set; } // nvarchar(254)
        [Display(Name = "МЭК"), DataType(DataType.Currency)]
        public decimal? MEC { get; set; } // money(19,4)
        [Display(Name = "МЭЭ"), DataType(DataType.Currency)]
        public decimal? MEE { get; set; } // money(19,4)
        [Display(Name = "ЭКМП"), DataType(DataType.Currency)]
        public decimal? EQMA { get; set; } // money(19,4)
        [Display(Name = "Комментарий к случаю")]
        public string EventComments { get; set; } // nvarchar(250)
        [Display(Name = "Территория выставившая счет")]
        public string SourceName { get; set; } // nvarchar(250)
        [Display(Name = "Территория стархования")]
        public string DestinationName { get; set; } // nvarchar(250)
        [Display(Name = "Версия счета")]
        public string Version { get; set; } // nvarchar(250)

        [Display(Name = "Вид помощи")]
        public decimal? VMPNAME { get; set; }
        [Display(AutoGenerateField = false)]
        public int? InsuranceDocType { get; set; }
        [Display(AutoGenerateField = false)]
        public int? SexCode { get; set; }
        [Display(AutoGenerateField = false)]
        public string TerritoryOkato { get; set; }
        [Display(AutoGenerateField = false)]
        public int? DocType { get; set; }
        [Display(AutoGenerateField = false)]
        public int? PaymentStatusCode { get; set; }
        [Display(AutoGenerateField = false)]
        public decimal? MoPrice { get; set; }
        [Display(AutoGenerateField = false)]
        public int? MoPaymentStatus { get; set; }
        [Display(AutoGenerateField = false)]
        public string EventTypeName { get; set; }
        [Display(AutoGenerateField = false)]
        public string RegionalAttributeName { get; set; }
        [Display(AutoGenerateField = false)]
        public int? EventType { get; set; }
        [Display(AutoGenerateField = false)]
        public int? RegionalAttribute { get; set; }
        [Display(AutoGenerateField = false)]
        public int? TYPE { get; set; }

        [Display(AutoGenerateField = false)]
        public int? Direction { get; set; }

        [Display(AutoGenerateField = false)]
        public int? MedicalExternalId { get; set; }

    }

    public class ZslEventViewMeta
    {
        [Display(Name = "Внешний ID")]
        public object ExternalId { get; set; }
        [Display(Name = "ID счета")]
        public int? AccountId { get; set; } // int(10)
        [Display(Name = "ID счета МО")]
        public int? MedicalAccountId { get; set; } // int(10)
        [Display(Name = "ID законченного случая")]
        public int ZslMedicalEventId { get; set; } // int(10)
        [Display(Name = "ID пациента")]
        public int? PatientId { get; set; } // int(10)
        [Display(Name = "Фамилия")]
        public string Surname { get; set; } // nvarchar(40)
        [Display(Name = "Имя")]
        public string Name { get; set; } // nvarchar(40)
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; } // nvarchar(40)
        [Display(Name = "Дата рождения")]
        public DateTime? Birthday { get; set; } // datetime(3)
        [Display(Name = "Пол")]
        public string Sex { get; set; } // nvarchar(7)
        [Display(Name = "Место рождения")]
        public string BirthPlace { get; set; } // nvarchar(100)
        [Display(Name = "Серия УДЛ")]
        public string DocSeries { get; set; } // nvarchar(10)
        [Display(Name = "Номер УДЛ")]
        public string DocNum { get; set; } // nvarchar(20)
        [Display(Name = "ЕНП")]
        public string INP { get; set; } // nvarchar(20)
        [Display(Name = "Номер ОМС")]
        public string InsuranceDocNumber { get; set; } // nvarchar(20)
        [Display(Name = "Серия ОМС")]
        public string InsuranceDocSeries { get; set; } // nvarchar(10)
        [Display(Name = "СМО")]
        public string Insurance { get; set; } // nvarchar(5)
        [Display(Name = "Условия МП")]
        public decimal? AssistanceConditions { get; set; } // numeric(2)
        [Display(Name = "Диагноз")]
        public string Diagnosis { get; set; } // nvarchar(10)
        [Display(Name = "Стоимость"), DataType(DataType.Currency)]
        public decimal? Price { get; set; } // money(19,4)
        [Display(Name = "Тариф"), DataType(DataType.Currency)]
        public decimal? Rate { get; set; } // money(19,4)
        [Display(Name = "Кол-во")]
        public decimal? Quantity { get; set; } // money(19,4)
        [Display(Name = "Принято к оплате"), DataType(DataType.Currency)]
        public decimal? AcceptPrice { get; set; } // money(19,4)
        [Display(Name = "Начало случая")]
        public DateTime? EventBegin { get; set; } // datetime(3)
        [Display(Name = "Окончание случая")]
        public DateTime? EventEnd { get; set; } // datetime(3)
        [Display(Name = "Профиль")]
        public decimal? Profile { get; set; } // numeric(3)
        [Display(Name = "Специальность")]
        public decimal? Speciality { get; set; } // numeric(9)
        [Display(Name = "Результат")]
        public decimal? Result { get; set; } // numeric(3)
        [Display(Name = "Исход")]
        public decimal? Outcome { get; set; } // numeric(3)
        [Display(Name = "Статус оплаты")]
        public string PaymentStatus { get; set; } // nvarchar(254)
        [Display(Name = "Комментарий к случаю")]
        public string EventComments { get; set; } // nvarchar(250)

        [Display(AutoGenerateField = false)]
        public decimal? AssistanceType { get; set; }
        [Display(AutoGenerateField = false)]
        public int? InsuranceDocType { get; set; }
        [Display(AutoGenerateField = false)]
        public int? SexCode { get; set; }
        [Display(AutoGenerateField = false)]
        public string TerritoryOkato { get; set; }
        [Display(AutoGenerateField = false)]
        public int? DocType { get; set; }

    }

    public partial class FactTerritoryAccount : IDXDataErrorInfo 
    {
        void IDXDataErrorInfo.GetPropertyError(string propertyName, ErrorInfo info)
        {
            
        }

        void IDXDataErrorInfo.GetError(ErrorInfo info)
        {
            if (!EconomicDate.HasValue && Status >= 3)
            {
                SetErrorInfo(info, "Необходимо заполнить экономическую дату", ErrorType.Warning);
            }
                
        }
    }

}
