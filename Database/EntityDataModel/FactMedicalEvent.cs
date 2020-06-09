namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactMedicalEvent")]
    public partial class FactMedicalEvent
    {
        public FactMedicalEvent()
        {
            FactEQMAs = new HashSet<FactEQMA>();
            FactExternalRefuses = new HashSet<FactExternalRefuse>();
            FactMECs = new HashSet<FactMEC>();
            FactMedicalServices = new HashSet<FactMedicalService>();
            FactMEEs = new HashSet<FactMEE>();
        }

        [Key]
        public int MedicalEventId { get; set; }

        public int? PatientId { get; set; }

        public int? ReferralOrganization { get; set; }

        public int? AssistanceType { get; set; }

        public int? AssistanceConditions { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Hospitalization { get; set; }

        public int? ProfileCodeId { get; set; }

        public int? Department { get; set; }

        public bool? IsChildren { get; set; }

        [StringLength(50)]
        public string History { get; set; }

        public DateTime? EventBegin { get; set; }

        public DateTime? EventEnd { get; set; }

        public int? DiagnosisPrimary { get; set; }

        public int? DiagnosisGeneral { get; set; }

        public int? DiagnosisSecondary { get; set; }

        public int? Result { get; set; }

        public int? Outcome { get; set; }

        public int? SpecialityCode { get; set; }

        [StringLength(16)]
        public string DoctorId { get; set; }

        public int? PaymentMethod { get; set; }

        [Column(TypeName = "money")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal? Rate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? MoPrice { get; set; }

        public int? PaymentStatus { get; set; }

        public int? MoPaymentStatus { get; set; }

        [Column(TypeName = "money")]
        public decimal? AcceptPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal? MEC { get; set; }

        [Column(TypeName = "money")]
        public decimal? MEE { get; set; }

        [Column(TypeName = "money")]
        public decimal? EQMA { get; set; }

        [StringLength(50)]
        public string ParticularCase { get; set; }

        [StringLength(20)]
        public string MES { get; set; }

        [StringLength(20)]
        public string SecondaryMES { get; set; }

        public int? PaymentSourceId { get; set; }

        [StringLength(50)]
        public string RateParticularSign { get; set; }

        [StringLength(50)]
        public string QuantityParticularSign { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        [Column(TypeName = "money")]
        public decimal? UetQuantity { get; set; }

        public int? ExternalId { get; set; }

        public int? MedicalExternalId { get; set; }

        [StringLength(6)]
        public string Subdivision { get; set; }

        [StringLength(6)]
        public string MedicalOrganizationCode { get; set; }

        public int? Request { get; set; }

        public int? RequestStatus { get; set; }

        public int? AssistanceForm { get; set; }

        [StringLength(9)]
        public string HighTechAssistanceType { get; set; }

        public int? HighTechAssistanceMethod { get; set; }

        [StringLength(1000)]
        public string DiagnosisSecondaryAggregate { get; set; }

        [StringLength(1000)]
        public string DiagnosisComplicationAggregate { get; set; }

        [StringLength(1000)]
        public string NewbornsWeightAggregate { get; set; }

        [StringLength(1000)]
        public string MESAggregate { get; set; }

        public int? SpecialityCodeV015 { get; set; }

        [Column(TypeName = "money")]
        public decimal? RefusalPrice { get; set; }

        public int? EventType { get; set; }

        public long? Flag { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual F005 F005 { get; set; }

        public virtual ICollection<FactEQMA> FactEQMAs { get; set; }

        public virtual ICollection<FactExternalRefuse> FactExternalRefuses { get; set; }

        public virtual ICollection<FactMEC> FactMECs { get; set; }

        public virtual V006 V006 { get; set; }

        public virtual V008 V008 { get; set; }

        public virtual V009 V009 { get; set; }

        public virtual V012 V012 { get; set; }

        public virtual M001 M001 { get; set; }

        public virtual M001 M0011 { get; set; }

        public virtual M001 M0012 { get; set; }

        public virtual localPaymentSource localPaymentSource { get; set; }

        public virtual V010 V010 { get; set; }

        public virtual V002 V002 { get; set; }

        public virtual V004 V004 { get; set; }

        public virtual ICollection<FactMedicalService> FactMedicalServices { get; set; }

        public virtual FactPatient FactPatient { get; set; }

        public virtual ICollection<FactMEE> FactMEEs { get; set; }

        public int? RegionalAttribute { get; set; }
        public int? HealthGroup { get; set; }

        public F009 JobStatus { get; set; }
        public int? IdKsg { get; set; }
    }
}
