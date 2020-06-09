namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalParticularCase")]
    public partial class globalParticularCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ParticularCaseID { get; set; }

        public int ParticularCaseCode { get; set; }

        [Required]
        [StringLength(254)]
        public string ParticularCaseName { get; set; }
    }
}
