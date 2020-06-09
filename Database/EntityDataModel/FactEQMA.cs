namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEQMA")]
    public partial class FactEQMA
    {
        [Key]
        public int EQMAId { get; set; }

        public DateTime? Date { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public int? ReasonId { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        public int? PatientId { get; set; }

        public int? MedicalEventId { get; set; }

        public int? ActId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Penalty { get; set; }

        public int? EmployeeId { get; set; }

        public int? Source { get; set; }

        [StringLength(36)]
        public string ExternalGuid { get; set; }

        public virtual F014 F014 { get; set; }

        public virtual FactMedicalEvent FactMedicalEvent { get; set; }
    }
}
