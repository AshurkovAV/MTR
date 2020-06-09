using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Autofac;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v30.EAnswer
{
    [XmlRoot("ZL_LIST", IsNullable = true, Namespace = "")]
    public class RegisterEAnswer : IZRegisterAnswer
    {
        #region Inner

        [XmlIgnore]
        public List<IZRecordAnswer> InnerRecordCollection
        {
            get { return _recordsCollection.Select(p => p as IZRecordAnswer).ToList(); }
            set { _recordsCollection = value.Select(p => p as RecordsEAnswer).ToList(); }
        }

        [XmlIgnore]
        public IAccountAnswer InnerAccount { get; set; }

        [XmlIgnore]
        public IHeaderAnswer InnerHeader { get; set; }

        [XmlIgnore]
        public List<RecordsEAnswer> _recordsCollection { get; set; }
        #endregion

        [XmlElement(ElementName = "ZGLV")]
        public HeaderEAnswer Header
        {
            get { return InnerHeader as HeaderEAnswer; }
            set { InnerHeader = value; }
        }
        [XmlElement(ElementName = "SCHET")]
        public AccountEAnswer Account
        {
            get { return InnerAccount as AccountEAnswer; }
            set { InnerAccount = value; }
        }

        [XmlElement(ElementName = "ZAP")]
        public List<RecordsEAnswer> RecordsCollection
        {
            get { return _recordsCollection; }
            set { _recordsCollection = value; }
        }

        public RegisterEAnswer()
        {
            _recordsCollection = new List<RecordsEAnswer>();
            Header = new HeaderEAnswer();
        }
    }


    public class HeaderEAnswer : IHeaderAnswer
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public HeaderEAnswer()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "VERSION")]
        public string VersionXml
        {
            get { return _cacheRepository.Get("VersionCache").GetString(Version); }
            set { Version = _cacheRepository.Get("VersionCache").GetBack(value); }
        }

        [XmlElement(ElementName = "DATA")]
        public string DateXml
        {
            get { return Date.HasValue ? Date.Value.ToString("yyyy-MM-dd") : null; }
            set { Date = SafeConvert.ToDateTimeExact(value,"yyyy-MM-dd"); }
        }

        [XmlElement(ElementName = "OKATO_OMS")]
        public string TargetOkato { get; set; }

        [XmlIgnore]
        public DateTime? Date { get; set; }

        [XmlIgnore]
        public int? Version { get; set; }
    }

    public class AccountEAnswer : IAccountAnswer
    {
        [XmlElement(ElementName = "YEAR")]
        public string YearXml
        {
            get
            {
                return Year.HasValue ? Year.Value.ToString("D4", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Year = SafeConvert.ToInt32(value);
            }
        }

        [XmlElement(ElementName = "MONTH")]
        public string MonthXml
        {
            get
            {
                return Month.HasValue ? Month.Value.ToString("D2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Month = SafeConvert.ToInt32(value.Trim());
            }
        }

        [XmlElement(ElementName = "NSCHET")]
        public string AccountNumber { get; set; }
        [XmlElement(ElementName = "DSCHET")]
        public string AccountDateXml
        {
            get
            {
                return AccountDate != null ? AccountDate.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                AccountDate = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "SUMMAV")]
        public string PriceXml
        {
            get
            {
                return Price.HasValue ? Price.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Price = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SUMMAP", IsNullable = false)]
        public string AcceptPriceXml
        {
            get
            {
                return AcceptPrice.HasValue ? AcceptPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { AcceptPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SANK_MEK", IsNullable = false)]
        public string MECPenaltiesXml { 
            get
            {
                return MECPenalties.HasValue ? MECPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { MECPenalties = SafeConvert.ToDecimal(value); }

        }
        [XmlElement(ElementName = "SANK_MEE", IsNullable = false)]
        public string MEEPenaltiesXml { get
            {
                return MEEPenalties.HasValue ? MEEPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { MEEPenalties = SafeConvert.ToDecimal(value); }
        }
        [XmlElement(ElementName = "SANK_EKMP", IsNullable = false)]
        public string EQMAPenaltiesXml
        {
            get
            {
                return EQMAPenalties.HasValue ? EQMAPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { EQMAPenalties = SafeConvert.ToDecimal(value); }
        }

        [XmlIgnore]
        public int? Year { get; set; }
        [XmlIgnore]
        public int? Month { get; set; }
        [XmlIgnore]
        public decimal? Price { get; set; }
        [XmlIgnore]
        public DateTime? AccountDate { get; set; }
        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }
        [XmlIgnore]
        public decimal? MECPenalties { get; set; }
        [XmlIgnore]
        public decimal? MEEPenalties { get; set; }
        [XmlIgnore]
        public decimal? EQMAPenalties { get; set; } 
    }

    public class RecordsEAnswer : IZRecordAnswer
    {
        #region Inner

        [XmlIgnore]
        public List<IZslMeventAnswer> InnerEventCollection
        {
            get { return _eventCollection.Select(p => p as IZslMeventAnswer).ToList(); }
            set { _eventCollection = value.Select(p => p as ZslEventEAnswer).ToList(); }
        }

        [XmlIgnore]
        public IPatientAnswer InnerPatient { get; set; }

        [XmlIgnore]
        public List<ZslEventEAnswer> _eventCollection { get; set; }
        #endregion

        [XmlElement(ElementName = "N_ZAP")]
        public int ExternalId { get; set; }
        [XmlElement(ElementName = "PACIENT")]
        public PatientEAnswer Patient
        {
            get { return InnerPatient as PatientEAnswer; }
            set { InnerPatient = value; }
        }

        [XmlElement(ElementName = "Z_SL")]
        public List<ZslEventEAnswer> EventCollection
        {
            get { return _eventCollection; }
            set { _eventCollection = value; }
        }

        public RecordsEAnswer()
        {
            _eventCollection = new List<ZslEventEAnswer>();
        }
    }



}
