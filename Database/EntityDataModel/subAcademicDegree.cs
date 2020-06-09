namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subAcademicDegree")]
    public partial class subAcademicDegree
    {
        public subAcademicDegree()
        {
            subExpertAcademicDegrees = new HashSet<subExpertAcademicDegree>();
        }

        [Key]
        public int AcademicDegreeId { get; set; }

        [StringLength(150)]
        public string AName { get; set; }

        public virtual ICollection<subExpertAcademicDegree> subExpertAcademicDegrees { get; set; }
    }
}
