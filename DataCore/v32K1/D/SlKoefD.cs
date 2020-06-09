using System.Xml.Serialization;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v32K1.D
{
    public class SlKoefD : ISlKoef
    {
        [XmlElement(ElementName = "IDSL")]
        public int? NumberDifficultyTreatment { get; set; }
        [XmlElement(ElementName = "Z_SL")]
        public decimal? ValueDifficultyTreatment { get; set; }

    }
}
