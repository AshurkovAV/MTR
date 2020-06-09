namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventShortView")]
    public partial class EventShortView
    {
        public int? ExternalId { get; set; }

        public int? AccountId { get; set; }

        public int? MedicalAccountId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventId { get; set; }

        public int? PatientId { get; set; }

        [StringLength(40)]
        public string Surname { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(40)]
        public string Patronymic { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(7)]
        public string Sex { get; set; }

        [StringLength(100)]
        public string BirthPlace { get; set; }

        [StringLength(10)]
        public string DocSeries { get; set; }

        [StringLength(20)]
        public string DocNum { get; set; }

        [StringLength(20)]
        public string INP { get; set; }

        [StringLength(20)]
        public string InsuranceDocNumber { get; set; }

        [StringLength(10)]
        public string InsuranceDocSeries { get; set; }

        [StringLength(5)]
        public string Insurance { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? AssistanceConditions { get; set; }

        [StringLength(10)]
        public string Diagnosis { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? Rate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal? AcceptPrice { get; set; }

        public DateTime? EventBegin { get; set; }

        public DateTime? EventEnd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Profile { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Speciality { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Result { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Outcome { get; set; }

        [StringLength(254)]
        public string PaymentStatus { get; set; }

        [Column(TypeName = "money")]
        public decimal? MEC { get; set; }

        [Column(TypeName = "money")]
        public decimal? MEE { get; set; }

        [Column(TypeName = "money")]
        public decimal? EQMA { get; set; }

        [StringLength(250)]
        public string EventComments { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? AssistanceType { get; set; }
    }
}
