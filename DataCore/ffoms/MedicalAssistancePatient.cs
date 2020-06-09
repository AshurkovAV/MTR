using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Сведения о пациенте
    /// </summary>
    public class MedicalAssistancePatient
    {
        /// <summary>
        /// ОКАТО территории страхования
        /// </summary>
        [XmlElement(ElementName = "SMO_OK")]
        public string InsuranceOkato { get; set; }

        /// <summary>
        /// Пол пациента
        /// </summary>
        [XmlElement(ElementName = "W")]
        public string SexXml
        {
            get { return Sex.ToStringNullable(); }
            set
            {
                Sex = value.ToInt32Nullable();
            }
        }

        /// <summary>
        /// Возраст пациента
        /// Полных лет. Для детей до года указывается «0»
        /// </summary>
        [XmlElement(ElementName = "VZST")]
        public string AgeXml
        {
            get { return Age.ToStringNullable(); }
            set
            {
                Age = value.ToInt32Nullable();
            }
        }

        [XmlIgnore]
        public int? Sex { get; set; }

        [XmlIgnore]
        public int? Age { get; set; }

    }
}
