namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class O003
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(1)]
        public string RAZDEL { get; set; }

        [Required]
        [StringLength(2)]
        public string PRAZDEL { get; set; }

        [Required]
        [StringLength(8)]
        public string KOD { get; set; }

        [Required]
        [StringLength(250)]
        public string NAME11 { get; set; }

        [Required]
        [StringLength(250)]
        public string NAME12 { get; set; }

        [StringLength(250)]
        public string NOMDESCR { get; set; }

        [StringLength(3)]
        public string NOMAKT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? STATUS { get; set; }

        public DateTime? DATA_UPD { get; set; }
    }
}
