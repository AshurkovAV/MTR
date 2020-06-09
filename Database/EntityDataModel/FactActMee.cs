namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactActMee")]
    public partial class FactActMee
    {
        [Key]
        public int ActMeeId { get; set; }

        public int? Number { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(50)]
        public string Letter { get; set; }

        [StringLength(254)]
        public string Positions { get; set; }

        [StringLength(254)]
        public string Personal { get; set; }

        [StringLength(50)]
        public string PolicyNumber { get; set; }

        [StringLength(254)]
        public string MedicalOrganization { get; set; }

        [StringLength(254)]
        public string History { get; set; }

        public DateTime? EventBegin { get; set; }

        public DateTime? EventEnd { get; set; }

        [StringLength(100)]
        public string DiagnosisGeneral { get; set; }

        [StringLength(254)]
        public string DiagnosisGeneralComments { get; set; }

        [StringLength(100)]
        public string DiagnosisSecondary { get; set; }

        [StringLength(254)]
        public string DiagnosisSecondaryComments { get; set; }

        [StringLength(100)]
        public string Doctor { get; set; }

        [StringLength(254)]
        public string Documentation { get; set; }

        [StringLength(254)]
        public string Duration { get; set; }

        [StringLength(254)]
        public string RefusalCode { get; set; }

        [Column(TypeName = "money")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? AcceptPrice { get; set; }

        public int EmployeeId { get; set; }

        [StringLength(254)]
        public string MedicalOrganizationDirector { get; set; }

        public int? AccountId { get; set; }

        public int PatientId { get; set; }

        public string Conclusion { get; set; }

        [Column(TypeName = "money")]
        public decimal? PenaltyPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal? RefusalPrice { get; set; }

        public int? AccountMoId { get; set; }

        public virtual FactPatient FactPatient { get; set; }
    }
}
