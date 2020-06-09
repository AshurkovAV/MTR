namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MES")]
    public partial class ME
    {
        [Key]
        public int MESId { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(254)]
        public string MName { get; set; }

        public double? ComplexServicePart { get; set; }

        public double? UETDoctor { get; set; }

        public double? UETPersonal { get; set; }

        public double? CorrectionFactor { get; set; }

        public double? UETService { get; set; }
    }
}
