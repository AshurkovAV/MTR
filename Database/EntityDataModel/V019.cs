namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V019
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int IDHM { get; set; }

        [Required]
        [StringLength(1000)]
        public string HMNAME { get; set; }

        [Required]
        [StringLength(1000)]
        public string DIAG { get; set; }

        [Required]
        [StringLength(9)]
        public string HVID { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }
    }
}
