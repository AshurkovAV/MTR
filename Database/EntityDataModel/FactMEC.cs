namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactMEC")]
    public partial class FactMEC
    {
        [Key]
        public int MECId { get; set; }

        public DateTime? Date { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public int? ReasonId { get; set; }

        public int EmployeeId { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        public int? PatientId { get; set; }

        public int? MedicalEventId { get; set; }

        public int? Type { get; set; }

        public bool? IsLock { get; set; }

        public int? Source { get; set; }

        [StringLength(36)]
        public string ExternalGuid { get; set; }

        public virtual F014 F014 { get; set; }

        public virtual FactMedicalEvent FactMedicalEvent { get; set; }

        public virtual FactPatient FactPatient { get; set; }
    }
}
