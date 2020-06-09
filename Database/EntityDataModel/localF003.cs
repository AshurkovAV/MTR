namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class localF003
    {
        [StringLength(250)]
        public string ShortName { get; set; }

        [StringLength(40)]
        public string PositionName { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(50)]
        public string Patronymic { get; set; }

        [StringLength(5)]
        public string OKATO { get; set; }

        [StringLength(6)]
        public string Code { get; set; }

        [StringLength(254)]
        public string FullAddress { get; set; }

        [StringLength(40)]
        public string Phone { get; set; }

        public int LocalF003Id { get; set; }
    }
}
