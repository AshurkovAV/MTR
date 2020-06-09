using System;
using System.Xml.Serialization;
using Core.Extensions;
using Core.Helpers;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v31.E
{
    public class DirectionOnkE: IDirectionOnkE
    {
        public DirectionOnkE()
        {
        }

        [XmlElement(ElementName = "NAPR_DATE")]
        public string DirectionDateXml
        {
            get { return DirectionDate.HasValue ? DirectionDate.Value.ToString("yyyy-MM-dd") : null; }
            set
            {
                DirectionDate = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }
        [XmlElement(ElementName = "NAPR_MO", IsNullable = false)]
        public string DirectionMo { get; set; }
        [XmlElement(ElementName = "NAPR_V")]
        public string DirectionViewXml
        {
            get { return DirectionViewId.ToStringNullable(); }
            set { DirectionViewId = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "MET_ISSL", IsNullable = false)]
        public string MetIsslXml
        {
            get { return MetIsslId.ToStringNullable(); }
            set { MetIsslId = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "NAPR_USL", IsNullable = false)]
        public string DirectionService { get; set; }

     
        [XmlIgnore]
        public DateTime? DirectionDate { get; set; }
        [XmlIgnore]
        public int? DirectionViewId { get; set; }
        [XmlIgnore]
        public int? MetIsslId { get; set; }

       }
}
