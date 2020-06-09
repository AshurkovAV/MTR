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
using Medical.DataCore.v30.E;

namespace Medical.DataCore.v30.EAnswer
{
    public class EventEAnswer: IZMeventAnswer
    {
        [XmlIgnore]
        public List<RefusalE> _refusals { get; set; }

        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventEAnswer()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            InnerRefusals = new List<IRefusal>();
        }

        [XmlElement(ElementName = "SL_ID")]
        public string ExternalId { get; set; }

        [XmlElement(ElementName = "NHISTORY")]
        public string HistoryXml
        {
            get { return History; }
            set { History = value.TrimNullable(); }
        }
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

        [XmlElement(ElementName = "SANK", IsNullable = false)]
        public List<RefusalE> RefusalsXml
        {
            get { return _refusals; }
            set { _refusals = value; }
        }

        [XmlIgnore]
        public string History { get; set; }

        [XmlIgnore]
        public List<IRefusal> InnerRefusals
        {
            get { return _refusals.Select(p => p as IRefusal).ToList(); }
            set { _refusals = value.Select(p => p as RefusalE).ToList(); }
        }
        //[XmlIgnore]
        //public decimal? Rate { get; set; }
        //[XmlIgnore]
        //public decimal? Quantity { get; set; }

    }
}
