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

namespace Medical.DataCore.v30K1.D
{
    public class EventD
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

        public EventD()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }
        [XmlIgnore]
        public KsgKpgD InnerKsgKpg
        {
            get { return _ksgKpg; }
            set { _ksgKpg = value as KsgKpgD; }
        }
        [XmlIgnore]
        public KsgKpgD _ksgKpg { get; set; }

        [XmlElement(ElementName = "SL_ID")]
        public string SlIdGuid { get; set; }
        [XmlElement(ElementName = "USL_OK")]
        public string AssistanceConditionsXml
        {
            get
            {
                return AssistanceConditions.HasValue ? AssistanceConditions.ToString() : null;
            }
            set { AssistanceConditions = SafeConvert.ToInt32(value); }
        }

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
        [XmlElement(ElementName = "LPU_1", IsNullable = false)]
        public string SubdivisionXml
        {
            get { return Subdivision; }
            set { Subdivision = value; }
        }

        [XmlElement(ElementName = "PODR")]
        public string DepartmentXml
        {
            get
            {
                return Department.HasValue ? Department.Value.ToString(CultureInfo.InvariantCulture) : null;
            }
            set
            {
                Department = SafeConvert.ToInt32(value);
            }
        }
       
        [XmlElement(ElementName = "PROFIL")]
        public int? ProfileCodeId { get; set; }
        [XmlElement(ElementName = "PROFIL_K", IsNullable = false)]
        public int ProfileCodeBedId { get; set; }
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
        public List<string> DiagnosisSecondaryXml
        {
            get
            {
                return DiagnosisSecondary;
            }
            set
            {
                DiagnosisSecondary = value;
            }
        }

        [XmlElement(ElementName = "DS3", IsNullable = false)]
        public List<string> DiagnosisComplicationXml { get; set; }
      

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

        [XmlElement(ElementName = "KSG_KPG", IsNullable = false)]
        public KsgKpgD KsgKpgXml
        {
            get { return _ksgKpg; }
            set { _ksgKpg = value; }
        }

        [XmlElement(ElementName = "PRVS")]
        public string SpecialityCodeXml { get; set; }

        [XmlElement(ElementName = "VERS_SPEC", IsNullable = false)]
        public string SpecialityClassifierVersion { get; set; }
  
        [XmlElement(ElementName = "IDDOKT")]
        public string DoctorIdXml
        {
            get { return DoctorId; }
            set { DoctorId = value; }
        }
        [XmlElement(ElementName = "ED_COL", IsNullable = false)]
        public string QuantityXml
        {
            get
            {
                return Quantity.HasValue ? Quantity.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Quantity = SafeConvert.ToDecimal(value); }
        }
      
       

        [XmlElement(ElementName = "USL")]
        public List<ServiceD> Service { get; set; }

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
        public string EventPriceXml
        {
            get
            {
                return EventPrice.HasValue ? EventPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { EventPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SUMP")]
        public string AcceptPriceXml
        {
            get
            {
                return AcceptPrice.HasValue ? AcceptPrice.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { AcceptPrice = SafeConvert.ToDecimal(value); }
        }

        [XmlElement(ElementName = "SANK", IsNullable = false)]
        public List<RefusalD> RefusalsXml
        {
            get { return Refusals; }
            set { Refusals = value; }
        }
       
        /// <summary>
        /// Продолжительность госпитализации(койко-дни) XMl
        /// </summary>
        [XmlElement(ElementName = "KD", IsNullable = false)]
        public string KdayXml
        {
            get
            {
                return Kday.HasValue ? Kday.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { Kday = SafeConvert.ToDecimal(value); }
        }
        //[XmlElement(ElementName = "KSG", IsNullable = false)]
        //public List<KsgKpgD> Ksg { get; set; }

        [XmlElement(ElementName = "DS1_PR", IsNullable = false)]
        public int Ds1pr { get; set; }
        [XmlElement(ElementName = "PR_D_N", IsNullable = false)]
        public int PrDn { get; set; }
        [XmlElement(ElementName = "DS2_N", IsNullable = false)]
        public List<DiagnosisSecondaryD> Ds2N { get; set; }

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
        public int? AssistanceConditions { get; set; }
        [XmlIgnore]
        public int? Department { get; set; }
        [XmlIgnore]
        public bool? IsChildren { get; set; } 
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
        public int? Disp { get; set; }
        [XmlIgnore]
        public decimal? Rate { get; set; }
        [XmlIgnore]
        public decimal? EventPrice { get; set; }
        [XmlIgnore]
        public decimal? AcceptPrice { get; set; }
        [XmlIgnore]
        public decimal? Quantity { get; set; }
        /// <summary>
        /// Продолжительность госпитализации(койко-дни)
        /// </summary>
        [XmlIgnore]
        public decimal? Kday { get; set; }
        [XmlIgnore]
        public string Comments { get; set; } 
     
        [XmlIgnore]
        public string DoctorId { get; set; }
        [XmlIgnore]
        public string Subdivision { get; set; }
        [XmlIgnore]
        public string HighTechAssistanceType { get; set; }

        [XmlIgnore]
        public int? HighTechAssistanceMethod { get; set; }

        [XmlIgnore]
        public List<RefusalD> Refusals { get; set; }

        [XmlIgnore]
        public List<string> DiagnosisSecondary { get; set; }

        [XmlIgnore]
        public string SecondaryMES { get; set; }

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
