using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v31K1.DV
{
    public class ZslEventD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public ZslEventD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            Event = new List<EventD>();
        }

        [XmlElement(ElementName = "IDCASE")]
        public int MedicalExternalId { get; set; }

        [XmlElement(ElementName = "USL_OK", IsNullable = false)]
        public string AssistanceConditionsXml
        {
            get
            {
                return AssistanceConditions.HasValue ? AssistanceConditions.ToString() : null;
            }
            set { AssistanceConditions = SafeConvert.ToInt32(value); }
        }
        [XmlElement(ElementName = "ZSL_ID")]
        public string ZslIdGuid { get; set; }
        [XmlElement(ElementName = "VIDPOM")]
        public int? AssistanceType { get; set; }

        [XmlElement(ElementName = "FOR_POM")]
        public string AssistanceFormXml
        {
            get { return AssistanceForm.HasValue ? AssistanceType.ToString() : null; }
            set { AssistanceForm = SafeConvert.ToInt32(value, false); }
        }

        [XmlElement(ElementName = "NPR_MO", IsNullable = false)]
        public string ReferralOrganizationNullable
        {
            get { return _cacheRepository.Get("F003Cache").GetString(ReferralOrganization); }
            set { ReferralOrganization = _cacheRepository.Get("F003Cache").GetBack(value); }
        }
        [XmlElement(ElementName = "NPR_DATE")]
        public string ReferralDateXml
        {
            get
            {
                return ReferralDate.HasValue ? ReferralDate.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                ReferralDate = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }
        [XmlElement(ElementName = "LPU")]
        public string MedicalOrganizationCode { get; set; }
        /// <summary>
        /// VBR Признак мобильной медицинской бригады
        /// </summary>
        [XmlElement(ElementName = "VBR", IsNullable = false)]
        public int Vbr { get; set; }
       
        [XmlElement(ElementName = "DATE_Z_1")]
        public string EventBeginXml
        {
            get
            {
                return EventBeginZ1.HasValue ? EventBeginZ1.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                EventBeginZ1 = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        } 
        [XmlElement(ElementName = "DATE_Z_2")]
        public string EventEndXml
        {
            get
            {
                return EventEndZ2.HasValue ? EventEndZ2.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                EventEndZ2 = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }
        //TODO P_OTK нужно подумать включать данное поле в обработку на региональном уровне
        [XmlElement(ElementName = "KD_Z", IsNullable = false)]
        public string QuantityXml
        {
            get
            {
                return Kdz.HasValue ? Kdz.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Kdz = SafeConvert.ToDecimal(value); }
        }
        /// <summary>
        /// VNOV_M Вес при рождении
        /// </summary>
        [XmlElement(ElementName = "VNOV_M", IsNullable = false)]
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
        /// <summary>
        /// RSLT
        /// </summary>
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
        /// <summary>
        /// OS_SLUCH
        /// </summary>
        [XmlElement(ElementName = "OS_SLUCH", IsNullable = false)]
        public List<string> ParticularCaseCollection { get; set; }
        /// <summary>
        /// OS_SLUCH_REGION
        /// </summary>
        [XmlElement(ElementName = "OS_SLUCH_REGION", IsNullable = false)]
        public string RegionalAttributeXml
        {
            get { return RegionalAttribute.HasValue ? RegionalAttribute.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { RegionalAttribute = SafeConvert.ToInt32(value); }
        }

        [XmlIgnore]
        public int? RegionalAttribute { get; set; }

        [XmlElement(ElementName = "SL")]
        public List<EventD> Event { get; set; }

        [XmlElement(ElementName = "IDSP")]
        public int? PaymentMethod { get; set; }

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
        [XmlElement(ElementName = "SANK")]
        public List<RefusalD> Refusals { get; set; }

        [XmlElement(ElementName = "SANK_IT", IsNullable = false)]
        public string RefusalPriceXml
        {
            get { return RefusalPrice.HasValue ? RefusalPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null; }
            set { RefusalPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlIgnore]
        public int? AssistanceConditions { get; set; }
        [XmlIgnore]
        public int? ReferralOrganization { get; set; }
        [XmlIgnore]
        public DateTime? ReferralDate { get; set; }
        [XmlIgnore]
        public DateTime? EventBeginZ1 { get; set; } 
        [XmlIgnore]
        public DateTime? EventEndZ2 { get; set; }
        /// <summary>
        /// Сумма выставленная
        /// </summary>
        [XmlIgnore]
        public decimal? Price { get; set; } 
        [XmlIgnore]
        public decimal? Kdz { get; set; } 
       
        public int? Result { get; set; } 
        [XmlIgnore]
        public int? Outcome { get; set; }
  
        [XmlIgnore]
        public int? PaymentStatus { get; set; }
        /// <summary>
        /// Сумма принятая
        /// </summary>
        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }

        [XmlIgnore]
        public int? AssistanceForm { get; set; }

        [XmlIgnore]
        public List<string> NewbornsWeight { get; set; }

        [XmlIgnore]
        public int? EventType { get; set; }
        
        [XmlIgnore]
        public decimal? RefusalPrice { get; set; }
        [XmlIgnore]
        public string NewbornsWeightAggregate
        {
            get { return NewbornsWeight.AggregateToString(); }
            set { NewbornsWeight = value.ToListByDelimiter(); }
        }

    }

    public class ParticularCase
    {
        [XmlElement(ElementName = "OS_SLUCH")]
        public string ParticularCaseId { get; set; } // nvarchar(250)
    }
}
