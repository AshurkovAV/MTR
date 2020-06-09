namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareMedicalSubdivision")]
    public partial class shareMedicalSubdivision
    {
        public shareMedicalSubdivision()
        {
            shareOperatingSchedules = new HashSet<shareOperatingSchedule>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MedicalSubdivisionId { get; set; }

        public int? SubdivisionCode { get; set; }

        [StringLength(254)]
        public string SubdivisionName { get; set; }

        public int AssistanceConditionsId { get; set; }

        public virtual V006 V006 { get; set; }

        public virtual ICollection<shareOperatingSchedule> shareOperatingSchedules { get; set; }
    }
}
