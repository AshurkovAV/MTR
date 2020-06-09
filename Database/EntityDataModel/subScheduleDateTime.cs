namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subScheduleDateTime")]
    public partial class subScheduleDateTime
    {
        [Key]
        public int ScheduleDateTimeId { get; set; }

        public int? DoctorScheduleId { get; set; }

        public DateTime? WorkTimeBegin { get; set; }

        public DateTime? WorkTimeEnd { get; set; }

        public bool? EvenSign { get; set; }

        public DateTime? WorkDate { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public virtual localDoctorSchedule localDoctorSchedule { get; set; }
    }
}
