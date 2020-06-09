using System.Collections.Generic;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v21K2.D
{
    public class PatientD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public PatientD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        /// <summary>
        /// Код записи о пациенте
        /// Необходим для связи с файлом персональных данных
        /// T(36)
        /// </summary>
        [XmlElement(ElementName = "ID_PAC")]    
        public string PatientGuid { get; set; }

        [XmlElement(ElementName = "VPOLIS")]
        public string InsuranceDocTypeXml
        {
            get { return InsuranceDocType.ToStringNullable(); }
            set { InsuranceDocType = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "SPOLIS", IsNullable = false)]
        public string InsuranceDocSeriesXml
        {
            get { return InsuranceDocSeries; }
            set { InsuranceDocSeries = value.TrimNullable(); }
        }

        [XmlElement(ElementName = "NPOLIS")]
        public string PolicyNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(INP))
                    return INP.Trim();

                return InsuranceDocNumber;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                switch (InsuranceDocType)
                {
                    case 1:
                    case 2:
                        InsuranceDocNumber = value.TrimNullable();
                        break;
                    case 3:
                        INP = value.TrimNullable();
                        break;
                    default:
                        InsuranceDocNumber = value.TrimNullable();
                        break;
                }
            }
        }

        [XmlElement(ElementName = "SMO", IsNullable = false)]
        public string InsuranceIdXml
        {
            get { return _cacheRepository.Get("F002Cache").GetString(InsuranceId); }
            set { InsuranceId = _cacheRepository.Get("F002Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SMO_OGRN", IsNullable = false)]
        public string InsuranceOgrnXml
        {
            get
            {
                return InsuranceOgrn;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    InsuranceOgrn = value.Trim();
            }
        }

        [XmlElement(ElementName = "SMO_OK", IsNullable = false)]
        public string TerritoryOkatoXml
        {
            get { return _cacheRepository.Get("F010Cache").GetString(TerritoryOkato); }
            set { TerritoryOkato = _cacheRepository.Get("F010Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SMO_NAM", IsNullable = false)]
        public string InsuranceNameXml
        {
            get
            {
                return InsuranceName;
            }
            set
            {
                InsuranceName = value.TrimNullable().ReplaceByList(new List<string> {"<", ">", "&", "\'", "\""});
            }
        }
        [XmlElement(ElementName = "NOVOR")]
        public string Newborn { get; set; }

        [XmlElement(ElementName = "VNOV_D", IsNullable = false)]
        public string NewbornWeightXml
        {
            get { return NewbornWeight.ToStringNullable(); }
            set { NewbornWeight = value.ToInt32Nullable(); }
        } 

        [XmlIgnore]
        public string InsuranceName { get; set; } 
        [XmlIgnore]
        public int? TerritoryOkato { get; set; } 
        [XmlIgnore]
        public string InsuranceOgrn { get; set; }
        [XmlIgnore]
        public int? InsuranceId { get; set; }
        [XmlIgnore]
        public string InsuranceDocSeries { get; set; }

        [XmlIgnore]
        public string INP { get; set; } 
        [XmlIgnore]
        public string InsuranceDocNumber { get; set; } 

        [XmlIgnore]
        public int? NewbornWeight { get; set; } 

        [XmlIgnore]
        public int? InsuranceDocType { get; set; }

        
    }
}
