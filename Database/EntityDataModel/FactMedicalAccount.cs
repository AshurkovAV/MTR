namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactMedicalAccount")]
    public partial class FactMedicalAccount
    {
        public FactMedicalAccount()
        {
            FactPatients = new HashSet<FactPatient>();
        }

        [Key]
        public int MedicalAccountId { get; set; }

        public int? MedicalOrganization { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(15)]
        public string AccountNumber { get; set; }

        public DateTime? AccountDate { get; set; }

        public int? InsurancePayer { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? AcceptPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal? MECPenalties { get; set; }

        [Column(TypeName = "money")]
        public decimal? MEEPenalties { get; set; }

        [Column(TypeName = "money")]
        public decimal? EQMAPenalties { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public int? Status { get; set; }

        public int? Flags { get; set; }

        public virtual F002 F002 { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }
    }
}
