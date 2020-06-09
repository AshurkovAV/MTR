namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalScope")]
    public partial class globalScope
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScopeID { get; set; }

        [StringLength(20)]
        public string Name { get; set; }
    }
}
