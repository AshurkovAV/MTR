namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalClassifierType")]
    public partial class globalClassifierType
    {
        [Key]
        public int ClassifierTypeID { get; set; }

        [StringLength(50)]
        public string ClassifierXmlName { get; set; }

        [StringLength(50)]
        public string ClassifierName { get; set; }
    }
}
