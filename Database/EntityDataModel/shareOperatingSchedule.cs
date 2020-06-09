namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareOperatingSchedule")]
    public partial class shareOperatingSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OperatingScheduleId { get; set; }

        public int MedicalOrganizationId { get; set; }

        public DateTime? Date { get; set; }

        public bool? Monday { get; set; }

        public bool? Tuesday { get; set; }

        public bool? Wednesday { get; set; }

        public bool? Thursday { get; set; }

        public bool? Friday { get; set; }

        public bool? Saturday { get; set; }

        public bool? Sunday { get; set; }

        public int? AssistanceConditionsId { get; set; }

        public int? DepartmentId { get; set; }

        public int? SubdivisionId { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual shareMedicalDepartment shareMedicalDepartment { get; set; }

        public virtual shareMedicalSubdivision shareMedicalSubdivision { get; set; }
    }
}
