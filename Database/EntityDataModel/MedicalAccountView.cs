namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedicalAccountView")]
    public partial class MedicalAccountView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        [StringLength(6)]
        public string CodeMo { get; set; }

        [StringLength(250)]
        public string ShortNameMo { get; set; }
    }
}
