using System;
using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Общие сведения
    /// </summary>
    public class MedicalAssistanceCommonInfo
    {
        /// <summary>
        /// Код файла 
        /// Уникальный код (например, порядковый номер)
        /// </summary>
        [XmlElement(ElementName = "CODE")]
        public string CodeXml
        {
            get { return Code.ToStringNullable(); }
            set { Code = value.ToInt32Nullable(); }
            
        }

        /// <summary>
        /// Отчётный год
        /// </summary>
        [XmlElement(ElementName = "YEAR")]
        public string YearXml
        {
            get { return Year.ToStringNullable("D4"); }
            set
            {
                Year = value.ToInt32Nullable();
            }
        }

        
        /// <summary>
        /// Отчётный месяц
        /// </summary>
        [XmlElement(ElementName = "MONTH", IsNullable = false)]
        public string MonthXml
        {
            get { return Month.ToStringNullable("D2"); }
            set
            {
                Month = value.ToInt32Nullable();
           }
        }

        /// <summary>
        /// Номер решения комиссии
        /// </summary>
        [XmlElement(ElementName = "NSVD")]
        public string AcceptNumber { get; set; }

        /// <summary>
        /// Дата решения комиссии
        /// Дата утверждения сведений / изменений
        /// </summary>
        [XmlElement(ElementName = "DSVD")]
        public string AcceptDateXml
        {
            get { return AcceptDate.ToFormatString(); }
            set { AcceptDate = value.ToDateTimeExact(); }
        }

        [XmlElement(ElementName = "OBLM")]
        public string MedicalOrganizationExistsXml
        {
            get { return MedicalOrganizationExists.ToStringNullable(); }
            set
            {
                MedicalOrganizationExists = value.ToInt32Nullable();
            }
        }
        

        
        [XmlIgnore]
        public int? Code { get; set; }

        [XmlIgnore]
        public int? Year { get; set; }

        [XmlIgnore]
        public int? Month { get; set; }

        [XmlIgnore]
        public DateTime? AcceptDate { get; set; }

        [XmlIgnore]
        public int? MedicalOrganizationExists { get; set; }
    }
}
