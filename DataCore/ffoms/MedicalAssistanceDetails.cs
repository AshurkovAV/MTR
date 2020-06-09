using System.Collections.Generic;
using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Сведения об объёмах и стоимости
    /// </summary>
    public class MedicalAssistanceDetails
    {
        /// <summary>
        /// Номер позиции записи
        /// Уникально идентифицирует запись в пределах файла
        /// </summary>
        [XmlElement(ElementName = "N_SV")]
        public string PositionNumberXml
        {
            get { return PositionNumber.ToStringNullable(); }
            set { PositionNumber = value.ToInt32Nullable(); }
            
        }

        /// <summary>
        /// Код МО
        /// Реестровый номер медицинской организации
        /// </summary>
        [XmlElement(ElementName = "MO_SV")]
        public string MedicalOrganization { get; set; }

        /// <summary>
        /// Показатели
        /// Повторяется для каждого показателя
        /// </summary>
        [XmlElement(ElementName = "IT_SV")]
        public List<MedicalAssistanceIndicators> IndicatorsCollection { get; set; }

        public MedicalAssistanceDetails()
        {
            IndicatorsCollection = new List<MedicalAssistanceIndicators>();
        }

        
        [XmlIgnore]
        public int? PositionNumber { get; set; }

        
    }
}
