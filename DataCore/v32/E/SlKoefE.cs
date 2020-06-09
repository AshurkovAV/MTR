using System.Globalization;
using System.Xml.Serialization;
using Core.Helpers;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v32.E
{
    public class SlKoefE: ISlKoef
    {
        [XmlElement(ElementName = "IDSL")]
        public int? NumberDifficultyTreatment { get; set; }
        [XmlElement(ElementName = "Z_SL")]
        public string ValueDifficultyTreatmentXml
        {
            get
            {
                return ValueDifficultyTreatment.HasValue ? ValueDifficultyTreatment.Value.ToString("F5", CultureInfo.InvariantCulture) : null;
            }
            set { ValueDifficultyTreatment = SafeConvert.ToDecimal(value); }
        }

        [XmlIgnore]
        public decimal? ValueDifficultyTreatment { get; set; }

    }
}
