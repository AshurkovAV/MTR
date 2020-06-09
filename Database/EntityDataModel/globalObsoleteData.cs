namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalObsoleteData")]
    public partial class globalObsoleteData
    {
        [Key]
        public int ObsoleteDataID { get; set; }

        public int OldValue { get; set; }

        public int NewValue { get; set; }

        [Required]
        [StringLength(100)]
        public string Classifier { get; set; }
    }
}
