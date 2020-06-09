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

namespace Medical.DataCore.v32K1.D
{
    [XmlRoot("ZL_LIST", IsNullable = true, Namespace = "")]
    public class AccountRegisterD: IAccountRegister
    {
        [XmlIgnore]
        public List<IRecordsCollectionD> InnerRecordCollection
        {
            get { return _recordsCollection.Select(p => p as IRecordsCollectionD).ToList(); }
            set { _recordsCollection = value.Select(p => p as RecordsCollection).ToList(); }
        }

        [XmlIgnore]
        public IAccountD InnerAccount { get; set; }

        [XmlIgnore]
        public IHeaderD InnerHeader { get; set; }

        [XmlIgnore]
        public List<RecordsCollection> _recordsCollection { get; set; }

        [XmlElement(ElementName = "ZGLV")]
        public Header Header { get; set; }  
        [XmlElement(ElementName = "SCHET")]
        public Account Account { get; set; }

        [XmlElement(ElementName = "ZAP")]
        public List<RecordsCollection> RecordsCollection
        {
            get { return _recordsCollection; }
            set { _recordsCollection = value; }
        }

        public AccountRegisterD()
        {
            //RecordsCollection = new List<RecordsCollection>();
            _recordsCollection = new List<RecordsCollection>();
        }
    }

    public class Header :IHeaderD
    {
        [XmlElement(ElementName = "VERSION")]
        public string Version { get; set; }
        [XmlElement(ElementName = "DATA")]
        public string DateXml
        {
            get { return Date.HasValue ? Date.Value.ToString("yyyy-MM-dd") : null; }
            set { Date = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
        }
        [XmlElement(ElementName = "FILENAME")]
        public string FileName { get; set; }
        /// <summary>
        /// SD_Z
        /// </summary>
        [XmlElement(ElementName = "SD_Z")]
        public string Sdz { get; set; }

        [XmlIgnore]
        public DateTime? Date { get; set; }
    }

    public class Account :IAccountD
    {
        [XmlIgnore]
        private static readonly ICacheRepository _cacheRepository = Di.I.Resolve<ICacheRepository>();
        public Account()
        {
        }

        [XmlElement(ElementName = "CODE")]
        public int ExternalId { get; set; }

        [XmlElement(ElementName = "CODE_MO")]
        public string MedicalOrganizationCode
        {
            get { return _cacheRepository.Get("F003Cache").GetString(MedicalOrganization); }
            set { MedicalOrganization = _cacheRepository.Get("F003Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "YEAR")]
        public string YearXml
        {
            get
            {
                return Year != null ? Year.Value.ToString("D4", CultureInfo.InvariantCulture) : null;
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
                return Month != null ? Month.Value.ToString("D2", CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Month = SafeConvert.ToInt32(value);
            }
        }
        
        [XmlElement(ElementName = "NSCHET")]
        public string AccountNumber { get; set; }
        [XmlElement(ElementName = "DSCHET")]
        public string AccountDateXml
        {
            get { return AccountDate.HasValue ? AccountDate.Value.ToString("yyyy-MM-dd") : null; }
            set { AccountDate = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
        } 

        [XmlElement(ElementName = "PLAT",IsNullable = false)]
        public string InsurancePayerXml
        {
            get { return _cacheRepository.Get("F002Cache").GetString(InsurancePayer); }
            set { InsurancePayer = _cacheRepository.Get("F002Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SUMMAV")]
        public string PriceXml
        {
            get
            {
                return Price != null ? Price.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
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
                return AcceptPrice != null ? AcceptPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
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
                return MECPenalties != null ? MECPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
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
                return MEEPenalties != null ? MEEPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
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
                return EQMAPenalties != null ? EQMAPenalties.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
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
        public int? MedicalOrganization { get; set; } 
        [XmlIgnore]
        public DateTime? AccountDate { get; set; } 
        [XmlIgnore]
        public int? InsurancePayer { get; set; }
        [XmlIgnore]
        public decimal? Price { get; set; }
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

    public class RecordsCollection : IRecordsCollectionD
    {
        [XmlElement(ElementName = "N_ZAP")]
        public int ExternalId { get; set; }
        [XmlElement(ElementName = "PR_NOV")]
        public string IsSended { get; set; }
        [XmlElement(ElementName = "PACIENT")]
        public PatientD Patient { get; set; }
        [XmlElement(ElementName = "Z_SL")]
        public List<ZslEventD> EventCollection { get; set; }

        public RecordsCollection()
        {
            EventCollection = new List<ZslEventD>();
        }
    }

}
