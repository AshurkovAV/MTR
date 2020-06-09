namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalV015EqV002")]
    public partial class globalV015EqV002
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int V015EqV002Id { get; set; }

        public virtual V002 Profile { get; set; }

        public virtual V015 Speciality { get; set; }
    }
}
