using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v21.E
{
    public class EventE : IMEvent
    {
        #region Inner
        [XmlIgnore]
        public List<IService> InnerServiceCollection 
        {
            get { return _serviceCollection.Select(p => p as IService).ToList(); }
            set { 
                _serviceCollection = value.Select(p => p as ServiceE).ToList(); 
            }
        }
        [XmlIgnore]
        public List<object> InnerRefusals {
            get { return _refusals.Select(p=>p as object).ToList(); }
            set { _refusals = value.Select(p=>p as RefusalE).ToList(); }
        }

        [XmlIgnore]
        public List<ServiceE> _serviceCollection { get; set; }

        [XmlIgnore]
        public List<RefusalE> _refusals { get; set; }


        #endregion

         [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

         public EventE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            _serviceCollection = new List<ServiceE>();
            InnerRefusals = new List<object>();
        }

        [XmlElement(ElementName = "IDCASE")]
        public int ExternalId { get; set; }

        [XmlElement(ElementName = "USL_OK")]
        public string AssistanceConditionsXml
        {
            get { return AssistanceConditions.HasValue ? AssistanceConditions.ToString() : null; }
            set { AssistanceConditions = SafeConvert.ToInt32(value.TrimNullable(), false); }
        }

        [XmlElement(ElementName = "VIDPOM")]
        public string AssistanceTypeXml
        {
            get { return AssistanceType.HasValue ? AssistanceType.ToString() : null; }
            set { AssistanceType = SafeConvert.ToInt32(value.TrimNullable(), false); }
        }

        [XmlElement(ElementName = "FOR_POM")]
        public string AssistanceFormXml
        {
            get { return AssistanceForm.HasValue ? AssistanceForm.ToString() : null; }
            set { AssistanceForm = SafeConvert.ToInt32(value.TrimNullable(), false); }
        }

        [XmlElement(ElementName = "VID_HMP")]
        public string HighTechAssistanceTypeXml
        {
            get { return HighTechAssistanceType; }
            set { HighTechAssistanceType = value.TrimNullable(); }
        }

        [XmlElement(ElementName = "METOD_HMP")]
        public string HighTechAssistanceMethodXml
        {
            get { return HighTechAssistanceMethod.ToStringNullable(); }
            set { HighTechAssistanceMethod = value.TrimNullable().ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "EXTR", IsNullable = false)]
        public string HospitalizationNullable
        {
            get { return Hospitalization.HasValue ? Hospitalization.ToString() : null; }
            set { Hospitalization = SafeConvert.ToDecimal(value.TrimNullable()); }
        }

        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationCodeXml
        {
            get { return MedicalOrganizationCode; }
            set
            {
                MedicalOrganizationCode = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "PROFIL")]
        public string ProfileCodeIdXml
        {
            get { return ProfileCodeId.HasValue ? ProfileCodeId.ToString() : null; }
            set { ProfileCodeId = SafeConvert.ToInt32(value.TrimNullable(), false); }
        }

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
                History = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "DATE_1")]
        public string EventBeginXml
        {
            get { return EventBegin.HasValue ? EventBegin.Value.ToString("yyyy-MM-dd") : null; }
            set { EventBegin = SafeConvert.ToDateTimeExact(value.TrimNullable(), "yyyy-MM-dd"); }
        }

        [XmlElement(ElementName = "DATE_2")]
        public string EventEndXml
        {
            get { return EventEnd.HasValue ? EventEnd.Value.ToString("yyyy-MM-dd") : null; }
            set { EventEnd = SafeConvert.ToDateTimeExact(value.TrimNullable(), "yyyy-MM-dd"); }
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
                SecondaryMES = value.TrimNullable();
            }
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
        public string SpecialityClassifierVersion
        { 
            get;set;
        }

        [XmlElement(ElementName = "IDSP")]
        public string PaymentMethodXml
        {
            get { return PaymentMethod.HasValue ? PaymentMethod.ToString() : null; }
            set { PaymentMethod = SafeConvert.ToInt32(value.TrimNullable(), false); }
        }

        [XmlElement(ElementName = "ED_COL", IsNullable = false)]
        public string QuantityXml
        {
            get { return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { Quantity = SafeConvert.ToDecimal(value.TrimNullable()); }
        }

        [XmlElement(ElementName = "TARIF", IsNullable = false)]
        public string RateXml
        {
            get { return Rate.HasValue ? Rate.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { Rate = SafeConvert.ToDecimal(value.TrimNullable()); }
        }

        [XmlElement(ElementName = "SUMV")]
        public string PriceXml
        {
            get { return Price.ToF2(); }
            set { Price = value.ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "OPLATA", IsNullable = false)]
        public string PaymentStatusXml
        {
            get { return _cacheRepository.Get("F005Cache").GetString(PaymentStatus); }
            set { PaymentStatus = _cacheRepository.Get("F005Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SUMP")]
        public string AcceptPriceXml
        {
            get { return AcceptPrice.ToF2(); }
            set { AcceptPrice = value.TrimNullable().ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "SANK_IT", IsNullable = false)]
        public string RefusalPriceXml
        {
            get { return RefusalPrice.HasValue ? RefusalPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { RefusalPrice = SafeConvert.ToDecimal(value.TrimNullable()); }
        }

        [XmlElement(ElementName = "SANK", IsNullable = false)]
        public List<RefusalE> RefusalsXml
        {
            get { return _refusals; }
            set { _refusals = value; }
        }

        [XmlElement(ElementName = "USL", IsNullable = false)]
        public List<ServiceE> ServiceXml
        {
            get { return _serviceCollection; }
            set { _serviceCollection = value; }
        }

        [XmlElement(ElementName = "COMENTSL", IsNullable = false)]
        public string CommentsXml
        {
            get { return Comments; }
            set
            {
                Comments = value.TrimNullable();
            }
        }

        [XmlIgnore]
        public int? AssistanceType { get; set; }

        [XmlIgnore]
        public string SecondaryMES { get; set; }

        [XmlIgnore]
        public int? PaymentMethod { get; set; }

        [XmlIgnore]
        public int? ProfileCodeId { get; set; }

        [XmlIgnore]
        public int? AssistanceConditions { get; set; }

        [XmlIgnore]
        public string MedicalOrganizationCode { get; set; }

        // int(10)
        [XmlIgnore]
        public decimal? Hospitalization { get; set; }

        // numeric(2)
        [XmlIgnore]
        public bool? IsChildren { get; set; }

        // bit
        [XmlIgnore]
        public string History { get; set; }

        // nvarchar(50)
        [XmlIgnore]
        public DateTime? EventBegin { get; set; }

        // datetime(3)
        [XmlIgnore]
        public DateTime? EventEnd { get; set; }

        // datetime(3)
        [XmlIgnore]
        public int? DiagnosisPrimary { get; set; }

        // int(10)   
        [XmlIgnore]
        public int? DiagnosisGeneral { get; set; }

        // int(10)
        [XmlIgnore]
        public List<string> DiagnosisSecondary { get; set; }

        // int(10)
        [XmlIgnore]
        public decimal? Rate { get; set; }

        // money(19,4)
        [XmlIgnore]
        public decimal? Price { get; set; }

        // money(19,4)
        [XmlIgnore]
        public decimal? Quantity { get; set; }

        // money(19,4)
        [XmlIgnore]
        public string Comments { get; set; }

        // nvarchar(250)
        [XmlIgnore]
        public int? Result { get; set; }

        // int(10)
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

        // int(10)
        [XmlIgnore]
        public int? DoctorId { get; set; }

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
        public int? DiagnosisComplication { get; set; }
        
        [XmlIgnore]
        public List<string> NewbornsWeight { get; set; }

        [XmlIgnore]
        public int? SpecialityCodeExtend { get; set; }

        [XmlIgnore]
        public decimal? RefusalPrice { get; set; }

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

    }
}