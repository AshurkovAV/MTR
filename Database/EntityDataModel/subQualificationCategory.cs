namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subQualificationCategory")]
    public partial class subQualificationCategory
    {
        [Key]
        public int QualificationCategoryId { get; set; }

        public int ExpertId { get; set; }

        [Required]
        [StringLength(150)]
        public string Category { get; set; }

        public virtual F004 F004 { get; set; }
    }
}
