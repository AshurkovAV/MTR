using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;
using Medical.DataCore.v32.E;

namespace Medical.DataCore.v32.EAnswer
{
    public class ZslEventEAnswer : IZslMeventAnswer
    {
        [XmlIgnore]
        public List<RefusalE> _refusals { get; set; }
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public ZslEventEAnswer()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            InnerEventCollection = new List<IZMeventAnswer>();
            InnerRefusals = new List<IRefusal>();
        }

        [XmlElement(ElementName = "IDCASE")]
        public int ExternalId { get; set; }

        [XmlElement(ElementName = "IDSP")]
        public int? PaymentMethod { get; set; }

        //[XmlElement(ElementName = "ED_COL", IsNullable = false)]
        //public string QuantityXml
        //{
        //    get { return Quantity.ToF2(); }
        //    set { Quantity = value.ToDecimalNullable(); }
        //}

        //[XmlElement(ElementName = "TARIF", IsNullable = false)]
        //public string RateXml
        //{
        //    get { return Rate.ToF2(); }
        //    set { Rate = value.ToDecimalNullable(); }
        //}

        [XmlElement(ElementName = "SL", IsNullable = false)]
        public List<EventEAnswer> EventXml
        {
            get { return _answer; }
            set { _answer = value; }
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
        [XmlElement(ElementName = "SANK", IsNullable = false)]
        public List<RefusalE> RefusalsXml
        {
            get { return _refusals; }
            set { _refusals = value; }
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
        [XmlIgnore]
        public List<IRefusal> InnerRefusals
        {
            get { return _refusals.Select(p => p as IRefusal).ToList(); }
            set { _refusals = value.Select(p => p as RefusalE).ToList(); }
        }
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
        public List<EventEAnswer> _answer { get; set; }

        [XmlIgnore]
        public decimal? RefusalPrice { get; set; }

        [XmlIgnore]
        public List<IZMeventAnswer> InnerEventCollection
        {
            get { return _answer.Select(p => p as IZMeventAnswer).ToList(); }
            set { _answer = value.Select(p => p as EventEAnswer).ToList(); }
        }
    } 
}
