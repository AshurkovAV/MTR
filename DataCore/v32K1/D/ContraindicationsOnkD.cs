using System;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v32K1.D
{
    /// <summary>
    /// B_PROT Сведения об имеющихся противопоказаниях и отказах
    /// </summary>
    public class ContraindicationsOnkD
    {
        public ContraindicationsOnkD()
        {
        }

        [XmlElement(ElementName = "PROT")]
        public string ProtXml
        {
            get { return Prot.ToStringNullable(); }
            set { Prot = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "D_PROT")]
        public string DprotXml
        {
            get { return Dprot.ToFormatString(); }
            set { Dprot = value.ToDateTimeExact(); }
        }

        [XmlIgnore]
        public int? Prot { get; set; }

        [XmlIgnore]
        public DateTime? Dprot { get; set; }

    }
}
