namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F007
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IDVED { get; set; }

        [Required]
        [StringLength(254)]
        public string VEDNAME { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }
    }
}
