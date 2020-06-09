namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactProcessing")]
    public partial class FactProcessing
    {
        [Key]
        public int ProcessingId { get; set; }

        public int? Weight { get; set; }

        public virtual globalProcessingType ProcessingType { get; set; }

        public bool? IsEnable { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public virtual globalScope Scope { get; set; }

        public virtual globalVersion Version { get; set; }

        public string Query { get; set; }

        [StringLength(254)]
        public string Description { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}
