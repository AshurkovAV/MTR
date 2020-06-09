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

namespace Medical.DataCore.v30.E
{
    public class KsgKpgE
    {
        [XmlIgnore]
        public List<ISlKoef> InnerSlCoefCollection
        {
            get { return _slCoefCollection.Select(p => p as ISlKoef).ToList(); }
            set
            {
                _slCoefCollection = value.Select(p => p as SlKoefE).ToList();
            }
        }
        [XmlIgnore]
        public List<SlKoefE> _slCoefCollection { get; set; }
        [XmlIgnore]
        private readonly ICacheRepository _cacheRepository;

        public KsgKpgE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            _slCoefCollection = new List<SlKoefE>();
        }

        [XmlElement(ElementName = "N_KSG", IsNullable = false)]
        public string KksgXml
        {
            get { return Kksg.HasValue ? Kksg.ToString() : null; }
            set { Kksg = SafeConvert.ToInt32(value, false); }
        }

        [XmlIgnore]
        public int? Kksg { get; set; }
        [XmlElement(ElementName = "VER_KSG")]
        public int? VersionKsg { get; set; }

        [XmlElement(ElementName = "KSG_PG")]
        public int IndicationPgKsgXml
        {
            get { return IndicationPgKsg.HasValue && IndicationPgKsg.Value ? 1 : 0; }
            set { IndicationPgKsg = value == 1; }
        }
        [XmlElement(ElementName = "N_KPG", IsNullable = false)]
        public string KkpgXml
        {

            get { return Kkpg.HasValue ? Kkpg.ToString() : null; }
            set { Kkpg = SafeConvert.ToInt32(value, false); }
        }
        [XmlIgnore]
        public int? Kkpg { get; set; }
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
        [XmlElement(ElementName = "DKK1", IsNullable = false)]
        public string AdditionСlassificatoryСriterionXml
        {
            get { return _cacheRepository.Get("V024Cache").GetString(AdditionСlassificatoryСriterion); }
            set { AdditionСlassificatoryСriterion = _cacheRepository.Get("V024Cache").GetBack(value); }
        }
        [XmlIgnore]
        public int? AdditionСlassificatoryСriterion { get; set; }
        [XmlIgnore]
        public int? AdditionСlassificatoryСriterion2 { get; set; }
        [XmlElement(ElementName = "DKK2", IsNullable = false)]
        public string AdditionСlassificatoryСriterionXml2
        {
            get { return _cacheRepository.Get("V024Cache").GetString(AdditionСlassificatoryСriterion2); }
            set { AdditionСlassificatoryСriterion2 = _cacheRepository.Get("V024Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "SL_K")]
        public int IndicationKslpXml
        {
            get { return IndicationKslp.HasValue && IndicationKslp.Value ? 1 : 0; }
            set { IndicationKslp = value == 1; }
        }
        [XmlElement(ElementName = "IT_SL", IsNullable = false)]
        public decimal ItSl { get; set; }

        [XmlElement(ElementName = "SL_KOEF", IsNullable = false)]
        public List<SlKoefE> SlKoef
        {
            get { return _slCoefCollection; }
            set { _slCoefCollection = value; }
        }
        [XmlIgnore]
        public bool? IndicationPgKsg { get; set; }
        [XmlIgnore]
        public bool? IndicationKslp { get; set; }
        [XmlIgnore]
        public List<string> CritXml
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
