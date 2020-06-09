namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactPreparedReport")]
    public partial class FactPreparedReport
    {
        [Key]
        public int PreparedReportId { get; set; }

        [Required]
        public byte[] Body { get; set; }

        public DateTime Date { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        public int ExternalId { get; set; }

        public int Scope { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int Number { get; set; }

        public int ReportId { get; set; }

        public int PageCount { get; set; }

        public int? SubId { get; set; }
    }
}
