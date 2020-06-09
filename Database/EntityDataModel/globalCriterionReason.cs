namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalCriterionReason")]
    public partial class globalCriterionReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CriterionReasonID { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string SourceName { get; set; }
    }
}
