namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localEmployee")]
    public partial class localEmployee
    {
        [StringLength(40)]
        public string Surname { get; set; }

        [StringLength(40)]
        public string EName { get; set; }

        [StringLength(40)]
        public string Patronymic { get; set; }

        [StringLength(40)]
        public string Position { get; set; }

        [StringLength(40)]
        public string Speciality { get; set; }

        [StringLength(40)]
        public string Phone { get; set; }

        [Key]
        public int EmployeeId { get; set; }

        [StringLength(20)]
        public string ConfNumber { get; set; }
    }
}
