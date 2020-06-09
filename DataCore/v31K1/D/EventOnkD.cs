using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DataCore.v31K1.D
{
    /// <summary>
    /// ONK_SL
    /// </summary>
    public class EventOnkD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventOnkD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        [XmlElement(ElementName = "DS1_T")]
        public string Ds1tXml {
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
        public List<DiagBlokOnkD> DiagBlokOnk { get; set; }

        /// <summary>
        /// Сведения об имеющихся противопоказаниях и отказах
        /// </summary>
        [XmlElement(ElementName = "B_PROT")]
        public List<ContraindicationsOnkD> СontraindicationsOnk { get; set; }
        /// <summary>
        /// Сведения об услуге при лечении онкологического заболевания
        /// </summary>
        [XmlElement(ElementName = "ONK_USL", IsNullable = false)]
        public List<ServiceOnkD> ServiceOnk { get; set; }


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
