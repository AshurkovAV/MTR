using System;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;
using Medical.DataCore.v10.E;

namespace Medical.DataCore.partial
{
    [XmlRoot("ZL_LIST", IsNullable = true, Namespace = "")]
    public class Register
    {
        #region Inner

        [XmlIgnore]
        public Account InnerAccount { get; set; }

        [XmlIgnore]
        public Header InnerHeader { get; set; }

        #endregion

        [XmlElement(ElementName = "ZGLV")]
        public Header Header
        {
            get { return InnerHeader; }
            set { InnerHeader = value; }
        }
        [XmlElement(ElementName = "SCHET")]
        public Account Account
        {
            get { return InnerAccount; }
            set { InnerAccount = value; }
        }


        public Register()
        {
            Header = new Header();
        }
    }

    public class Header
    {
        [XmlIgnore]
        private readonly ICacheRepository _cacheRepository;

        public Header()
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
                return Date.HasValue ? Date.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                Date = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }
        [XmlElement(ElementName = "C_OKATO1")]
        public string SourceOkatoXml
        {
            get
            {
                return SourceOkato;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    SourceOkato = value.Trim();
            }
        }

        [XmlElement(ElementName = "OKATO_OMS")]
        public string TargetOkatoXml
        {
            get
            {
                return TargetOkato;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    TargetOkato = value.Trim();
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

    public class Account
    {
        [XmlElement(ElementName = "CODE")]
        public string ExternalIdXml
        {
            get { return ExternalId.HasValue ? ExternalId.ToString() : null; }
            set { ExternalId = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "YEAR")]
        public string YearXml
        {
            get { return Year.HasValue ? Year.Value.ToString("D4", CultureInfo.InvariantCulture) : null; }
            set { Year = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "MONTH")]
        public string MonthXml
        {
            get { return Month.HasValue ? Month.Value.ToString("D2", CultureInfo.InvariantCulture) : null; }
            set { Month = SafeConvert.ToInt32(value.Trim()); }
        }

        [XmlElement(ElementName = "NSCHET")]
        public string AccountNumberXml
        {
            get { return AccountNumber; }
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
    }
}
