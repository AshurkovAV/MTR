namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactReport")]
    public partial class FactReport
    {
        public int? Type { get; set; }

        public bool? IsEnable { get; set; }

        public int? Scope { get; set; }

        [StringLength(254)]
        public string Description { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public bool? IsAuto { get; set; }

        public int FactReportID { get; set; }

        public byte[] Body { get; set; }

        public bool? IsMultiple { get; set; }
    }
}
