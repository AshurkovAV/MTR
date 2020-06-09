using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Сведения об оказанной медицинской помощи
    /// </summary>
    [XmlRoot("ISP_OB", IsNullable = true, Namespace = "")]
    public class MedicalAssistanceCompleted
    {
        /// <summary>
        /// Заголовок файла
        /// Информация о файле
        /// </summary>
        [XmlElement(ElementName = "ZGLV")]
        public MedicalAssistanceHeader Header { get; set; }

        /// <summary>
        /// Общие сведения
        /// </summary>
        [XmlElement(ElementName = "SVD")]
        public MedicalAssistanceCommonInfo CommonInfo { get; set; }
        
        /// <summary>
        /// Сведения об объёмах и стоимости
        /// Повторяется для каждой медицинской организации
        /// </summary>
        [XmlElement(ElementName = "OB_SV")]
        public List<MedicalAssistanceDetails> DetailsCollection { get; set; }

        /// <summary>
        /// Записи
        /// Записи о случаях оказания медицинской помощи
        /// Заполняется для файла со сведениями об оказанной медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "ZAP", IsNullable = false)]
        public List<MedicalAssistanceRecords> RecordsCollection { get; set; }

        public MedicalAssistanceCompleted()
        {
            DetailsCollection = new List<MedicalAssistanceDetails>();
        }
        
    }
}
