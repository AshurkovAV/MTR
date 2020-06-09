using System.Xml.Serialization;
using Core.Extensions;

namespace Medical.DataCore.ffoms
{
    /// <summary>
    /// Профили медицинской помощи
    /// </summary>
    public class MedicalAssistanceProfiles
    {
        /// <summary>
        /// Код профиля медицинской помощи
        /// </summary>
        [XmlElement(ElementName = "PROFIL_MP")]
        public string ProfileCodeXml
        {
            get { return ProfileCode.ToStringNullable(); }
            set { ProfileCode = value.ToInt32Nullable(); }

        }

        /// <summary>
        /// Значение показателя (объём)
        /// За исключением случаев межтерриториальных расчётов
        /// </summary>
        [XmlElement(ElementName = "OT_KOL")]
        public string IndicatorValueTotalXml
        {
            get { return IndicatorValueTotal.ToStringNullable(); }
            set { IndicatorValueTotal = value.ToInt32Nullable(); }

        }

        /// <summary>
        /// Значение показателя (стоимость)
        /// За исключением случаев межтерриториальных расчётов
        /// </summary>
        [XmlElement(ElementName = "OT_S_KOL")]
        public string IndicatorPriceTotalXml
        {
            get { return IndicatorPriceTotal.ToF2(); }
            set { IndicatorPriceTotal = value.ToDecimalNullable(); }

        }

        /// <summary>
        /// Значение показателя (объём)
        /// За исключением случаев межтерриториальных расчётов
        /// </summary>
        [XmlElement(ElementName = "R_KOL")]
        public string IndicatorValueXml
        {
            get { return IndicatorValue.ToStringNullable(); }
            set { IndicatorValue = value.ToInt32Nullable(); }

        }

        /// <summary>
        /// Значение показателя (стоимость)
        /// За исключением случаев межтерриториальных расчётов
        /// </summary>
        [XmlElement(ElementName = "R_S_KOL")]
        public string IndicatorPriceXml
        {
            get { return IndicatorPrice.ToF2(); }
            set { IndicatorPrice = value.ToDecimalNullable(); }

        }

        /// <summary>
        /// Значение показателя (объём) за отчётный месяц по случаям межтерриториальных расчётов
        /// </summary>
        [XmlElement(ElementName = "R_KOL_M", IsNullable = false)]
        public string IndicatorTerritoryValueXml
        {
            get { return IndicatorTerritoryValue.ToStringNullable(); }
            set { IndicatorTerritoryValue = value.ToInt32Nullable(); }

        }

        /// <summary>
        /// Значение показателя (стоимость) за отчётный месяц по случаям межтерриториальных расчётов
        /// </summary>
        [XmlElement(ElementName = "R_S_KOL_M", IsNullable = false)]
        public string IndicatorTerritoryPriceXml
        {
            get { return IndicatorTerritoryPrice.ToF2(); }
            set { IndicatorTerritoryPrice = value.ToDecimalNullable(); }

        }

        [XmlIgnore]
        public int? ProfileCode { get; set; }

        [XmlIgnore]
        public int? IndicatorValue { get; set; }

        [XmlIgnore]
        public int? IndicatorValueTotal { get; set; }

        [XmlIgnore]
        public decimal? IndicatorPrice{ get; set; }

        [XmlIgnore]
        public decimal? IndicatorPriceTotal { get; set; }

        [XmlIgnore]
        public int? IndicatorTerritoryValue { get; set; }

        [XmlIgnore]
        public decimal? IndicatorTerritoryPrice { get; set; }
    }
}
