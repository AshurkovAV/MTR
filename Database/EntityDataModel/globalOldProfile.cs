namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalOldProfile")]
    public partial class globalOldProfile
    {
        public globalOldProfile()
        {
            globalEquivalentDatas = new HashSet<globalEquivalentData>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OldProfileId { get; set; }

        public int? Code { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        public virtual ICollection<globalEquivalentData> globalEquivalentDatas { get; set; }
    }
}
