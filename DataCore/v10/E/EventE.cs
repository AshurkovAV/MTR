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

namespace Medical.DataCore.v10.E
{
    public class EventE : IMEvent
    {
        #region Inner
        [XmlIgnore]
        public List<IService> InnerServiceCollection 
        {
            get { return _serviceCollection.Select(p => p as IService).ToList(); }
            set { _serviceCollection = value.Select(p => p as ServiceE).ToList(); }
        }

        [XmlIgnore]
        public List<ServiceE> _serviceCollection { get; set; }
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
            set { AssistanceConditions = SafeConvert.ToInt32(value, false); }
        }

        [XmlElement(ElementName = "VIDPOM")]
        public string AssistanceTypeXml
        {
            get { return AssistanceType.HasValue ? AssistanceType.ToString() : null; }
            set { AssistanceType = SafeConvert.ToInt32(value, false); }
        }

        [XmlElement(ElementName = "EXTR", IsNullable = false)]
        public string HospitalizationNullable
        {
            get { return Hospitalization.HasValue ? Hospitalization.ToString() : null; }
            set { Hospitalization = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationCodeXml
        {
            get { return MedicalOrganizationCode; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    MedicalOrganizationCode = value.Trim();
            }
        }

        [XmlElement(ElementName = "PROFIL")]
        public string ProfileCodeIdXml
        {
            get { return ProfileCodeId.HasValue ? ProfileCodeId.ToString() : null; }
            set { ProfileCodeId = SafeConvert.ToInt32(value, false); }
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
                if (!string.IsNullOrWhiteSpace(value))
                    History = value.Trim();
            }
        }

        [XmlElement(ElementName = "DATE_1")]
        public string EventBeginXml
        {
            get { return EventBegin.HasValue ? EventBegin.Value.ToString("yyyy-MM-dd") : null; }
            set { EventBegin = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
        }

        [XmlElement(ElementName = "DATE_2")]
        public string EventEndXml
        {
            get { return EventEnd.HasValue ? EventEnd.Value.ToString("yyyy-MM-dd") : null; }
            set { EventEnd = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
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
        public string DiagnosisSecondaryXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(DiagnosisSecondary); }
            set { DiagnosisSecondary = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }


        [XmlElement(ElementName = "CODE_MES1", IsNullable = false)]
        public string MESIdXml
        {
            get { return MES; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    MES = value.Trim();
            }
        }

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
        public string SpecialityCodeXml
        {
            get { return _cacheRepository.Get("V004Cache").GetString(SpecialityCode); }
            set { SpecialityCode = _cacheRepository.Get("V004Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "IDSP")]
        public string PaymentMethodXml
        {
            get { return PaymentMethod.HasValue ? PaymentMethod.ToString() : null; }
            set { PaymentMethod = SafeConvert.ToInt32(value, false); }
        }

        [XmlElement(ElementName = "ED_COL", IsNullable = false)]
        public string QuantityXml
        {
            get { return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { Quantity = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "TARIF", IsNullable = false)]
        public string RateXml
        {
            get { return Rate.HasValue ? Rate.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { Rate = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SUMV")]
        public string PriceXml
        {
            get { return Price.HasValue ? Price.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { Price = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "OPLATA", IsNullable = false)]
        public string PaymentStatusXml
        {
            get { return _cacheRepository.Get("F005Cache").GetString(PaymentStatus); }
            set { PaymentStatus = _cacheRepository.Get("F005Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SUMP", IsNullable = false)]
        public string AcceptPriceXml
        {
            get { return AcceptPrice.HasValue ? AcceptPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { AcceptPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "REFREASON")]
        public List<int> RefusalsXml
        {
            get
            {
                return InnerRefusals
                    .Select(p => p.ToInt32Nullable())
                    .Where(p=>p.HasValue)
                    .Select(p=>p.Value)
                    .ToList();
            }
            set
            {
                InnerRefusals = value.Select(p=>p as object).ToList();
            }
        }


        [XmlElement(ElementName = "SANK_MEK", IsNullable = false)]
        public string MecXml
        {
            get { return MEC.HasValue ? MEC.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { MEC = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SANK_MEE", IsNullable = false)]
        public string MeeXml
        {
            get { return MEE.HasValue ? MEE.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { MEE = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SANK_EKMP", IsNullable = false)]
        public string EqmaXml
        {
            get { return EQMA.HasValue ? EQMA.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { EQMA = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "USL", IsNullable = false)]
        public List<ServiceE> ServiceCollection
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
                if (!string.IsNullOrWhiteSpace(value))
                    Comments = value.Trim();
            }
        }

        [XmlIgnore]
        public int? AssistanceType { get; set; }

        [XmlIgnore]
        public string SecondaryMES { get; set; }

        [XmlIgnore]
        public string MES { get; set; }

        [XmlIgnore]
        public int? PaymentMethod { get; set; }

        [XmlIgnore]
        public int? ProfileCodeId { get; set; }

        [XmlIgnore]
        public int? AssistanceConditions { get; set; }

        [XmlIgnore]
        public List<object> InnerRefusals { get; set; }

        [XmlIgnore]
        public string MedicalOrganizationCode { get; set; }

        [XmlIgnore]
        public int? ReferralOrganization { get; set; }

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
        public int? DiagnosisSecondary { get; set; }

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

        // int(10)
        [XmlIgnore]
        public int? SpecialityCode { get; set; }

        

        // int(10)
        [XmlIgnore]
        public int? DoctorId { get; set; }

        [XmlIgnore]
        public int? PaymentStatus { get; set; }

        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }

        [XmlIgnore]
        public decimal? MEC { get; set; }

        [XmlIgnore]
        public decimal? MEE { get; set; }

        [XmlIgnore]
        public decimal? EQMA { get; set; }


        #region v21 interface implementation
        [XmlIgnore]
        public string SpecialityClassifierVersion { get; set; }
        [XmlIgnore]
        public int? SpecialityCodeV015 { get; set; }
         [XmlIgnore]
        public decimal? RefusalPrice { get; set; }

        #endregion
    }
}