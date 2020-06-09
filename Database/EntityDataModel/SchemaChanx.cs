namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SchemaChanx
    {
        [Key]
        public int SchemaChangeID { get; set; }

        [Required]
        [StringLength(2)]
        public string MajorReleaseNumber { get; set; }

        [Required]
        [StringLength(2)]
        public string MinorReleaseNumber { get; set; }

        [Required]
        [StringLength(4)]
        public string PointReleaseNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string ScriptName { get; set; }

        public DateTime DateApplied { get; set; }
    }
}
