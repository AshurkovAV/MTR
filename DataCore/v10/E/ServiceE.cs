using System;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v10.E
{
    public class ServiceE : IService
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public ServiceE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "IDSERV")]
        public int ExternalId { get; set; } 

        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationXml
        {
            get
            {
                return MedicalOrganization.HasValue ? MedicalOrganization.Value.ToString(CultureInfo.InvariantCulture) : null;
            }
            set
            {
                MedicalOrganization = SafeConvert.ToInt32(value,false);
            }
        } 

        [XmlElement(ElementName = "PROFIL")]
        public int? Profile { get; set; }

        [XmlElement(ElementName = "DET")]
        public int IsChildrenXml
        {
            get { return IsChildren.HasValue && IsChildren.Value ? 1 : 0; }
            set { IsChildren = value == 1; }
        }

        [XmlElement(ElementName = "DATE_IN")]
        public string EventBeginXml
        {
            get
            {
                return ServiceBegin.HasValue ? ServiceBegin.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                ServiceBegin = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        } 
        [XmlElement(ElementName = "DATE_OUT")]
        public string EventEndXml
        {
            get
            {
                return ServiceEnd.HasValue ? ServiceEnd.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                ServiceEnd = SafeConvert.ToDateTimeExact(value,"yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "DS", IsNullable = false)]
        public string DiagnosisXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(Diagnosis); }
            set { Diagnosis = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "USL")]
        public string ServiceName { get; set; }


        [XmlElement(ElementName = "KOL_USL")]
        public string QuantityXml
        {
            get
            {
                return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Quantity = SafeConvert.ToDecimal(value);
            }
        }

        [XmlElement(ElementName = "TARIF")]
        public string RateXml
        {
            get
            {
                return Rate.HasValue ? Rate.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Rate = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SUMV_USL")]
        public string PriceXml
        {
            get
            {
                return Price.HasValue ? Price.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Price = SafeConvert.ToDecimal(value);
            }
        }

        [XmlElement(ElementName = "PRVS")]
        public string SpecialityCodeXml
        {
            get { return _cacheRepository.Get("V004Cache").GetString(SpecialityCode); }
            set { SpecialityCode = _cacheRepository.Get("V004Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "COMENTU", IsNullable = false)]
        public string CommentsXml
        {
            get
            {
                return Comments;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comments = value.Trim();
            }
        }
       
        [XmlIgnore]
        public bool? IsChildren { get; set; } 
        [XmlIgnore]
        public DateTime? ServiceBegin { get; set; } 
        [XmlIgnore]
        public DateTime? ServiceEnd { get; set; } 
        [XmlIgnore]
        public int? Diagnosis { get; set; } 
        [XmlIgnore]
        public decimal? Rate { get; set; } 
        [XmlIgnore]
        public decimal? Price { get; set; } 
        [XmlIgnore]
        public decimal? Quantity { get; set; } 
        [XmlIgnore]
        public string Comments { get; set; }

        [XmlIgnore]
        public string MedicalOrganizationCode { get; set; }

        [XmlIgnore]    
        public int? SpecialityCode { get; set; }
        [XmlIgnore]   
        public int? MedicalOrganization { get; set; } 
    }

   
}
