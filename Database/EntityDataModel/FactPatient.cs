namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactPatient")]
    public partial class FactPatient
    {
        public FactPatient()
        {
            FactActEqmas = new HashSet<FactActEqma>();
            FactActMees = new HashSet<FactActMee>();
            FactExternalRefuses = new HashSet<FactExternalRefuse>();
            FactMECs = new HashSet<FactMEC>();
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
            FactSrzQueries = new HashSet<FactSrzQuery>();
        }

        [Key]
        public int PatientId { get; set; }

        public int? InsuranceId { get; set; }

        public int? AccountId { get; set; }

        public int? MedicalAccountId { get; set; }

        public int? PersonalId { get; set; }

        [StringLength(20)]
        public string INP { get; set; }

        public int? InsuranceDocType { get; set; }

        [StringLength(10)]
        public string InsuranceDocSeries { get; set; }

        [StringLength(20)]
        public string InsuranceDocNumber { get; set; }

        public int? TerritoryOkato { get; set; }

        [StringLength(15)]
        public string InsuranceOgrn { get; set; }

        [StringLength(100)]
        public string InsuranceName { get; set; }

        [StringLength(9)]
        public string Newborn { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public bool? Sended { get; set; }

        public int? ExternalId { get; set; }

        public int? MedicalExternalId { get; set; }

        public int? ExchangeId { get; set; }

        public bool? HasError { get; set; }

        [StringLength(50)]
        public string TortilaGuid { get; set; }

        public int? SrzStatus { get; set; }

        public int? DocumentId { get; set; }

        [StringLength(5)]
        public string InsuranceRegion { get; set; }

        public int? NewbornWeight { get; set; }

        public int? ParentId { get; set; }

        public virtual F002 F002 { get; set; }

        public virtual F008 F008 { get; set; }

        public virtual F010 F010 { get; set; }

        public virtual ICollection<FactActEqma> FactActEqmas { get; set; }

        public virtual ICollection<FactActMee> FactActMees { get; set; }

        public virtual FactDocument FactDocument { get; set; }

        public virtual FactExchange FactExchange { get; set; }

        public virtual ICollection<FactExternalRefuse> FactExternalRefuses { get; set; }

        public virtual ICollection<FactMEC> FactMECs { get; set; }

        public virtual FactMedicalAccount FactMedicalAccount { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }

        public virtual ICollection<FactSrzQuery> FactSrzQueries { get; set; }

        public virtual FactPerson FactPerson { get; set; }

        public virtual FactTerritoryAccount FactTerritoryAccount { get; set; }
    }
}
