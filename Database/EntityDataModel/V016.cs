namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V016
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string IDDT { get; set; }

        [Required]
        [StringLength(254)]
        public string DTNAME { get; set; }

        [Required]
        [StringLength(15)]
        public string DTRULE { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }
    }
}
