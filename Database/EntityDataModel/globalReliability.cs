namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalReliability")]
    public partial class globalReliability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ReliabilityID { get; set; }

        public int Code { get; set; }

        [Required]
        [StringLength(254)]
        public string Name { get; set; }
    }
}
