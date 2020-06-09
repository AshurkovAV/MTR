namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F013
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(5)]
        public string smocod { get; set; }

        [Required]
        [StringLength(3)]
        public string codpvp { get; set; }

        [Required]
        [StringLength(254)]
        public string Address { get; set; }

        [Required]
        [StringLength(40)]
        public string Phone { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }
    }
}
