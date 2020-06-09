namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactClassifierVersion")]
    public partial class FactClassifierVersion
    {
        [Key]
        public int ClassifierVersionID { get; set; }

        [Required]
        [StringLength(50)]
        public string ClassifierName { get; set; }

        [Required]
        [StringLength(10)]
        public string ClassifierVersion { get; set; }

        public DateTime ClassifierVersionDate { get; set; }

        public DateTime ClassifierUpdateDate { get; set; }
    }
}
