namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalCurePeriod")]
    public partial class globalCurePeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CurePeriodId { get; set; }

        public int ProfileId { get; set; }

        [Column(TypeName = "money")]
        public decimal Duration { get; set; }
    }
}
