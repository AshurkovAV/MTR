namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalProcessingType")]
    public partial class globalProcessingType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProcessingTypeId { get; set; }

        public int Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
