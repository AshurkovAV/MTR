namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PatientShortView")]
    public partial class PatientShortView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PatientId { get; set; }

        public int? AccountId { get; set; }

        public int? MedicalAccountId { get; set; }

        [StringLength(40)]
        public string Surname { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(40)]
        public string Patronymic { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(7)]
        public string Sex { get; set; }

        [StringLength(20)]
        public string INP { get; set; }

        [StringLength(20)]
        public string InsuranceDocNumber { get; set; }

        [StringLength(10)]
        public string InsuranceDocSeries { get; set; }

        [StringLength(5)]
        public string Insurance { get; set; }

        public int? EventCount { get; set; }
    }
}
