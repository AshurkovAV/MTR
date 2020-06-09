using System;
using System.Xml.Serialization;
using Core.Extensions;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v32.E
{
    /// <summary>
    /// B_PROT
    /// </summary>
    public class ContraindicationsOnkE: IContraindicationsOnk
    {
        public ContraindicationsOnkE()
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
