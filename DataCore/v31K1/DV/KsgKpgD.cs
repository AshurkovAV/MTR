using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;
using Core.Helpers;

namespace Medical.DataCore.v31K1.DV
{
    public class KsgKpgD : IKsgKpg
    {
        [XmlIgnore]
        public List<ISlKoef> InnerSlCoefCollection
        {
            get { return _slCoefCollection.Select(p => p as ISlKoef).ToList(); }
            set
            {
                _slCoefCollection = value.Select(p => p as SlKoefD).ToList();
            }
        }
        [XmlIgnore]
        public List<SlKoefD> _slCoefCollection { get; set; }
        [XmlIgnore]
        private readonly ICacheRepository _cacheRepository;

        public KsgKpgD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            _slCoefCollection = new List<SlKoefD>();
        }

        [XmlElement(ElementName = "N_KSG", IsNullable = false)]
        public string KksgXml
        {
            get { return Kksg; }
            set { Kksg = value; }
        }

        [XmlIgnore]
        public string Kksg { get; set; }
        [XmlElement(ElementName = "VER_KSG")]
        public int? VersionKsg { get; set; }

        [XmlElement(ElementName = "KSG_PG")]
        public int IndicationPgKsgXml
        {
            get { return IndicationPgKsg.HasValue && IndicationPgKsg.Value ? 1 : 0; }
            set { IndicationPgKsg = value == 1; }
        }

        [XmlElement(ElementName = "N_KPG", IsNullable = false)]
        public string Kkpg { get; set; }
        
        [XmlElement(ElementName = "KOEF_Z")]
        public decimal? KoefZ { get; set; }
        [XmlElement(ElementName = "KOEF_UP")]
        public decimal? KoefUp { get; set; }
        [XmlElement(ElementName = "BZTSZ")]
        public decimal? BaseRate { get; set; }
        [XmlElement(ElementName = "KOEF_D")]
        public decimal? KoefD { get; set; }
        [XmlElement(ElementName = "KOEF_U")]
        public decimal? KoefU { get; set; }
        [XmlElement(ElementName = "CRIT", IsNullable = false)]
        public List<string> CritXml{get; set;}

        [XmlElement(ElementName = "SL_K")]
        public int IndicationKslpXml
        {
            get { return IndicationKslp.HasValue && IndicationKslp.Value ? 1 : 0; }
            set { IndicationKslp = value == 1; }
        }
        [XmlElement(ElementName = "IT_SL", IsNullable = false)]
        public decimal ItSl { get; set; }

        [XmlElement(ElementName = "SL_KOEF", IsNullable = false)]
        public List<SlKoefD> SlKoef
        {
            get { return _slCoefCollection; }
            set { _slCoefCollection = value; }
        }
        [XmlIgnore]
        public bool? IndicationPgKsg { get; set; }
        [XmlIgnore]
        public bool? IndicationKslp { get; set; }

    }
}
