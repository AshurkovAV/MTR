namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class localIDC10Group
    {
        public localIDC10Group()
        {
            localIDC10GroupComposition = new HashSet<localIDC10GroupComposition>();
            shareRates = new HashSet<shareRate>();
        }

        [Key]
        public int IDC10GroupId { get; set; }

        [StringLength(250)]
        public string IDC10GroupName { get; set; }

        public virtual ICollection<localIDC10GroupComposition> localIDC10GroupComposition { get; set; }

        public virtual ICollection<shareRate> shareRates { get; set; }
    }
}
