using System;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v31K1.DV
{
    /// <summary>
    /// SANK
    /// </summary>
    public class RefusalD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public RefusalD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "S_CODE")]
        public string ExternalGuid { get; set; }

        [XmlElement(ElementName = "S_SUM")]
        public string RefusalRateXml
        {
            get { return RefusalRate.ToF2(); }
            set { RefusalRate = value.ToDecimalNullable(); }
        }

        [XmlElement(ElementName = "S_TIP")]
        public string RefusalTypeXml {
            get { return RefusalType.ToStringNullable(); }
            set { RefusalType = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "SL_ID")]
        public string SlidGuid { get; set; }

        [XmlElement(ElementName = "S_OSN")]
        public string RefusalCodeXml
        {
            get { return RefusalCode.ToStringNullable(); }
            set { RefusalCode = value.ToInt32Nullable(); }
            //get { return _cacheRepository.Get("F014aCache").GetString(RefusalCode); }
            //set { RefusalCode = _cacheRepository.Get("F014aCache").GetBack(value); }
        }


        [XmlElement(ElementName = "S_DATE", IsNullable = false)]
        public string DateXml {
            get { return Date.ToFormatString(); }
            set { Date = value.ToDateTimeExact(); }
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
            set { RefusalSource = value.ToInt32Nullable(); }
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

        [XmlIgnore]
        public DateTime? Date { get; set; }
        
    }

   
}
