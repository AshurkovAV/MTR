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

namespace Medical.DataCore.v10.EAnswer
{
    public class EventEAnswer : IMEventAnswer
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventEAnswer()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "IDCASE")]
        public int ExternalId { get; set; }
        [XmlElement(ElementName = "NHISTORY")]
        public string HistoryXml
        {
            get
            {
                return History;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    History = value.Trim();
            }
        }
        [XmlElement(ElementName = "IDSP")]
        public int? PaymentMethod { get; set; } 

        [XmlElement(ElementName = "ED_COL", IsNullable = false)]
        public string QuantityXml
        {
            get
            {
                return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Quantity = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "TARIF", IsNullable = false)]
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
        public string PaymentStatusXml
        {
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

        [XmlElement(ElementName = "REFREASON")]
        public List<int> RefusalsXml
        {
            get
            {
                return InnerRefusals
                    .Select(p => p.ToInt32Nullable())
                    .Where(p => p.HasValue)
                    .Select(p => p.Value)
                    .ToList();
            }
            set
            {
                InnerRefusals = value.Select(p => p as object).ToList();
            }
        }

        [XmlElement(ElementName = "SANK_MEK", IsNullable = false)]
        public string MecXml
        {
            get
            {
                return Mec.HasValue ? Mec.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Mec = SafeConvert.ToDecimal(value); }
        }
        [XmlElement(ElementName = "SANK_MEE", IsNullable = false)]
        public string MeeXml
        {
            get
            {
                return Mee.HasValue ? Mee.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Mee = SafeConvert.ToDecimal(value); }
        }
        [XmlElement(ElementName = "SANK_EKMP", IsNullable = false)]
        public string EqmaXml
        {
            get
            {
                return Eqma.HasValue ? Eqma.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Eqma = SafeConvert.ToDecimal(value); }
        }

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
                    Comments = value;
            }
        }

        
        [XmlIgnore]
        public string History { get; set; } 
        [XmlIgnore]
        public decimal? Rate { get; set; } 
        [XmlIgnore]
        public decimal? Price { get; set; } 
        [XmlIgnore]
        public decimal? Quantity { get; set; } 
        [XmlIgnore]
        public string Comments { get; set; } 
        [XmlIgnore]
        public int? PaymentStatus { get; set; }
        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }
        [XmlIgnore]
        public decimal? Mec { get; set; }
        [XmlIgnore]
        public decimal? Mee { get; set; }
        [XmlIgnore]
        public decimal? Eqma { get; set; }

        [XmlIgnore]
        public List<object> InnerRefusals { get; set; }

        [XmlIgnore]
        public decimal? RefusalPrice { get; set; }
    } 
}
