namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class globalSchoolStatu
    {
        [Key]
        public int SchoolStatusID { get; set; }

        public int Code { get; set; }

        [Required]
        [StringLength(254)]
        public string Name { get; set; }
    }
}
