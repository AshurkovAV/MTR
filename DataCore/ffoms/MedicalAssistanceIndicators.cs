using System.Collections.Generic;
using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Показатели
    /// </summary>
    public class MedicalAssistanceIndicators
    {
        /// <summary>
        /// Код показателя
        /// </summary>
        [XmlElement(ElementName = "OT_NAIM")]
        public string IndicatorCodeXml
        {
            get { return IndicatorCode.ToStringNullable(); }
            set { IndicatorCode = value.ToInt32Nullable(); }

        }

        /// <summary>
        /// Профили медицинской помощи
        /// Повторяется для каждого профиля
        /// </summary>
        [XmlElement(ElementName = "PR_SV")]
        public List<MedicalAssistanceProfiles> ProfilesCollection { get; set; }

        public MedicalAssistanceIndicators()
        {
            ProfilesCollection = new List<MedicalAssistanceProfiles>();
        }

        [XmlIgnore]
        public int? IndicatorCode { get; set; }
    }
}
