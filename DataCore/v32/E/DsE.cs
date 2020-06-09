using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v32.E
{
    public class DsE : IDs
    {
        public DsE()
        {
        }

        [XmlElement(ElementName = "DS2")]
        public string Ds { get; set; }
        
        [XmlIgnore]    
        public int? DsType { get; set; }
    }
}
