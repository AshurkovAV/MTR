namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactExpertCriterion")]
    public partial class FactExpertCriterion
    {
        [StringLength(20)]
        public string Code { get; set; }

        public int? Weight { get; set; }

        public int? Reason { get; set; }

        [StringLength(100)]
        public string PG { get; set; }

        public int? Type { get; set; }

        public bool? IsEnable { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public int? Scope { get; set; }

        public string Query { get; set; }

        [StringLength(254)]
        public string Description { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public bool? IsAuto { get; set; }

        [Column(TypeName = "money")]
        public decimal? PenaltyPercent { get; set; }

        public int? ErrorScope { get; set; }

        [Column(TypeName = "money")]
        public decimal? RefusalPercent { get; set; }

        public int FactExpertCriterionID { get; set; }

        public int? Version { get; set; }

        public int? Group { get; set; }

        public virtual globalExaminationGroup globalExaminationGroup { get; set; }

        public virtual globalVersion globalVersion { get; set; }
    }
}
