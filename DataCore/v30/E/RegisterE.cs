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

namespace Medical.DataCore.v30.E
{
    [XmlRoot("ZL_LIST", IsNullable = true, Namespace = "")]
    public class RegisterE : IZRegister
    {
        #region Inner

        [XmlIgnore]
        public List<IZRecord> InnerRecordCollection { 
            get { return _recordsCollection.Select(p=>p as IZRecord).ToList(); }
            set { _recordsCollection = value.Select(p=>p as RecordsE).ToList(); }
        }

        [XmlIgnore]
        public IAccount InnerAccount { get; set; }

        [XmlIgnore]
        public IHeader InnerHeader { get; set; }

        [XmlIgnore]
        public List<RecordsE> _recordsCollection { get; set; }
        #endregion

        [XmlElement(ElementName = "ZGLV")]
        public HeaderE Header
        {
            get { return InnerHeader as HeaderE; }
            set { InnerHeader = value; }
        }
        [XmlElement(ElementName = "SCHET")]
        public AccountE Account
        {
            get { return InnerAccount as AccountE; }
            set { InnerAccount = value; }
        }

        [XmlElement(ElementName = "ZAP")]
        public List<RecordsE> RecordsCollection
        {
            get { return _recordsCollection; }
            set { _recordsCollection = value; }
        }

        public RegisterE()
        {
            _recordsCollection = new List<RecordsE>();
            Header = new HeaderE();
        }
    }

    public class HeaderE : IHeader
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public HeaderE()
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
            get
            {
                return Date.ToFormatString();
            }
            set
            {
                Date = value.ToDateTimeExact();
            }
        }
        /// <summary>
        /// Код ОКАТО территории, выставившей счет
        /// </summary>
        [XmlElement(ElementName = "C_OKATO1")]
        public string SourceOkatoXml
        {
            get
            {
                return SourceOkato;
            }
            set
            {
                SourceOkato = value.TrimNullable();
            }
        }
        /// <summary>
        /// Код ОКАТО территории страхования по ОМС (территория, в которую выставляется счет)
        /// </summary>
        [XmlElement(ElementName = "OKATO_OMS")]
        public string TargetOkatoXml
        {
            get
            {                
                return TargetOkato;
            }
            set
            {
                TargetOkato = value.TrimNullable();
            }
        }

        [XmlIgnore]
        public int? Version { get; set; }
        [XmlIgnore]
        public DateTime? Date { get; set; }
        [XmlIgnore]
        public string SourceOkato { get; set; }
        [XmlIgnore]
        public string TargetOkato { get; set; }
    }

    public class AccountE : IAccount
    {
        [XmlElement(ElementName = "CODE")]
        public string ExternalIdXml
        {
            get
            {
                return ExternalId.HasValue ? ExternalId.ToString() : null;
            }
            set
            {
                ExternalId = SafeConvert.ToInt32(value);
            }
        }

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
        public string AccountNumberXml
        {
            get
            {
                return AccountNumber;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    AccountNumber = value.Trim();
            }
        }
        [XmlElement(ElementName = "DSCHET")]
        public string AccountDateXml
        {
            get
            {
                return AccountDate.HasValue ? AccountDate.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                
                AccountDate = SafeConvert.ToDateTimeExact(value,"yyyy-MM-dd");
            }
        } 

        [XmlElement(ElementName = "SUMMAV")]
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

        [XmlElement(ElementName = "COMENTS", IsNullable = false)]
        public string CommentsXml 
        { 
            get { return Comments; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comments = value;
            }
        }

        [XmlElement(ElementName = "SUMMAP", IsNullable = false)]
        public string AcceptPriceXml
        {
            get
            {
                return AcceptPrice.HasValue ? AcceptPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                AcceptPrice = SafeConvert.ToDecimal(value);
            }
        }

        [XmlElement(ElementName = "SANK_MEK", IsNullable = false)]
        public string MECPenaltiesXml
        {
            get
            {
                return MECPenalties.HasValue ? MECPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                MECPenalties = SafeConvert.ToDecimal(value);
            }

        }
        [XmlElement(ElementName = "SANK_MEE", IsNullable = false)]
        public string MEEPenaltiesXml
        {
            get
            {
                return MEEPenalties.HasValue ? MEEPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                MEEPenalties = SafeConvert.ToDecimal(value);
            }
        }
        [XmlElement(ElementName = "SANK_EKMP", IsNullable = false)]
        public string EQMAPenaltiesXml
        {
            get
            {
                return EQMAPenalties.HasValue ? EQMAPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                EQMAPenalties = SafeConvert.ToDecimal(value);
            }
        }

        [XmlIgnore]
        public int? Year { get; set; }
        [XmlIgnore]
        public int? Month { get; set; }
        [XmlIgnore]
        public int? ExternalId { get; set; }
        [XmlIgnore]
        public string AccountNumber { get; set; }
        [XmlIgnore]
        public decimal? Price { get; set; } 
        [XmlIgnore]
        public DateTime? AccountDate { get; set; } 
        [XmlIgnore]
        public string Comments { get; set; }
        [XmlIgnore]
        public decimal? AcceptPrice { get; set; } 
        [XmlIgnore]
        public decimal? MECPenalties { get; set; } 
        [XmlIgnore]
        public decimal? MEEPenalties { get; set; } 
        [XmlIgnore]
        public decimal? EQMAPenalties { get; set; } 
    }

    public class RecordsE : IZRecord
    {
        #region Inner

        [XmlIgnore]
        public List<IZslEvent> InnerZslEventCollection
        {
            get { return _eventCollection.Select(p => p as IZslEvent).ToList(); }
            set { _eventCollection = value.Select(p => p as ZslEventE).ToList(); }
        }

        [XmlIgnore]
        public IPatient InnerPatient { get; set; }

        [XmlIgnore]
        public List<ZslEventE> _eventCollection { get; set; }
        #endregion


        [XmlElement(ElementName = "N_ZAP")]
        public int ExternalId { get; set; }
        [XmlElement(ElementName = "PACIENT")]
        public PatientE Patient
        {
            get { return InnerPatient as PatientE; }
            set { InnerPatient = value; }
        }
        [XmlElement(ElementName = "Z_SL")]
        public List<ZslEventE> EventCollection
        {
            get { return _eventCollection; }
            set { _eventCollection = value; }
        }

        public RecordsE()
        {
            _eventCollection = new List<ZslEventE>();
        }

    }

}
