namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class O001
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string KOD { get; set; }

        [Required]
        [StringLength(250)]
        public string NAME11 { get; set; }

        [Required]
        [StringLength(250)]
        public string NAME12 { get; set; }

        [Required]
        [StringLength(2)]
        public string ALFA2 { get; set; }

        [StringLength(3)]
        public string ALFA3 { get; set; }

        [StringLength(250)]
        public string NOMDESCR { get; set; }

        [StringLength(3)]
        public string NOMAKT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? STATUS { get; set; }

        public DateTime? DATA_UPD { get; set; }
    }
}
