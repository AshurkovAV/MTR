namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class M001Sections
    {
        public M001Sections()
        {
            M001 = new HashSet<M001>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(10)]
        public string SectionId { get; set; }

        [StringLength(254)]
        public string SectionName { get; set; }

        public virtual ICollection<M001> M001 { get; set; }
    }
}
