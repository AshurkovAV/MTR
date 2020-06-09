using System;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v31K1.DV
{
    /// <summary>
    /// CONS
    /// </summary>
    public class ConsultationsOnkD
    {
        public ConsultationsOnkD()
        {
        }

        [XmlElement(ElementName = "PR_CONS")]
        public string PrConsXml
        {
            get { return PrCons.ToStringNullable(); }
            set { PrCons = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "DT_CONS", IsNullable = false)]
        public string DtConsXml
        {
            get { return DtCons.ToFormatString(); }
            set { DtCons = value.ToDateTimeExact(); }
        }

        [XmlIgnore]
        public int? PrCons { get; set; }

        [XmlIgnore]
        public DateTime? DtCons { get; set; }

    }
}
