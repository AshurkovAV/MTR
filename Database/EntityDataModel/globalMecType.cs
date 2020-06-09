namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalMecType")]
    public partial class globalMecType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MecTypeId { get; set; }

        public int Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
