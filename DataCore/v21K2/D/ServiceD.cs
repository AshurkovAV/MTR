using System;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using DataModel;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v21K2.D
{
    public class ServiceD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public ServiceD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "IDSERV")]
        public string ExternalGUID { get; set; } 

        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationCode { get; set; } 

        [XmlElement(ElementName = "LPU_1", IsNullable = false)]
        public string SubdivisionXml
        {
            get { return Subdivision; }
            set { Subdivision = value; }
        }

        [XmlElement(ElementName = "PODR", IsNullable = false)]
        public string MedicalDepartmentCodeXml
        {
            get { return MedicalDepartmentCode; }
            set { MedicalDepartmentCode = value; }
        }

        [XmlElement(ElementName = "PROFIL")]
        public string ProfileCodeIdXml
        {
            get { return Profile.ToStringNullable(); }
            set { Profile = value.ToInt32Nullable(); }
        } 

        [XmlElement(ElementName = "VID_VME")]
        public string SurgeryTypeXml
        {
            get { return SurgeryType; }
            set { SurgeryType = value.TrimNullable(); }
        }

        [XmlElement(ElementName = "DET")]
        public string IsChildrenXml
        {
            get
            {
                return IsChildren.ToStringNullable(); 
            }
            set
            {
                IsChildren = value.ToBoolNullable(); 
            }
        }

        [XmlElement(ElementName = "DATE_IN")]
        public string EventBeginXml
        {
            get { return ServiceBegin.HasValue ? ServiceBegin.Value.ToString("yyyy-MM-dd") : null; }
            set
            {
                ServiceBegin = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        } 
        [XmlElement(ElementName = "DATE_OUT")]
        public string EventEndXml
        {
            get { return ServiceEnd.HasValue ? ServiceEnd.Value.ToString("yyyy-MM-dd") : null; }
            set
            {
                ServiceEnd = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "DS")]
        public string DiagnosisPrimaryXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(Diagnosis); }
            set { Diagnosis = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }

        //by Ira
        [XmlElement(ElementName = "IDKSG", IsNullable = false)]
        public int IDKSG { get; set; }

        [XmlElement(ElementName = "CODE_USL")]
        public string ServiceCode { get; set; }


        [XmlElement(ElementName = "KOL_USL")]
        public string QuantityXml
        {
            get
            {
                return Quantity.ToF2();
            }
            set { Quantity = value.ToDecimalNullable(); }
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
            set { Price = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "PRVS")]
        public string SpecialityCodeV015Xml
        {
            get { return _cacheRepository.Get("V015Cache").GetString(SpecialityCodeV015); }
            set { SpecialityCodeV015 = _cacheRepository.Get("V015Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "CODE_MD", IsNullable = false)]
        public string DoctorIdXml
        {
            get { return DoctorId; }
            set { DoctorId = value.TrimNullable(); }
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

        [XmlElement(ElementName = "USL_PRR", IsNullable = false)]
        public string IsServiceBeforeEventXml
        {
            get { return IsServiceBeforeEvent.ToStringNullable(); }
            set { IsServiceBeforeEvent = value.ToInt32Nullable() == 1; }
        }

        [XmlElement(ElementName = "USL_OTKAZ", IsNullable = false)]
        public string IsServiceRefuseXml
        {
            get { return IsServiceRefuse.ToStringNullable(); }
            set { IsServiceRefuse = value.ToInt32Nullable() == 1; }
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
        public int? SpecialityCodeV015 { get; set; }
        [XmlIgnore]
        public string Subdivision { get; set; } 
        [XmlIgnore]
        public string MedicalDepartmentCode { get; set; }
        [XmlIgnore]
        public string DoctorId { get; set; }

        [XmlIgnore]
        public string SurgeryType { get; set; }
        [XmlIgnore]
        public int? Profile { get; set; }
        [XmlIgnore]
        public bool? IsServiceBeforeEvent { get; set; }
        [XmlIgnore]
        public bool? IsServiceRefuse { get; set; }

        
        [XmlIgnore]
        public long?  Flag { get; set; }
    }

   
}
