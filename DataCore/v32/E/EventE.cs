using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v32.E
{
    public class EventE: IZEvent
    {
        #region Inner
        [XmlIgnore]
        public List<IService> InnerServiceCollection
        {
            get { return _serviceCollection.Select(p => p as IService).ToList(); }
            set
            {
                _serviceCollection = value.Select(p => p as ServiceE).ToList();
            }
        }
        [XmlIgnore]
        public List<IDirectionOnkE> InnerDirectionOnkCollection
        {
            get { return _directionOnkECollection.Select(p => p as IDirectionOnkE).ToList(); }
            set
            {
                _directionOnkECollection = value.Select(p => p as DirectionOnkE).ToList();
            }
        }
        [XmlIgnore]
        public List<IConsultationsOnk> InnerConsultationsOnkCollection
        {
            get { return _consultationsOnkCollection.Select(p => p as IConsultationsOnk).ToList(); }
            set
            {
                _consultationsOnkCollection = value.Select(p => p as ConsultationsOnkE).ToList();
            }
        }
        [XmlIgnore]
        public IEventOnk InnerEventOnk
        {
            get { return _eventOnk as IEventOnk; }
            set
            {
                _eventOnk = value as EventOnkE;
            }
        }
        [XmlIgnore]
        public List<object> InnerRefusals
        {
            get { return _refusals.Select(p => p as object).ToList(); }
            set { _refusals = value.Select(p => p as RefusalE).ToList(); }
        }

        [XmlIgnore]
        public IKsgKpg InnerKsgKpg
        {
            get { return _ksgKpg; }
            set { _ksgKpg = value as KsgKpgE; }
        }

        [XmlIgnore]
        public List<ServiceE> _serviceCollection { get; set; }
        [XmlIgnore]
        public List<DirectionOnkE> _directionOnkECollection { get; set; }
        [XmlIgnore]
        public List<ConsultationsOnkE> _consultationsOnkCollection { get; set; }
        [XmlIgnore]
        public EventOnkE _eventOnk { get; set; }

        [XmlIgnore]
        public List<RefusalE> _refusals { get; set; }
        [XmlIgnore]
        public KsgKpgE _ksgKpg { get; set; }


        #endregion


        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
            _serviceCollection = new List<ServiceE>();
            _directionOnkECollection = new List<DirectionOnkE>();
            _consultationsOnkCollection = new List<ConsultationsOnkE>();
            //_eventOnk = new EventOnkE();
            //_ksgKpg = new KsgKpgE();
            InnerRefusals = new List<object>();

        }

        [XmlElement(ElementName = "SL_ID")]
        public string ExternalId { get; set; }

        [XmlElement(ElementName = "VID_HMP", IsNullable = false)]
        public string HighTechAssistanceTypeXml
        {
            get { return HighTechAssistanceType; }
            set { HighTechAssistanceType = value; }
        }

        [XmlElement(ElementName = "METOD_HMP", IsNullable = false)]
        public string HighTechAssistanceMethodXml
        {
            get { return HighTechAssistanceMethod.HasValue ? HighTechAssistanceMethod.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { HighTechAssistanceMethod = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "PROFIL")]
        public int? ProfileCodeId { get; set; }

        [XmlElement(ElementName = "PROFIL_K", IsNullable = false)]
        public string ProfileCodeBedXml
        {
            get { return ProfileCodeBedId.HasValue ? ProfileCodeBedId.Value.ToString(CultureInfo.InvariantCulture):null; }
            set { ProfileCodeBedId = SafeConvert.ToInt32(value); }
        }
        [XmlIgnore]
        public int? ProfileCodeBedId { get; set; }

        [XmlElement(ElementName = "DET")]
        public int IsChildrenXml
        {
            get { return IsChildren.HasValue && IsChildren.Value ? 1 : 0; }
            set { IsChildren = value == 1; }
        }
        [XmlElement(ElementName = "P_CEL", IsNullable = false)]
        public string Pcel { get; set; }

        [XmlElement(ElementName = "DISP")]
        public string DispXml
        {
            get { return Disp.HasValue ? Disp.ToString() : null; }
            set { Disp = SafeConvert.ToInt32(value, false); }
        }

        [XmlElement(ElementName = "TAL_D")]
        public string HightTechAssistanceTalonXml
        {
            get
            {
                return HightTechAssistanceTalon.HasValue ? HightTechAssistanceTalon.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                HightTechAssistanceTalon = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "NHISTORY")]
        public string HistoryXml
        {
            get { return History; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                    History = value.Trim();
            }
        } 
        [XmlElement(ElementName = "DATE_1")]
        public string EventBeginXml
        {
            get
            {
                return EventBegin.HasValue ? EventBegin.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                EventBegin = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        } 
        [XmlElement(ElementName = "DATE_2")]
        public string EventEndXml
        {
            get
            {
                return EventEnd.HasValue ? EventEnd.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                EventEnd = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Продолжительность госпитализации(койко-дни) XMl
        /// </summary>
        [XmlElement(ElementName = "KD", IsNullable = false)]
        public string KdayXml
        {
            get
            {
                return Kday.HasValue ? Kday.Value.ToString(CultureInfo.InvariantCulture) : null;
            }
            set { Kday = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "DS0", IsNullable = false)]
        public string DiagnosisPrimaryXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(DiagnosisPrimary); }
            set { DiagnosisPrimary = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }


        [XmlElement(ElementName = "DS1")]
        public string DiagnosisGeneralXml
        {
            get { return _cacheRepository.Get("IDC10Cache").GetString(DiagnosisGeneral); }
            set { DiagnosisGeneral = _cacheRepository.Get("IDC10Cache").GetBack(value); }
        }

        [XmlElement(ElementName = "DS2", IsNullable = false)]
        public List<string> DiagnosisSecondaryXml { get; set; }

        [XmlElement(ElementName = "DS3", IsNullable = false)]
        public List<string> DiagnosisComplicationXml { get; set; }

        [XmlElement(ElementName = "C_ZAB")]
        public string CzabXml
        {
            get { return Czab.HasValue ? Czab.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { Czab = SafeConvert.ToInt32(value); }
        }
        [XmlElement(ElementName = "DS_ONK")]
        public string SignSuspectedDsOnkXml
        {
            get { return SignSuspectedDsOnk.HasValue ? SignSuspectedDsOnk.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { SignSuspectedDsOnk = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "DN")]
        public string DnXml
        {
            get { return Dn.HasValue ? Dn.ToString() : null; }
            set { Dn = SafeConvert.ToInt32(value, false); }
        }
        
        [XmlElement(ElementName = "CODE_MES1", IsNullable = false)]
        public List<string> MESXml { get; set; }

        [XmlElement(ElementName = "CODE_MES2", IsNullable = false)]
        public string SecondaryMESXml
        {
            get { return SecondaryMES; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    SecondaryMES = value.Trim();
            }
        }
        /// <summary>
        /// Направления
        /// </summary>
        [XmlElement(ElementName = "NAPR", IsNullable = false)]
        public List<DirectionOnkE> DirectionOnkXml
        {
            get { return _directionOnkECollection; }
            set { _directionOnkECollection = value; }
        }

        /// <summary>
        /// Сведения о проведении консилиума
        /// </summary>
        [XmlElement(ElementName = "CONS", IsNullable = false)]
        public List<ConsultationsOnkE> ConsultationsOnkXml
        {
            get { return _consultationsOnkCollection; }
            set { _consultationsOnkCollection = value; }
        }

        [XmlElement(ElementName = "ONK_SL")]
        public EventOnkE EventOnk
        {
            get { return _eventOnk;}
            set { _eventOnk = value; }
        }

        [XmlElement(ElementName = "KSG_KPG", IsNullable = false)]
        public KsgKpgE KsgKpgXml
        {
            get { return _ksgKpg; }
            set { _ksgKpg = value; }
        }

        [XmlElement(ElementName = "REAB")]
        public int? ReabXml
        {
            get { return Reab.HasValue && Reab.Value ? 1 : (int?)null; }
            set { Reab = value == 1; }
        }

        [XmlElement(ElementName = "PRVS")]
        public string SpecialityCodeXml { get; set; }

        [XmlElement(ElementName = "VERS_SPEC")]
        public string SpecialityClassifierVersion { get; set; }
  
        [XmlElement(ElementName = "ED_COL", IsNullable = false)]
        public string QuantityXml
        {
            get
            {
                return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Quantity = SafeConvert.ToDecimal(value); }
        }
      
        [XmlElement(ElementName = "TARIF", IsNullable = false)]
        public string RateXml
        {
            get
            {
                return Rate.HasValue ? Rate.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Rate = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SUM_M")]
        public string PriceXml
        {
            get
            {
                return EventPrice.HasValue ? EventPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { EventPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "USL")]
        public List<ServiceE> ServiceXml {
            get { return _serviceCollection; }
            set { _serviceCollection = value; }
        }

        [XmlElement(ElementName = "COMENTSL", IsNullable = false)]
        public string CommentsXml
        {
            get
            {
                return Comments;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comments = value.Trim();
            }
        }
        
        [XmlIgnore]
        public bool? IsChildren { get; set; }
        [XmlIgnore]
        public bool? Reab { get; set; }
        [XmlIgnore]
        public string History { get; set; }
        [XmlIgnore]
        public DateTime? HightTechAssistanceTalon { get; set; }
        [XmlIgnore]
        public DateTime? EventBegin { get; set; } 
        [XmlIgnore]
        public DateTime? EventEnd { get; set; } 
        [XmlIgnore]
        public int? DiagnosisPrimary { get; set; } 
        [XmlIgnore]
        public int? DiagnosisGeneral { get; set; }
        [XmlIgnore]
        public int? Czab { get; set; }
        [XmlIgnore]
        public int? Disp { get; set; }
        [XmlIgnore]
        public int? Dn { get; set; }
        [XmlIgnore]
        public decimal? Rate { get; set; }
        [XmlIgnore]
        public decimal? EventPrice { get; set; } 
        [XmlIgnore]
        public decimal? Quantity { get; set; }
        /// <summary>
        /// Продолжительность госпитализации(койко-дни)
        /// </summary>
        [XmlIgnore]
        public int? Kday { get; set; }
        [XmlIgnore]
        public string Comments { get; set; } 
        
        [XmlIgnore]
        public string HighTechAssistanceType { get; set; }

        [XmlIgnore]
        public int? HighTechAssistanceMethod { get; set; }
       

        [XmlIgnore]
        public List<string> DiagnosisSecondary { get; set; }

        [XmlIgnore]
        public string SecondaryMES { get; set; }
        [XmlIgnore]
        public int? SignSuspectedDsOnk { get; set; }

        [XmlIgnore]
        public int? SpecialityCodeV021
        {
            get
            {
                if (!SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V021")
                {
                    return _cacheRepository.Get("V021Cache").GetBack(SpecialityCodeXml);
                }
                return null;
            }
            set
            {
                if (SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V021")
                {
                    SpecialityCodeXml = _cacheRepository.Get("V021Cache").GetString(value);
                }
            }
        }
        [XmlIgnore]
        public int? SpecialityCodeV015
        {
            get
            {
                if (SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V015")
                {
                    return _cacheRepository.Get("V015Cache").GetBack(SpecialityCodeXml);
                }
                return null;
            }
            set
            {
                if (SpecialityClassifierVersion.IsNullOrWhiteSpace() || SpecialityClassifierVersion == "V015")
                {
                    SpecialityCodeXml = _cacheRepository.Get("V015Cache").GetString(value);
                }
            }
        }
        [XmlIgnore]
        public string DiagnosisSecondaryAggregate
        {
            get { return DiagnosisSecondary.AggregateToString(); }
            set { DiagnosisSecondary = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string DiagnosisComplicationAggregate
        {
            get { return DiagnosisComplicationXml.AggregateToString(); }
            set { DiagnosisComplicationXml = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string MESAggregate
        {
            get { return MESXml.AggregateToString(); }
            set { MESXml = value.ToListByDelimiter(); }
        }
        
    }
  

    public class LocalElement
    {
        [XmlElement(ElementName = "RateParticularSign")]
        public string RateParticularSign { get; set; } // nvarchar(50)
    }

}
