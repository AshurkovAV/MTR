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
    public class AnticancerDrugOnkE: IAnticancerDrugOnk
    {

        [XmlIgnore]
        private readonly ICacheRepository _cacheRepository;
        /// <summary>
        /// LEK_PR Сведения о введенном противоопухолевом лекарственном препарате 
        /// </summary>
        public AnticancerDrugOnkE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "REGNUM")]
        public string RegNum { get; set; }

        [XmlElement(ElementName = "CODE_SH")]
        public string CodeSh { get; set; }


        [XmlElement(ElementName = "DATE_INJ")]
        public string DataInjXml
        {
            get { return DataInj.HasValue ? DataInj.Value.ToString("yyyy-MM-dd") : null; }
            set
            {
                DataInj = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }
        [XmlIgnore]
        public DateTime? DataInj { get; set; }

    }
}
