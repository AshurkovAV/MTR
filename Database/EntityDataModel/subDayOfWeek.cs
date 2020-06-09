namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subDayOfWeek")]
    public partial class subDayOfWeek
    {
        public subDayOfWeek()
        {
            globalNonworkingDays = new HashSet<globalNonworkingDay>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DayOfWeekId { get; set; }

        [StringLength(11)]
        public string FullName { get; set; }

        [StringLength(5)]
        public string ShortName { get; set; }

        public virtual ICollection<globalNonworkingDay> globalNonworkingDays { get; set; }
    }
}
