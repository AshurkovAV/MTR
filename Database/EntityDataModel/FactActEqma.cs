namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactActEqma")]
    public partial class FactActEqma
    {
        public int? Number { get; set; }

        public DateTime? Date { get; set; }

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

        [Column(TypeName = "money")]
        public decimal? Duration { get; set; }

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

        public string ExpertConclusion { get; set; }

        [Column(TypeName = "money")]
        public decimal? PenaltyPrice { get; set; }

        public int? ExpertId { get; set; }

        [StringLength(250)]
        public string AssignmentOrganization { get; set; }

        public int? AssignmentNumber { get; set; }

        [StringLength(250)]
        public string AssignmentReason { get; set; }

        public DateTime? PersonalBirthday { get; set; }

        [StringLength(250)]
        public string Insurance { get; set; }

        [StringLength(250)]
        public string Department { get; set; }

        [Column(TypeName = "money")]
        public decimal? TotalDuration { get; set; }

        [StringLength(250)]
        public string DiagnosisPostmortemGeneral { get; set; }

        [StringLength(250)]
        public string DiagnosisPostmortemSecondary { get; set; }

        public int? Outcome { get; set; }

        [StringLength(250)]
        public string Surgery { get; set; }

        public DateTime? SurgeryDate { get; set; }

        public int? ExpertMastermind { get; set; }

        [StringLength(250)]
        public string MedicalOrganizationRepresentative { get; set; }

        [Key]
        public int ActEqma { get; set; }

        public string DisclosedDefect { get; set; }

        public string ExpertRecommendation { get; set; }

        public int? AccountMoId { get; set; }

        public virtual FactPatient FactPatient { get; set; }

        [Column(TypeName = "money")]
        public decimal? RefusalPrice { get; set; }

        [StringLength(250)]
        public string WorkPlace { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(500)]
        public string OutcomeComments { get; set; }

        [StringLength(100)]
        public string DiagnosisComplication { get; set; }

        [StringLength(254)]
        public string DiagnosisComplicationComments { get; set; }

        [StringLength(250)]
        public string DiagnosisPostmortemComplication { get; set; }

        public string Anamnesis { get; set; }
        public string AnamnesisError { get; set; }
        public string Diagnosis { get; set; }
        public string DiagnosisError { get; set; }
        public string Cure { get; set; }
        public string CureError { get; set; }
        public string Continuity { get; set; }
        public string ContinuityError { get; set; }
        public string FinalConclusion { get; set; }
        public string SignificantError { get; set; }
    }
}
