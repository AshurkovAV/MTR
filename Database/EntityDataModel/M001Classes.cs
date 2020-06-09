namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class M001Classes
    {
        public M001Classes()
        {
            M001 = new HashSet<M001>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(10)]
        public string ClassId { get; set; }

        [StringLength(254)]
        public string ClassName { get; set; }

        public virtual ICollection<M001> M001 { get; set; }
    }
}
