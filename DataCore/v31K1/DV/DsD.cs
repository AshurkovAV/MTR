using System.Xml.Serialization;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v31K1.DV
{
    public class DsD 
    {
        [XmlElement(ElementName = "DS2")]
        public string Ds { get; set; }
        [XmlElement(ElementName = "DS2_PR")]
        public int? DsPr { get; set; }
        [XmlElement(ElementName = "DS_TYPE")]
        public int? DsType { get; set; }
        [XmlElement(ElementName = "PR_DS2_N")]
        public int? PrDs2N { get; set; }

    }
}
