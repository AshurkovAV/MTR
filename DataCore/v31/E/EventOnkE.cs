using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v31.E
{
    /// <summary>
    /// ONK_SL
    /// </summary>
    public class EventOnkE : IEventOnk
    {
        [XmlIgnore]
        public List<IDiagBlokOnk> InnerDiagBlokOnkCollection
        {
            get { return _diagBlokOnkCollection.Select(p => p as IDiagBlokOnk).ToList(); }
            set
            {
                _diagBlokOnkCollection = value.Select(p => p as DiagBlokOnkE).ToList();
            }
        }
        [XmlIgnore]
        public List<DiagBlokOnkE> _diagBlokOnkCollection { get; set; }
        [XmlIgnore]
        public List<IContraindicationsOnk> InnerСontraindicationsOnkCollection
        {
            get { return _contraindicationsOnkCollection.Select(p => p as IContraindicationsOnk).ToList(); }
            set
            {
                _contraindicationsOnkCollection = value.Select(p => p as ContraindicationsOnkE).ToList();
            }
        }
        [XmlIgnore]
        public List<ContraindicationsOnkE> _contraindicationsOnkCollection { get; set; }
        [XmlIgnore]
        public List<IServiceOnk> InnerServiceOnkCollection
        {
            get { return _serviceOnkCollection.Select(p => p as IServiceOnk).ToList(); }
            set
            {
                _serviceOnkCollection = value.Select(p => p as ServiceOnkE).ToList();
            }
        }
        [XmlIgnore]
        public List<ServiceOnkE> _serviceOnkCollection { get; set; }
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventOnkE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            _diagBlokOnkCollection = new List<DiagBlokOnkE>();
            _contraindicationsOnkCollection = new List<ContraindicationsOnkE>();
            _serviceOnkCollection = new List<ServiceOnkE>();
        }

        [XmlElement(ElementName = "DS1_T")]
        public string Ds1tXml
        {
            get { return Ds1t.ToStringNullable(); }
            set { Ds1t = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "STAD", IsNullable = false)]
        public string StageDiseaseXml
        {
            get { return StageDisease.ToStringNullable(); }
            set { StageDisease = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "ONK_T", IsNullable = false)]
        public string OnkTXml
        {
            get { return OnkT.ToStringNullable(); }
            set { OnkT = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "ONK_N", IsNullable = false)]
        public string OnkNXml
        {
            get { return OnkN.ToStringNullable(); }
            set { OnkN = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "ONK_M", IsNullable = false)]
        public string OnkMXml
        {
            get { return OnkM.ToStringNullable(); }
            set { OnkM = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "MTSTZ", IsNullable = false)]
        public string MtstzXml
        {
            get { return Mtstz.ToStringNullable(); }
            set { Mtstz = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "SOD", IsNullable = false)]
        public string SodXml
        {
            get { return Sod.ToF2(); }
            set { Sod = value.ToDecimalNullable(); }
        }
        [XmlElement(ElementName = "K_FR", IsNullable = false)]
        public string KfrXml
        {
            get { return Kfr.ToStringNullable(); }
            set { Kfr = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "WEI", IsNullable = false)]
        public string WeiXml
        {
            get { return Wei.ToF1(); }
            set { Wei = value.ToDecimalNullable(); }
        }
        [XmlElement(ElementName = "HEI", IsNullable = false)]
        public string HeiXml
        {
            get { return Hei.ToStringNullable(); }
            set { Hei = value.ToInt32Nullable(); }
        }
        [XmlElement(ElementName = "BSA", IsNullable = false)]
        public string BsaXml
        {
            get { return Bsa.ToF2(); }
            set { Bsa = value.ToDecimalNullable(); }
        }
        /// <summary>
        /// Диагностический блок
        /// </summary>
        [XmlElement(ElementName = "B_DIAG")]
        public List<DiagBlokOnkE> DiagBlokOnk
        {
            get { return _diagBlokOnkCollection; }
            set { _diagBlokOnkCollection = value; }
        }

        /// <summary>
        /// Сведения об имеющихся противопоказаниях и отказах
        /// </summary>
        [XmlElement(ElementName = "B_PROT")]
        public List<ContraindicationsOnkE> СontraindicationsOnk
        {
            get { return _contraindicationsOnkCollection; }
            set { _contraindicationsOnkCollection = value; }
        }

        /// <summary>
        /// Сведения об услуге при лечении онкологического заболевания
        /// </summary>
        [XmlElement(ElementName = "ONK_USL", IsNullable = false)]
        public List<ServiceOnkE> ServiceOnk
        {
            get { return _serviceOnkCollection; }
            set { _serviceOnkCollection = value; }
        }

        [XmlIgnore]
        public decimal? Sod { get; set; }
        [XmlIgnore]
        public decimal? Wei { get; set; }
        [XmlIgnore]
        public decimal? Bsa { get; set; }
        [XmlIgnore]
        public int? Ds1t { get; set; }
        [XmlIgnore]
        public int? StageDisease { get; set; }
        [XmlIgnore]
        public int? OnkT { get; set; }
        [XmlIgnore]
        public int? OnkN { get; set; }
        [XmlIgnore]
        public int? OnkM { get; set; }
        [XmlIgnore]
        public int? Mtstz { get; set; }
        [XmlIgnore]
        public int? Kfr { get; set; }
        [XmlIgnore]
        public int? Hei { get; set; }

    }

   
}
