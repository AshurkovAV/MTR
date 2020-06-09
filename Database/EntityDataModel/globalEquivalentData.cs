namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalEquivalentData")]
    public partial class globalEquivalentData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EquivalentDataId { get; set; }

        public int ProfileId { get; set; }

        public int OldProfileId { get; set; }

        public int SpecialityId { get; set; }

        public bool? IsChildren { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual globalOldProfile globalOldProfile { get; set; }

        public virtual V002 V002 { get; set; }

        public virtual V004 V004 { get; set; }
    }
}
