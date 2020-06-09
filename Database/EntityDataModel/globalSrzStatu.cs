namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class globalSrzStatu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SrzStatusId { get; set; }

        public int? SrzStatusCode { get; set; }

        [StringLength(50)]
        public string SrzStatusName { get; set; }
    }
}
