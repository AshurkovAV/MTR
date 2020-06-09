namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localOptionsDictionary")]
    public partial class localOptionsDictionary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OptionsDictionaryId { get; set; }

        public int Key { get; set; }

        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }
    }
}
