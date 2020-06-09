namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V017
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int IDDR { get; set; }

        [Required]
        [StringLength(254)]
        public string DRNAME { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }
    }
}
