namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localLogin")]
    public partial class localLogin
    {
        public int? EmployeeId { get; set; }

        public int? Role { get; set; }

        [StringLength(40)]
        public string Login { get; set; }

        [StringLength(32)]
        public string Password { get; set; }

        [Key]
        public int LoginId { get; set; }
    }
}
