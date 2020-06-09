using System.Xml.Serialization;

namespace Medical.DataCore.v32K1.D
{
    public class DiagnosisSecondaryD
    {
        [XmlElement(ElementName = "DS2")]
        public string Diagnosis { get; set; }
        [XmlElement(ElementName = "DS2_PR", IsNullable = false)]
        public string Ds2Pr { get; set; }
        [XmlElement(ElementName = "PR_DS2_N", IsNullable = false)]
        public string PrDs2N { get; set; }
    }
}
