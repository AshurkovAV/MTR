namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareMedicalDepartment")]
    public partial class shareMedicalDepartment
    {
        public shareMedicalDepartment()
        {
            shareDoctors = new HashSet<shareDoctor>();
            shareOperatingSchedules = new HashSet<shareOperatingSchedule>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MedicalDepartmentId { get; set; }

        public int? MedicalOrganizationId { get; set; }

        public int? DepartmentCode { get; set; }

        [StringLength(254)]
        public string DepartmentName { get; set; }

        public int? AssistanceConditionsId { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual F003 F0031 { get; set; }

        public virtual ICollection<shareDoctor> shareDoctors { get; set; }

        public virtual ICollection<shareOperatingSchedule> shareOperatingSchedules { get; set; }
    }
}
