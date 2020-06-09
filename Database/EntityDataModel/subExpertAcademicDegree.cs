namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subExpertAcademicDegree")]
    public partial class subExpertAcademicDegree
    {
        [Key]
        public int ExpertAcademicDegreeId { get; set; }

        public int ExpertId { get; set; }

        public int AcademicDegreeId { get; set; }

        public virtual F004 F004 { get; set; }

        public virtual subAcademicDegree subAcademicDegree { get; set; }
    }
}
