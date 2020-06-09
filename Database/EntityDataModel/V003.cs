namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V003
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDLIC { get; set; }

        [StringLength(254)]
        public string LICNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IERARH { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PRIM { get; set; }
    }
}
