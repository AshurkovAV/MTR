using System;
using System.Xml.Serialization;
using Core.Helpers;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v10.E
{
    public class PatientE : IPatient
    {
        [XmlElement(ElementName = "VPOLIS")]
        public string InsuranceDocTypeXml
        {
            get
            {
                return InsuranceDocType.HasValue ? InsuranceDocType.Value.ToString() : null;
            }
            set
            {
                InsuranceDocType = SafeConvert.ToInt32(value,false);
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
                if (!string.IsNullOrWhiteSpace(value))
                    InsuranceDocSeries = value.Trim();
            }
        }

        [XmlElement(ElementName = "NPOLIS")]
        public string PolicyNumber  
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(INP))
                    return INP.Trim();

                return InsuranceDocNumber;
            }
            set {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                switch (InsuranceDocType)
                {
                    case 1:
                    case 2:
                        InsuranceDocNumber = value.Trim();
                        break;
                    case 3:
                        INP = value.Trim();
                        break;
                }
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
                if (!string.IsNullOrWhiteSpace(value))
                    Surname = value.Trim();
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
                if (!string.IsNullOrWhiteSpace(value))
                    PName = value.Trim();
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
                if (!string.IsNullOrWhiteSpace(value))
                    Patronymic = value.Trim();
            }
        }
        [XmlElement(ElementName = "W")]
        public string SexXml
        {
            get { return Sex.HasValue ? Sex.Value.ToString() : null; }
            set { Sex = SafeConvert.ToInt32(value,false);}
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
                Birthday = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }
        }

        [XmlElement(ElementName = "FAM_P", IsNullable = false)]
        public string RepresentativeSurnameXml
        {
            get
            {
                return RepresentativeSurname;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
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
                   RepresentativeName = value.Trim();
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
                    RepresentativePatronymic = value.Trim();
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
                RepresentativeSex = SafeConvert.ToInt32(value,false);
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
                RepresentativeBirthday = SafeConvert.ToDateTimeExact(value, "yyyy-MM-dd");
            }

        }

        [XmlElement(ElementName = "MR", IsNullable = false)]
        public string BirthPlaceXml
        {
            get
            {
                return BirthPlace;
            }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                    BirthPlace = value.Trim();
            }
        }

        [XmlElement(ElementName = "DOCTYPE", IsNullable = false)]
        public string DocTypeXml
        {
            get { return DocType.HasValue ? DocType.Value.ToString() : null; }
            set
            {
                DocType = SafeConvert.ToInt32(value,false);
            }
        }

        [XmlElement(ElementName = "DOCSER", IsNullable = false)]
        public string DocSeriesXml
        {
            get { return DocSeries; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    DocSeries = value.Trim();
            }
        }

        [XmlElement(ElementName = "DOCNUM", IsNullable = false)]
        public string DocNumberXml
        {
            get { return DocNumber; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    DocNumber = value.Trim();
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
                if (!string.IsNullOrWhiteSpace(value))
                    SNILS = value.Trim();
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
                AddressReg = SafeConvert.ToInt32(value);
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
                AddressLive = SafeConvert.ToInt32(value);
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
                if (!string.IsNullOrWhiteSpace(value))
                    Newborn = value.Trim();
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
                if (!string.IsNullOrWhiteSpace(value))
                    Comments = value.Trim();
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
    }
}
