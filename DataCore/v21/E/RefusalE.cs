using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v21.E
{
    public class RefusalE
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

        [XmlElement(ElementName = "S_OSN")]
        public string RefusalCodeXml
        {
            get { return _cacheRepository.Get("F014Cache").GetString(RefusalCode); }
            set { RefusalCode = _cacheRepository.Get("F014Cache").GetBack(value.TrimNullable()); }
        }

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
