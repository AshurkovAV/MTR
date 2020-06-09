using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Записи
    /// </summary>
    public class MedicalAssistanceRecords
    {
        /// <summary>
        /// Номер позиции записи
        /// Уникально идентифицирует запись в пределах файла
        /// </summary>
        [XmlElement(ElementName = "N_ZAP")]
        public string PositionNumberXml
        {
            get { return PositionNumber.ToStringNullable(); }
            set { PositionNumber = value.ToInt32Nullable(); }
        }

        /// <summary>
        /// Сведения о пациенте
        /// В одной записи может указываться только один случай оказания медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "PACIENT")]
        public MedicalAssistancePatient Patient { get; set; }

        /// <summary>
        /// Сведения о случае
        /// В одной записи может указываться только один случай оказания медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "SLUCH")]
        public MedicalAssistanceEvent MedicalEvent { get; set; }

        [XmlIgnore]
        public int? PositionNumber { get; set; }
    }
}
