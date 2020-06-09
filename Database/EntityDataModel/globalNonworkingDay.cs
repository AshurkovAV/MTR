namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalNonworkingDay")]
    public partial class globalNonworkingDay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NonworkingDayId { get; set; }

        public int DayWeek { get; set; }

        public DateTime? DayDate { get; set; }

        public int? DayType { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public virtual subDayOfWeek subDayOfWeek { get; set; }
    }
}
