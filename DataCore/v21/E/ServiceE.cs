using System;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v21.E
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
        public string ExternalIdXml {
            get
            {
                if (ExternalGUID.IsNotNullOrEmpty())
                {
                    return ExternalGUID;
                }
                return ExternalId.ToStringNullable();
            }
            set
            {
                ExternalId = value.TrimNullable().ToInt32Nullable();
                if (ExternalId == null)
                {
                    ExternalGUID = value;
                }
            }
        } 

        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationXml
        {
            get
            {
                return MedicalOrganization.HasValue ? MedicalOrganization.Value.ToString(CultureInfo.InvariantCulture) : null;
            }
            set
            {
                MedicalOrganization = SafeConvert.ToInt32(value.TrimNullable(), false);
            }
        } 

        [XmlElement(ElementName = "PROFIL")]
        public int? Profile { get; set; }

        [XmlElement(ElementName = "VID_VME")]
        public string SurgeryTypeXml
        {
            get { return SurgeryType; }
            set { SurgeryType = value.TrimNullable(); }
        }
        

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
                ServiceBegin = SafeConvert.ToDateTimeExact(value.TrimNullable(), "yyyy-MM-dd");
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
                ServiceEnd = SafeConvert.ToDateTimeExact(value.TrimNullable(), "yyyy-MM-dd");
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
                Quantity = SafeConvert.ToDecimal(value.TrimNullable());
            }
        }

        [XmlElement(ElementName = "TARIF")]
        public string RateXml
        {
            get
            {
                return Rate.HasValue ? Rate.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Rate = SafeConvert.ToDecimal(value.TrimNullable()); }
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
                Price = SafeConvert.ToDecimal(value.TrimNullable());
            }
        }

        [XmlElement(ElementName = "PRVS")]
        public string SpecialityCodeXml { 
            get
            {
                return SpecialityCode.HasValue ? SpecialityCode.ToString() : null;
                //return UnityService.Instance.Resolve<ClassifierManager>().IdToString<V004>(SpecialityCode, "Id", "IDMSP");
            }
            set
            {
                SpecialityCode = value.TrimNullable().ToInt32Nullable();
                //SpecialityCode = UnityService.Instance.Resolve<ClassifierManager>().ObjectToId<V004>(Convert.ToDecimal(value), "IDMSP", "Id");

            }
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
                Comments = value.TrimNullable();
            }
        }

        [XmlIgnore]
        public int? ExternalId { get; set; }
        [XmlIgnore]
        public string ExternalGUID { get; set; }

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

        [XmlIgnore]
        public string SurgeryType { get; set; } 
        
    }

   
}
