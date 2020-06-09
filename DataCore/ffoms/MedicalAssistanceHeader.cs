using System;
using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Заголовок файла
    /// </summary>
    public class MedicalAssistanceHeader
    {
        /// <summary>
        /// Версия взаимодействия 
        /// Текущей редакции соответствует значение «1.0»
        /// </summary>
        [XmlElement(ElementName = "VERSION")]
        public string Version { get; set; }

        /// <summary>
        /// Дата файла
        /// В формате ГГГГ-ММ-ДД
        /// </summary>
        [XmlElement(ElementName = "DATA")]
        public string DateXml
        {
            get { return Date.ToFormatString(); }
            set { Date = value.ToDateTimeExact(); }
        }

        /// <summary>
        /// Имя файла
        /// Имя файла без расширения
        /// </summary>
        [XmlElement(ElementName = "FILENAME")]
        public string Filename { get; set; }

        /// <summary>
        /// Имя исходного файла
        /// Заполняется для файлов с исправлениями. Указывается имя основного файла, к которому применены исправления
        /// </summary>
        [XmlElement(ElementName = "FIRSTNAME", IsNullable = false)]
        public string Firstname { get; set; }

        [XmlIgnore]
        public DateTime? Date { get; set; }
    }
}
