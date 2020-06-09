namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class O004
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(2)]
        public string KOD { get; set; }

        [Required]
        [StringLength(250)]
        public string NAME1 { get; set; }

        [StringLength(52)]
        public string ALG { get; set; }

        [StringLength(3)]
        public string NOMAKT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? STATUS { get; set; }

        public DateTime? DATA_UPD { get; set; }
    }
}
