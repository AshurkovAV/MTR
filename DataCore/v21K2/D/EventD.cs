using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v21K2.D
{
    public class EventD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "IDCASE")]
        public int MedicalExternalId { get; set; }

        [XmlElement(ElementName = "USL_OK")]
        public string AssistanceConditionsXml
        {
            get
            {
                return AssistanceConditions.HasValue ? AssistanceConditions.ToString() : null;
            }
            set { AssistanceConditions = SafeConvert.ToInt32(value); }
        } 

        [XmlElement(ElementName = "VIDPOM")]
        public int? AssistanceType { get; set; }

        [XmlElement(ElementName = "FOR_POM")]
        public string AssistanceFormXml
        {
            get { return AssistanceForm.HasValue ? AssistanceType.ToString() : null; }
            set { AssistanceForm = SafeConvert.ToInt32(value, false); }
        }

        [XmlElement(ElementName = "VID_HMP")]
        public string HighTechAssistanceTypeXml
        {
            get { return HighTechAssistanceType; }
            set { HighTechAssistanceType = value; }
        }

        [XmlElement(ElementName = "METOD_HMP")]
        public string HighTechAssistanceMethodXml
        {
            get { return HighTechAssistanceMethod.HasValue ? HighTechAssistanceMethod.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { HighTechAssistanceMethod = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "NPR_MO", IsNullable = false)]
        public string ReferralOrganizationNullable
        {
            get { return _cacheRepository.Get("F003Cache").GetString(ReferralOrganization); }
            set { ReferralOrganization = _cacheRepository.Get("F003Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "EXTR", IsNullable = false)]
        public string HospitalizationNullable
        {
            get
            {
                return Hospitalization.HasValue ? Hospitalization.ToString() : null;
            }
            set
            {
                
                Hospitalization = SafeConvert.ToDecimal(value);
            }
        }
        
        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationCode { get; set; }

        [XmlElement(ElementName = "LPU_1", IsNullable = false)]
        public string SubdivisionXml
        {
            get { return Subdivision; }
            set { Subdivision = value; }
        }

        [XmlElement(ElementName = "PODR")]
        public string DepartmentXml
        {
            get
            {
                return Department.HasValue ? Department.Value.ToString(CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Department = SafeConvert.ToInt32(value);
            }
        }

        [XmlElement(ElementName = "PROFIL")]
        public int? ProfileCodeId { get; set; }
        [XmlElement(ElementName = "DET")]
        public int IsChildrenXml
        {
            get { return IsChildren.HasValue && IsChildren.Value ? 1 : 0; }
            set { IsChildren = value == 1; }
        } 
        [XmlElement(ElementName = "NHISTORY")]
        public string HistoryXml
        {
            get { return History; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                    History = value.Trim();
            }
        } 
        [XmlElement(ElementName = "DATE_1")]
        public string EventBeginXml
        {
            get
            {
                return EventBegin.HasValue ? EventBegin.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                EventBegin = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        } 
        [XmlElement(ElementName = "DATE_2")]
        public string EventEndXml
        {
            get
            {
                return EventEnd.HasValue ? EventEnd.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                EventEnd = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "DS0", IsNullable = false)]
        public string DiagnosisPrimaryXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(DiagnosisPrimary); }
            set { DiagnosisPrimary = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }


        [XmlElement(ElementName = "DS1")]
        public string DiagnosisGeneralXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(DiagnosisGeneral); }
            set { DiagnosisGeneral = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "DS2", IsNullable = false)]
        public List<string> DiagnosisSecondaryXml
        {
            get
            {
                return DiagnosisSecondary;
            }
            set
            {
                DiagnosisSecondary = value;
            }
        }

        [XmlElement(ElementName = "DS3", IsNullable = false)]
        public List<string> DiagnosisComplicationXml { get; set; }

        [XmlElement(ElementName = "VNOV_M")]
        public List<string> NewbornsWeightXml
        {
            get
            {
                return NewbornsWeight;
            }
            set
            {
                NewbornsWeight = value;
            }
        }

        [XmlElement(ElementName = "CODE_MES1", IsNullable = false)]
        public List<string> MESXml { get; set; }

        [XmlElement(ElementName = "CODE_MES2", IsNullable = false)]
        public string SecondaryMESXml
        {
            get { return SecondaryMES; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    SecondaryMES = value.Trim();
            }
        }

        [XmlElement(ElementName = "SLUCH_TYPE")]
        public string EventTypeXml
        {
            get { return EventType.HasValue ? EventType.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { EventType = SafeConvert.ToInt32(value); }
        }
        

        [XmlElement(ElementName = "RSLT")]
        public string ResultXml
        {
            get { return _cacheRepository.Get("V009Cache").GetString(Result); }
            set { Result = _cacheRepository.Get("V009Cache").GetBack(value); }
        }
        [XmlElement(ElementName = "ISHOD")]
        public string OutcomeXml
        {
            get { return _cacheRepository.Get("V012Cache").GetString(Outcome); }
            set { Outcome = _cacheRepository.Get("V012Cache").GetBack(value); }
        } 
        [XmlElement(ElementName = "PRVS")]
        public string SpecialityCodeXml { get; set; }

        [XmlElement(ElementName = "VERS_SPEC")]
        public string SpecialityClassifierVersion { get; set; }
  
        [XmlElement(ElementName = "IDDOKT")]
        public string DoctorIdXml
        {
            get { return DoctorId; }
            set { DoctorId = value; }
        }

        [XmlElement(ElementName = "OS_SLUCH")]
        public List<string> ParticularCaseCollection { get; set; }

        [XmlElement(ElementName = "OS_SLUCH_REGION", IsNullable = false)]
        public string RegionalAttributeXml {
            get { return RegionalAttribute.HasValue ? RegionalAttribute.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { RegionalAttribute = SafeConvert.ToInt32(value); }
        }

        [XmlIgnore]
        public int? RegionalAttribute { get; set; }

        [XmlElement(ElementName = "VETERAN")]
        public int? Veteran { get; set; }

        [XmlElement(ElementName = "GR_ZDOROV")]
        public string HealthGroupXml {
            get { return HealthGroup.HasValue ? HealthGroup.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { HealthGroup = SafeConvert.ToInt32(value); }
        }

        [XmlIgnore]
        public int? HealthGroup { get; set; }

        [XmlElement(ElementName = "SCHOOL")]
        public int? School { get; set; }

        [XmlElement(ElementName = "WORK_STAT")]
        public int? JobStatus_Id { get; set; }

        [XmlElement(ElementName = "IDSP")]
        public int? PaymentMethod { get; set; }

        [XmlElement(ElementName = "IDKSG")]
        public int? IdKsg { get; set; }

        [XmlElement(ElementName = "ED_COL")]
        public string QuantityXml
        {
            get
            {
                return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Quantity = SafeConvert.ToDecimal(value); }
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

        [XmlElement(ElementName = "SUMV")]
        public string PriceXml
        {
            get
            {
                return Price.HasValue ? Price.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Price = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "OPLATA", IsNullable = false)]
        public string PaymentStatusXml {
            get { return _cacheRepository.Get("F005Cache").GetString(PaymentStatus); }
            set { PaymentStatus = _cacheRepository.Get("F005Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SUMP", IsNullable = false)]
        public string AcceptPriceXml
        {
            get
            {
                return AcceptPrice.HasValue ? AcceptPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { AcceptPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SANK_IT", IsNullable = false)]
        public string RefusalPriceXml
        {
            get { return RefusalPrice.HasValue ? RefusalPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { RefusalPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SANK", IsNullable = false)]
        public List<RefusalD> RefusalsXml
        {
            get { return Refusals; }
            set { Refusals = value; }
        }

        [XmlElement(ElementName = "USL")]
        public List<ServiceD> Service { get; set; }

        [XmlElement(ElementName = "COMENTSL", IsNullable = false)]
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
        public int? AssistanceConditions { get; set; }
        [XmlIgnore]
        public int? Department { get; set; }
        [XmlIgnore]
        public int? ReferralOrganization { get; set; } 
        [XmlIgnore]
        public decimal? Hospitalization { get; set; } 
        [XmlIgnore]
        public bool? IsChildren { get; set; } 
        [XmlIgnore]
        public string History { get; set; } 
        [XmlIgnore]
        public DateTime? EventBegin { get; set; } 
        [XmlIgnore]
        public DateTime? EventEnd { get; set; } 
        [XmlIgnore]
        public int? DiagnosisPrimary { get; set; } 
        [XmlIgnore]
        public int? DiagnosisGeneral { get; set; } 
        [XmlIgnore]
        public List<int> DiagnosisSecondaryList { get; set; }
        [XmlIgnore]
        public decimal? Rate { get; set; }
        [XmlIgnore]
        public decimal? Price { get; set; } 
        [XmlIgnore]
        public decimal? Quantity { get; set; } 
        [XmlIgnore]
        public string Comments { get; set; } 
        [XmlIgnore]
        public int? Result { get; set; } 
        [XmlIgnore]
        public int? Outcome { get; set; }
        [XmlIgnore]
        public int? SpecialityCode
        {
            get
            {
                if (SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V004")
                {
                    return _cacheRepository.Get("V004Cache").GetBack(SpecialityCodeXml);
                }
                return null;
            }
            set
            {
                if (SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V004")
                {
                    SpecialityCodeXml = _cacheRepository.Get("V004Cache").GetString(value);
                }
            }
        }

        [XmlIgnore]
        public int? SpecialityCodeV015
        {
            get
            {
                if (!SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V015")
                {
                    return _cacheRepository.Get("V015Cache").GetBack(SpecialityCodeXml);
                }
                return null;
            }
            set
            {
                if (SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V015")
                {
                    SpecialityCodeXml = _cacheRepository.Get("V015Cache").GetString(value);
                }
            }
        }
        [XmlIgnore]
        public string DoctorId { get; set; }
        [XmlIgnore]
        public string Subdivision { get; set; }
  
        [XmlIgnore]
        public int? PaymentStatus { get; set; }
        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }

        [XmlIgnore]
        public int? AssistanceForm { get; set; }

        [XmlIgnore]
        public string HighTechAssistanceType { get; set; }

        [XmlIgnore]
        public int? HighTechAssistanceMethod { get; set; }

        [XmlIgnore]
        public List<RefusalD> Refusals { get; set; }

        [XmlIgnore]
        public List<string> DiagnosisSecondary { get; set; }
        
        [XmlIgnore]
        public string DiagnosisSecondaryAggregate
        {
            get { return DiagnosisSecondary.AggregateToString(); }
            set { DiagnosisSecondary = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string DiagnosisComplicationAggregate
        {
            get { return DiagnosisComplicationXml.AggregateToString(); }
            set { DiagnosisComplicationXml = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string MESAggregate
        {
            get { return MESXml.AggregateToString(); }
            set { MESXml = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string NewbornsWeightAggregate
        {
            get { return NewbornsWeight.AggregateToString(); }
            set { NewbornsWeight = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string SecondaryMES { get; set; }

        [XmlIgnore]
        public List<string> NewbornsWeight { get; set; }

        [XmlIgnore]
        public int? EventType { get; set; }
        
        [XmlIgnore]
        public decimal? RefusalPrice { get; set; }


        [XmlElement(ElementName = "LOCAL")]
        public List<LocalElement> LocalElementCollection { get; set; }
        
    }

    public class ParticularCase
    {
        [XmlElement(ElementName = "OS_SLUCH")]
        public string ParticularCaseId { get; set; } // nvarchar(250)
    }

    public class LocalElement
    {
        [XmlElement(ElementName = "RateParticularSign")]
        public string RateParticularSign { get; set; } // nvarchar(50)
    }

}
