namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalQuantityParticularSign")]
    public partial class globalQuantityParticularSign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuantityParticularSignId { get; set; }

        [StringLength(50)]
        public string QuantityParticularSignName { get; set; }
    }
}
