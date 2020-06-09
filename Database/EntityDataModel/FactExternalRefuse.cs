namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactExternalRefuse")]
    public partial class FactExternalRefuse
    {
        [Key]
        public int ExternalRefuseId { get; set; }

        public int MedicalEventId { get; set; }

        public int ReasonId { get; set; }

        public int Generation { get; set; }

        public int PatientId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public bool? IsAgree { get; set; }

        public int? Type { get; set; }

        [StringLength(36)]
        public string ExternalGuid { get; set; }

        [StringLength(250)]
        public string Comment { get; set; }

        public int? Source { get; set; }

        public virtual F014 F014 { get; set; }

        public virtual FactMedicalEvent FactMedicalEvent { get; set; }

        public virtual FactPatient FactPatient { get; set; }
    }
}
