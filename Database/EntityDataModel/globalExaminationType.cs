namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalExaminationType")]
    public partial class globalExaminationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ExaminationTypeID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? IDVID { get; set; }
    }
}
