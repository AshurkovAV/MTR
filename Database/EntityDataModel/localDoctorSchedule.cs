namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localDoctorSchedule")]
    public partial class localDoctorSchedule
    {
        public localDoctorSchedule()
        {
            subScheduleDateTimes = new HashSet<subScheduleDateTime>();
        }

        [Key]
        public int DoctorScheduleId { get; set; }

        public int? DoctorId { get; set; }

        public int? NurseId { get; set; }

        [StringLength(10)]
        public string RoomNumber { get; set; }

        public virtual ICollection<subScheduleDateTime> subScheduleDateTimes { get; set; }
    }
}
