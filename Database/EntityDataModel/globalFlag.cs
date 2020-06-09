namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalFlag")]
    public partial class globalFlag
    {
        [Key]
        public int FlagID { get; set; }

        [Required]
        [StringLength(254)]
        public string Name { get; set; }

        public int Flag { get; set; }
    }
}
