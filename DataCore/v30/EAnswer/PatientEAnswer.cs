using System.Xml.Serialization;
using Core.Extensions;
using Core.Helpers;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v30.EAnswer
{
    public class PatientEAnswer : IPatientAnswer
    {
        [XmlElement(ElementName = "VPOLIS")]
        public string InsuranceDocTypeXml
        {
            get
            {
                return InsuranceDocType.HasValue ?  InsuranceDocType.ToString() : null;
            }
            set
            {
                InsuranceDocType = SafeConvert.ToInt32(value,false);
            }
        }

        [XmlElement(ElementName = "SPOLIS", IsNullable = false)]
        public string InsuranceDocSeriesXml
        {
            get
            {
                return InsuranceDocSeries;
            }
            set
            {
                InsuranceDocSeries = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "NPOLIS", IsNullable = false)]
        public string InsuranceDocNumberXml
        {
            get
            {
                return InsuranceDocNumber;
            }
            set
            {
                InsuranceDocNumber = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "ENP")]
        public string InpXml
        {
            get
            {
                return INP;
            }
            set
            {
                INP = value.TrimNullable();
            }
        }

        [XmlIgnore]
        public string INP { get; set; }
        [XmlIgnore]
        public string InsuranceDocNumber { get; set; }
        [XmlIgnore]
        public string InsuranceDocSeries { get; set; }
        [XmlIgnore]
        public int? InsuranceDocType { get; set; }

        [XmlIgnore]
        public string PolicyNumber { get; set; }
    }
}
