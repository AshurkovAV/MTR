using System.Xml.Serialization;
using Core.Helpers;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v10.EAnswer
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
                if (!string.IsNullOrWhiteSpace(value))
                    InsuranceDocSeries = value;
            }
        }

        [XmlElement(ElementName = "NPOLIS")]
        public string PolicyNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(INP))
                    return INP.Trim();
                if (!string.IsNullOrWhiteSpace(InsuranceDocNumber))
                    return InsuranceDocNumber.Trim();
                return string.Empty;
            }
            set
            {
                switch (InsuranceDocType)
                {
                    case 1:
                    case 2:
                        InsuranceDocNumber = value;
                        break;
                    case 3:
                        INP = value;
                        break;
                }
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
        
    }
}
