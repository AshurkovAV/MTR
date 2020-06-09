namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localUser")]
    public partial class localUser
    {
        [Key]
        public int UserID { get; set; }

        [StringLength(200)]
        public string Login { get; set; }

        [StringLength(200)]
        public string Pass { get; set; }

        [StringLength(200)]
        public string Salt { get; set; }

        public virtual localRole RoleID { get; set; }
        public bool Active { get; set; }

        [MaxLength(200)]
        public string LastName { get; set; }
        [MaxLength(200)]
        public string FirstName { get; set; }
        [MaxLength(200)]
        public string Patronymic { get; set; }
        [MaxLength(254)]
        public string Position { get; set; }
        [MaxLength(200)]
        public string Phone { get; set; }
        [MaxLength(200)]
        public string ConfNumber { get; set; }
        
       
    }
}
