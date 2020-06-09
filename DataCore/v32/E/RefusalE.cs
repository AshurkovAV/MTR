using System;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v32.E
{
    public class RefusalE:IRefusal
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public RefusalE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "S_CODE")]
        public string ExternalGuid { get; set; }

        [XmlElement(ElementName = "S_SUM")]
        public string RefusalRateXml
        {
            get { return RefusalRate.ToF2(); }
            set { RefusalRate = value.TrimNullable().ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "S_TIP")]
        public string RefusalTypeXml {
            get { return RefusalType.ToStringNullable(); }
            set { RefusalType = value.TrimNullable().ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "SL_ID")]
        public string SlidGuid { get; set; }
        [XmlElement(ElementName = "S_OSN")]
        public string RefusalCodeXml
        {
            get { return _cacheRepository.Get("F014Cache").GetString(RefusalCode); }
            set { RefusalCode = _cacheRepository.Get("F014Cache").GetBack(value.TrimNullable()); }
        }
        [XmlElement(ElementName = "DATE_ACT", IsNullable = false)]
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

        [XmlElement(ElementName = "NUM_ACT", IsNullable = false)]
        public string NumAct { get; set; }

        [XmlElement(ElementName = "CODE_EXP", IsNullable = false)]
        public string CodeExp { get; set; }
        [XmlElement(ElementName = "S_COM", IsNullable = false)]
        public string CommentsXml
        {
            get { return Comments; }
            set { Comments = value.TrimNullable(); }
        }

        [XmlElement(ElementName = "S_IST")]
        public string RefusalSourceXml
        {
            get { return RefusalSource.ToStringNullable(); }
            set { RefusalSource = value.TrimNullable().ToInt32Nullable(); }
        }
        [XmlIgnore]
        public DateTime? Date { get; set; }
        [XmlIgnore]
        public decimal? RefusalRate { get; set; }
        [XmlIgnore]
        public int? RefusalCode { get; set; }
        [XmlIgnore]
        public int? RefusalType { get; set; }
        [XmlIgnore]
        public int? RefusalSource { get; set; }
        
        [XmlIgnore]
        public string Comments { get; set; } 
    }

   
}
