using System;
using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Сведения о случае
    /// </summary>
    public class MedicalAssistanceEvent
    {
        /// <summary>
        /// Номер записи в реестре случаев 
        /// </summary>
        [XmlElement(ElementName = "IDCASE")]
        public string ExternalIdXml
        {
            get { return ExternalId.ToStringNullable(); }
            set { ExternalId = value.ToInt32Nullable(); }
            
        }

        /// <summary>
        /// Условия оказания медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "USL_OK")]
        public string AssistanceConditionsXml
        {
            get { return AssistanceConditions.ToStringNullable(); }
            set
            {
                AssistanceConditions = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Вид медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "VIDPOM")]
        public string AssistanceTypeXml
        {
            get { return AssistanceType.ToStringNullable(); }
            set
            {
                AssistanceType = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Форма оказания медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "FOR_POM")]
        public string AssistanceFormXml
        {
            get { return AssistanceForm.ToStringNullable(); }
            set
            {
                AssistanceForm = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Цель посещения
        /// Заполняется при амбулаторно-поликлинических условиях оказания медицинской помощи.
        /// 1 – заболевание;
        /// 2 – профосмотр;
        /// 0 – иное.
        /// </summary>
        [XmlElement(ElementName = "PCEL", IsNullable = false)]
        public string TargetXml
        {
            get { return Target.ToStringNullable(); }
            set
            {
                Target = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Вид высокотехнологичной медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "VID_HMP", IsNullable = false)]
        public string HighTechAssistanceType { get; set; }

        /// <summary>
        /// Метод высокотехнологичной медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "METOD_HMP", IsNullable = false)]
        public string HighTechAssistanceMethodXml
        {
            get { return HighTechAssistanceMethod.ToStringNullable(); }
            set
            {
                HighTechAssistanceMethod = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Код МО
        /// Реестровый номер медицинской организации
        /// </summary>
        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationCode { get; set; }

        /// <summary>
        /// Код профиля медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "PROFIL")]
        public string ProfileXml
        {
            get { return Profile.ToStringNullable(); }
            set
            {
                Profile = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Срок лечения
        /// </summary>
        [XmlElement(ElementName = "DATE_I")]
        public string DaysXml
        {
            get { return Days.ToStringNullable(); }
            set
            {
                Days = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Сумма, принятая к оплате СМО (ТФОМС)
        /// Сумма фактической оплаты
        /// </summary>
        [XmlElement(ElementName = "SUM")]
        public string AcceptPriceXml
        {
            get { return AcceptPrice.ToF2(); }
            set
            {
                AcceptPrice = value.ToDecimalNullable();
            }
        }


        [XmlIgnore]
        public int? ExternalId { get; set; }

        [XmlIgnore]
        public int? AssistanceConditions { get; set; }

        [XmlIgnore]
        public int? AssistanceType { get; set; }

        [XmlIgnore]
        public int? AssistanceForm { get; set; }

        [XmlIgnore]
        public int? Target { get; set; }

        [XmlIgnore]
        public int? HighTechAssistanceMethod { get; set; }

        [XmlIgnore]
        public int? Profile { get; set; }

        [XmlIgnore]
        public int? Days { get; set; }

        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }

        [XmlIgnore]
        public DateTime? AcceptDate { get; set; }
    }
}
