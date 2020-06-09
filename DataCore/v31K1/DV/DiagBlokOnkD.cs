using System;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v31K1.DV
{
    /// <summary>
    /// B_DIAG
    /// </summary>
    public class DiagBlokOnkD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public DiagBlokOnkD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }
        [XmlElement(ElementName = "DIAG_DATE")]
        public string DiagDateXml
        {
            get
            {
                return DiagDate.HasValue ? DiagDate.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                DiagDate = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "DIAG_TIP")]
        public string DiagTipXml
        {
            get { return DiagTip.ToStringNullable(); }
            set { DiagTip = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "DIAG_CODE")]
        public string DiagCodeXml
        {
            get
            {
                if (DiagTipXml == "1")
                {
                    return DiagCode7.ToStringNullable();
                }
                if (DiagTipXml == "2")
                {
                    return DiagCode10.ToStringNullable();
                }
                return null;
            }
            set
            {
                if (DiagTipXml == "1")
                {
                    DiagCode7 = value.ToInt32Nullable();
                }
                if (DiagTipXml == "2")
                {
                    DiagCode10 = value.ToInt32Nullable();
                }
            }
        }

        [XmlElement(ElementName = "DIAG_RSLT", IsNullable = false)]
        public string DiagRsltXml
        {
            get
            {
                if (DiagTipXml == "1")
                {
                    return DiagRslt8.ToStringNullable();
                }
                if (DiagTipXml == "2")
                {
                    return DiagRslt11.ToStringNullable();
                }
                return null;
            }
            set
            {
                if (DiagTipXml == "1")
                {
                    DiagRslt8 = value.ToInt32Nullable();
                }
                if (DiagTipXml == "2")
                {
                    DiagRslt11 = value.ToInt32Nullable();
                }
            }
        }

        [XmlElement(ElementName = "REC_RSLT", IsNullable = false)]
        public string RecRsltXml
        {
            get { return RecRslt.ToStringNullable(); }
            set { RecRslt = value.ToInt32Nullable(); }
        }

        [XmlIgnore]
        public int? RecRslt { get; set; }
        [XmlIgnore]
        public int? DiagTip { get; set; }

        [XmlIgnore]
        public int? DiagCode7 { get; set; }

        [XmlIgnore]
        public int? DiagCode10 { get; set; }

        [XmlIgnore]
        public int? DiagRslt8 { get; set; }

        [XmlIgnore]
        public int? DiagRslt11 { get; set; }

        [XmlIgnore]
        public DateTime? DiagDate { get; set; }

    }
}
