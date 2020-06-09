using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Autofac;
using Core.Extensions;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v30.E
{
    public class PatientE : IPatient
    {
        [XmlIgnore] 
        private readonly ICacheRepository _cacheRepository;

         public PatientE()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();

        }

        [XmlElement(ElementName = "VPOLIS")]
        public string InsuranceDocTypeXml
        {
            get
            {
                return InsuranceDocType.HasValue ? InsuranceDocType.Value.ToString() : null;
            }
            set
            {
                InsuranceDocType = SafeConvert.ToInt32(value.TrimNullable(),false);
            }
        }

        [XmlElement(ElementName = "SPOLIS", IsNullable = false)]
        public string InsuranceDocSeriesXml
        {
            get
            {
                return InsuranceDocSeries;
            }
            set
            {
                InsuranceDocSeries = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "NPOLIS", IsNullable = false)]
        public string InsuranceDocNumberXml  
        {
            get
            {
                return InsuranceDocNumber;
            }
            set {
                InsuranceDocNumber = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "ENP")]
        public string InpXml
        {
            get
            {
                return INP;
            }
            set
            {
                INP = value.TrimNullable();
            }
        }
        /// <summary>
        /// Регион страхования
        /// </summary>
        [XmlElement(ElementName = "ST_OKATO", IsNullable = false)]
        public string InsuranceRegionXml
        {
            get
            {
                return InsuranceRegion;
            }
            set
            {
                InsuranceRegion = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "FAM")]
        public string SurnameXml
        {
            get
            {
                return Surname;
            }
            set
            {
                Surname = value.TrimNullable();
            }
        }
        [XmlElement(ElementName = "IM")]
        public string PNameXml
        {
            get
            {
                return PName;
            }
            set
            {
                PName = value.TrimNullable();
            }
        }
        [XmlElement(ElementName = "OT")]
        public string PatronymicXml
        {
            get
            {
                return Patronymic;
            }
            set
            {
                Patronymic = value.TrimNullable();
            }
        }
        [XmlElement(ElementName = "W")]
        public string SexXml
        {
            get
            {
                return Sex.HasValue ? Sex.Value.ToString() : null;
            }
            set
            {
                Sex = SafeConvert.ToInt32(value.TrimNullable(),false);
            }
        }

        [XmlElement(ElementName = "DR")]
        public string BirthdayXml
        {
            get
            {
                return Birthday.HasValue ? Birthday.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                Birthday = SafeConvert.ToDateTimeExact(value.TrimNullable(), "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "DOST")]
        public List<string> ReliabilityXml { get; set; }

        [XmlElement(ElementName = "FAM_P", IsNullable = false)]
        public string RepresentativeSurnameXml
        {
            get
            {
                return RepresentativeSurname;
            }
            set
            {
                RepresentativeSurname = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "IM_P", IsNullable = false)]
        public string RepresentativeNameXml
        {
            get
            {
                return RepresentativeName;
            }
            set
            {
                RepresentativeName = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "OT_P", IsNullable = false)]
        public string RepresentativePatronymicXml
        {
            get
            {
                return RepresentativePatronymic;
            }
            set
            {
                RepresentativePatronymic = value.TrimNullable();
            }
        }
        [XmlElement(ElementName = "W_P", IsNullable = false)]
        public string RepresentativeSexXml
        {
            get
            {
                return RepresentativeSex.HasValue ? RepresentativeSex.Value.ToString() : null;
            }
            set
            {
                RepresentativeSex = SafeConvert.ToInt32(value.TrimNullable(),false);
            }
        }
        [XmlElement(ElementName = "DR_P", IsNullable = false)]
        public string RepresentativeBirthdayXml
        {
            get
            {
                return RepresentativeBirthday.HasValue ? RepresentativeBirthday.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                RepresentativeBirthday = SafeConvert.ToDateTimeExact(value.TrimNullable(), "yyyy-MM-dd");
            }

        }

        [XmlElement(ElementName = "DOST_P")]
        public List<string> ReliabilityRepresentativeXml { get; set; }

        [XmlElement(ElementName = "MR", IsNullable = false)]
        public string BirthPlaceXml
        {
            get
            {
                return BirthPlace;
            }
            set
            {
                BirthPlace = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "DOCTYPE", IsNullable = false)]
        public string DocTypeXml
        {
            get { return DocType.HasValue ? DocType.Value.ToString() : null; }
            set
            {
                DocType = SafeConvert.ToInt32(value.TrimNullable(),false);
            }
        }

        [XmlElement(ElementName = "DOCSER", IsNullable = false)]
        public string DocSeriesXml
        {
            get { return DocSeries; }
            set
            {
                DocSeries = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "DOCNUM", IsNullable = false)]
        public string DocNumberXml
        {
            get { return DocNumber; }
            set
            {
                DocNumber = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "SNILS", IsNullable = false)]
        public string SNILSXml
        {
            get
            {
                return SNILS;
            }
            set
            {
                SNILS = value.TrimNullable();
            }
        }

        [XmlElement(ElementName = "OKATOG", IsNullable = false)]
        public string AddressRegXml
        {
            get
            {
                return AddressReg.HasValue ? AddressReg.Value.ToString() : null;
            }
            set
            {
                AddressReg = SafeConvert.ToInt32(value.TrimNullable());
            }
        }
        [XmlElement(ElementName = "OKATOP", IsNullable = false)]
        public string AddressLiveXml
        {
            get
            {
                return AddressLive.HasValue ? AddressLive.Value.ToString() : null;
            }
            set
            {
                AddressLive = SafeConvert.ToInt32(value.TrimNullable());
            }
        }

        [XmlElement(ElementName = "NOVOR")]
        public string NewbornXml
        {
            get
            {
                return Newborn;
            }
            set
            {
                Newborn = value.TrimNullable();
            }
        }
        /// <summary>
        /// Вес при рождении
        /// </summary>
        [XmlElement(ElementName = "VNOV_D", IsNullable = false)]
        public string NewbornWeightXml
        {
            get
            {
                return NewbornWeight.ToStringNullable();
            }
            set
            {
                NewbornWeight = value.TrimNullable().ToInt32Nullable();
            }
        } 

        [XmlElement(ElementName = "COMENTP", IsNullable = false)]
        public string CommentsXml
        {
            get
            {
                return Comments;
            }
            set
            {
                Comments = value.TrimNullable();
            }
        }

        [XmlIgnore]
        public int? Sex { get; set; }
        [XmlIgnore]
        public int? InsuranceDocType { get; set; }
        [XmlIgnore]
        public string Surname { get; set; }
        [XmlIgnore]
        public string PName { get; set; }
        [XmlIgnore]
        public string Patronymic { get; set; }
        [XmlIgnore]
        public string Newborn { get; set; }
        [XmlIgnore]
        public DateTime? Birthday { get; set; }
        [XmlIgnore]
        public string BirthPlace { get; set; }
        [XmlIgnore]
        public int? AddressReg { get; set; }
        [XmlIgnore]
        public int? AddressLive { get; set; }
        [XmlIgnore]
        public string SNILS { get; set; }
        [XmlIgnore]
        public string RepresentativeName { get; set; }
        [XmlIgnore]
        public string RepresentativeSurname { get; set; }
        [XmlIgnore]
        public string RepresentativePatronymic { get; set; }
        [XmlIgnore]
        public int? RepresentativeSex { get; set; }
        [XmlIgnore]
        public DateTime? RepresentativeBirthday { get; set; }
        [XmlIgnore]
        public string Comments { get; set; }

        [XmlIgnore]
        public int? DocType { get; set; }
        [XmlIgnore]
        public string DocOrg { get; set; }
        [XmlIgnore]
        public DateTime? DocDate { get; set; }

        [XmlIgnore]
        public string DocSeries { get; set; }
        [XmlIgnore]
        public string DocNumber { get; set; } 

        [XmlIgnore]
        public string INP { get; set; } 
        [XmlIgnore]
        public string InsuranceDocNumber { get; set; }
        [XmlIgnore]
        public string InsuranceDocSeries { get; set; }
        [XmlIgnore]
        public string InsuranceRegion { get; set; }
        
        [XmlIgnore]
        public string Reliability {
            get { return ReliabilityXml.AggregateToString(); }
            set { ReliabilityXml = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string ReliabilityRepresentative
        {
            get { return ReliabilityRepresentativeXml.AggregateToString(); }
            set { ReliabilityRepresentativeXml = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public int? NewbornWeight { get; set; }



        [XmlIgnore]
        public string PolicyNumber { get; set; }
    }
}
