namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalClassifierMap")]
    public partial class globalClassifierMap
    {
        [Key]
        public int ClassifierMapID { get; set; }

        [Required]
        [StringLength(50)]
        public string XmlName { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool Immutable { get; set; }

        public int? Converter { get; set; }
    }
}
