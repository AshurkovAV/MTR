namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactSrzQuery")]
    public partial class FactSrzQuery
    {
        [Key]
        public int SrzQueryId { get; set; }

        public int? PatientId { get; set; }

        public int? Status { get; set; }

        [StringLength(50)]
        public string Guid { get; set; }

        public DateTime? DateQuery { get; set; }

        public DateTime? DateUpdate { get; set; }

        public bool? IsReaded { get; set; }

        public int? Type { get; set; }

        public virtual FactPatient FactPatient { get; set; }
    }
}
