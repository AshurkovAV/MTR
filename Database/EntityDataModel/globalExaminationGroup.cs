namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalExaminationGroup")]
    public partial class globalExaminationGroup
    {
        public globalExaminationGroup()
        {
            FactExpertCriterions = new HashSet<FactExpertCriterion>();
            FactExpertCriterion_copy = new HashSet<FactExpertCriterion_copy>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ExaminationGroupID { get; set; }

        [Required]
        [StringLength(254)]
        public string Name { get; set; }

        public virtual ICollection<FactExpertCriterion> FactExpertCriterions { get; set; }

        public virtual ICollection<FactExpertCriterion_copy> FactExpertCriterion_copy { get; set; }
    }
}
