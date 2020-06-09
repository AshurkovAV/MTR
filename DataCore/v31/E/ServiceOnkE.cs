using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Core.Extensions;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v31.E
{
    public class ServiceOnkE: IServiceOnk
    {
        [XmlIgnore]
        public List<IAnticancerDrugOnk> InnerAnticancerDrugOnkCollection
        {
            get { return _anticancerDrugOnkCollection.Select(p => p as IAnticancerDrugOnk).ToList(); }
            set
            {
                _anticancerDrugOnkCollection = value.Select(p => p as AnticancerDrugOnkE).ToList();
            }
        }
        [XmlIgnore]
        public List<AnticancerDrugOnkE> _anticancerDrugOnkCollection { get; set; }
        public ServiceOnkE()
        {
            _anticancerDrugOnkCollection = new List<AnticancerDrugOnkE>();
        }
        /// <summary>
        /// Тип услуги
        /// </summary>
        [XmlElement(ElementName = "USL_TIP")]
        public string ServicesOnkTypeXml
        {
            get { return ServicesOnkTypeId.ToStringNullable(); }
            set { ServicesOnkTypeId = value.ToInt32Nullable(); }
        }
        /// <summary>
        /// Тип хирургического лечения
        /// </summary>
        [XmlElement(ElementName = "HIR_TIP", IsNullable = false)]
        public string HirTypeXml
        {
            get { return HirTypeId.ToStringNullable(); }
            set { HirTypeId = value.ToInt32Nullable(); }
        }
        /// <summary>
        /// Линия лекарственной терапии
        /// </summary>
        [XmlElement(ElementName = "LEK_TIP_L", IsNullable = false)]
        public string LekTypeLXml
        {
            get { return LekTypeLId.ToStringNullable(); }
            set { LekTypeLId = value.ToInt32Nullable(); }
        }
        /// <summary>
        /// Цикл лекарственной терапии
        /// </summary>
        [XmlElement(ElementName = "LEK_TIP_V", IsNullable = false)]
        public string LekTypeVXml
        {
            get { return LekTypeVId.ToStringNullable(); }
            set { LekTypeVId = value.ToInt32Nullable(); }
        }
        /// <summary>
        /// Сведения о введенном противоопухолевом лекарственном препарате 
        /// </summary>
        [XmlElement(ElementName = "LEK_PR", IsNullable = false)]
        public List<AnticancerDrugOnkE> AnticancerDrugOnkXml
        {
            get { return _anticancerDrugOnkCollection; }
            set { _anticancerDrugOnkCollection = value; }
        }
        [XmlElement(ElementName = "PPRT", IsNullable = false)]
        public string PprtXml
        {
            get { return Pprt.ToStringNullable(); }
            set { Pprt = value.ToInt32Nullable(); }
        }
        /// <summary>
        /// Тип лучевой терапии
        /// </summary>
        [XmlElement(ElementName = "LUCH_TIP", IsNullable = false)]
        public string LuchTypeXml
        {
            get { return LuchTypeId.ToStringNullable(); }
            set { LuchTypeId = value.ToInt32Nullable(); }
        }
        [XmlIgnore]
        public int? ServicesOnkTypeId { get; set; }
        [XmlIgnore]
        public int? HirTypeId { get; set; }
        [XmlIgnore]
        public int? LekTypeLId { get; set; }
        [XmlIgnore]
        public int? Pprt { get; set; }
        [XmlIgnore]
        public int? LekTypeVId { get; set; }
        [XmlIgnore]
        public int? LuchTypeId { get; set; }
    }

   
}
