namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class localIDC10GroupComposition
    {
        [Key]
        public int IDC10GroupCompositionId { get; set; }

        public int? IDC10GroupId { get; set; }

        public int? IDC10Id { get; set; }

        public virtual localIDC10Group localIDC10Group { get; set; }

        public virtual M001 M001 { get; set; }
    }
}
