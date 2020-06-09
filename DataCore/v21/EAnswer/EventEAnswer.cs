using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;
using Medical.DataCore.v21.E;

namespace Medical.DataCore.v21.EAnswer
{
    public class EventEAnswer : IMEventAnswer
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventEAnswer()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            InnerRefusals = new List<object>();
        }

        [XmlElement(ElementName = "IDCASE")]
        public int ExternalId { get; set; }

        [XmlElement(ElementName = "NHISTORY")]
        public string HistoryXml
        {
            get { return History; }
            set { History = value.TrimNullable(); }
        }
        [XmlElement(ElementName = "IDSP")]
        public int? PaymentMethod { get; set; } 

        [XmlElement(ElementName = "ED_COL", IsNullable = false)]
        public string QuantityXml
        {
            get { return Quantity.ToF2(); }
            set { Quantity = value.ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "TARIF", IsNullable = false)]
        public string RateXml
        {
            get { return Rate.ToF2(); }
            set { Rate = value.ToDecimalNullable(); }
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
            set { PaymentStatus = _cacheRepository.Get("F005Cache").GetBack(value.TrimNullable()); }
        }
        
        [XmlElement(ElementName = "SUMP", IsNullable = false)]
        public string AcceptPriceXml
        {
            get { return AcceptPrice.ToF2(); }
            set { AcceptPrice = value.ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "SANK_IT", IsNullable = false)]
        public string RefusalPriceXml
        {
            get { return RefusalPrice.ToF2(); }
            set { RefusalPrice = value.ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "COMENTSL", IsNullable = false)]
        public string CommentsXml
        {
            get { return Comments; }
            set { Comments = value.TrimNullable(); }
        }

        [XmlElement(ElementName = "SANK", IsNullable = false)]
        public List<RefusalE> RefusalsXml
        {
            get { return _refusals; }
            set { _refusals = value; }
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
        public List<RefusalE> _refusals { get; set; }

        [XmlIgnore]
        public List<object> InnerRefusals
        {
            get { return _refusals.Select(p => p as object).ToList(); }
            set { _refusals = value.Select(p => p as RefusalE).ToList(); }
        }

        [XmlIgnore]
        public decimal? RefusalPrice { get; set; }
    } 
}
