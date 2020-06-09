using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Core.Extensions;
using Core.Helpers;

namespace Medical.DataCore.v21K2.D
{
    public class PersonalD
    {
        [XmlElement(ElementName = "ID_PAC")] 
        public string PatientGuid { get; set; }
        [XmlElement(ElementName = "FAM")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "IM")]    
        public string PName { get; set; }
        [XmlElement(ElementName = "OT")]    
        public string Patronymic { get; set; }
        [XmlElement(ElementName = "W")]     
        public int? Sex { get; set; }
        [XmlElement(ElementName = "DR")]
        public string BirthdayXml
        {
            get
            {
                return Birthday!=null ? Birthday.Value.ToString("yyyy-MM-dd") : null;
            }
            set
            {
                Birthday = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
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
                if(!string.IsNullOrWhiteSpace(value))
                    RepresentativeSurname = value.Trim();
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
                if (!string.IsNullOrWhiteSpace(value))
                    RepresentativeName = value;
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
                if (!string.IsNullOrWhiteSpace(value))
                    RepresentativePatronymic = value;
            }
        }
        [XmlElement(ElementName = "W_P", IsNullable = false)]
        public string RepresentativeSexXml
        {
            get
            {
                return RepresentativeSex.HasValue ? RepresentativeSex.Value.ToString() : null;
            }
            set { RepresentativeSex = SafeConvert.ToInt32(value); }
        }
        [XmlElement(ElementName = "DR_P", IsNullable = false)]
        public string RepresentativeBirthdayXml
        {
            get
            {
                if (RepresentativeBirthday == null)
                    return null;

                return RepresentativeBirthday.Value.ToString("yyyy-MM-dd");
            }
            set
            {
                RepresentativeBirthday = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
                if (value.IsNotNullOrWhiteSpace() && RepresentativeBirthday == null)
                {
                    RepresentativeBirthday = value.ToDateTime();
                }

            }

        }

        [XmlElement(ElementName = "DOST_P")]
        public List<string> ReliabilityRepresentativeXml { get; set; }

        [XmlElement(ElementName = "MR", IsNullable = false)]
        public string BirthPlaceXml
        {
            get { return BirthPlace; }
            set { BirthPlace = value.TrimNullable(); }
        }

        [XmlElement(ElementName = "DOCTYPE", IsNullable = false)]
        public string DocTypeXml
        {
            get { return DocType.HasValue ? DocType.Value.ToString(CultureInfo.InvariantCulture) : null; }
            set { DocType = SafeConvert.ToInt32(value); }
        }

        [XmlElement(ElementName = "DOCSER", IsNullable = false)]
        public string DocSeriesXml
        {
            get { return DocSeries; }
            set { DocSeries = value; }
        }

        [XmlElement(ElementName = "DOCNUM", IsNullable = false)]
        public string DocNumberXml
        {

            get { return DocNumber; }
            set { DocNumber = value; }
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
                if (!string.IsNullOrWhiteSpace(value))
                    SNILS = value.Trim();
            }
        }

        [XmlElement(ElementName = "OKATOG", IsNullable = false)]
        public string AddressRegXml
        {
            get { return AddressReg.ToStringNullable(); }
            set { AddressReg = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "OKATOP", IsNullable = false)]
        public string AddressLiveXml
        {
            get { return AddressLive.ToStringNullable(); }
            set { AddressLive = value.ToInt32Nullable(); }
        }

        [XmlElement(ElementName = "COMENTP", IsNullable = false)]
        public string CommentsXml
        {
            get { return Comments; }
            set { Comments = value.TrimNullable(); }
        }

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
        public string DocSeries { get; set; }
        [XmlIgnore]
        public string DocNumber { get; set; }

        [XmlIgnore]
        public string Reliability
        {
            get { return ReliabilityXml.AggregateToString(); }
            set { ReliabilityXml = value.ToListByDelimiter(); }
        }

        [XmlIgnore]
        public string ReliabilityRepresentative
        {
            get { return ReliabilityRepresentativeXml.AggregateToString(); }
            set { ReliabilityRepresentativeXml = value.ToListByDelimiter(); }
        }
    }
}
