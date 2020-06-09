using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Autofac;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v30K1.D
{
    [XmlRoot("PERS_LIST", IsNullable = true, Namespace = "")]
    public class PersonalRegisterD
    {
        [XmlElement(ElementName = "ZGLV")]
        public PersonalHeader Header { get; set; }  
        
        [XmlElement(ElementName = "PERS")]
        public List<PersonalD> PersonalsCollection { get; set; }

        public PersonalRegisterD()
        {
            PersonalsCollection = new List<PersonalD>();
        }
    }

    public class PersonalHeader
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public PersonalHeader()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "VERSION")]
        public string VersionXml
        {
            get { return _cacheRepository.Get("VersionCache").GetString(Version); }
            set { Version = _cacheRepository.Get("VersionCache").GetBack(value); }
        }

        [XmlElement(ElementName = "DATA")]
        public string DateXml
        {
            get { return Date.HasValue ? Date.Value.ToString("yyyy-MM-dd") : null; }
            set { Date = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd"); }
        }
        [XmlElement(ElementName = "FILENAME")]
        public string FileName { get; set; }
        [XmlElement(ElementName = "FILENAME1")]
        public string MainFileName { get; set; }
         
        [XmlIgnore]
        public int? Version { get; set; }
       

        [XmlIgnore]
        public DateTime? Date { get; set; }

    }

}
