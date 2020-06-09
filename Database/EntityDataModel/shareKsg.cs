namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareKsg")]
    public partial class shareKsg
    {
        public int ID { get; set; }

        public int KsgID { get; set; }

        [Required]
        [StringLength(254)]
        public string Name { get; set; }

        public int Norm { get; set; }

        [Column(TypeName = "money")]
        public decimal Rate { get; set; }

        [Column(TypeName = "money")]
        public decimal Rate2 { get; set; }

        [Column(TypeName = "money")]
        public decimal Rate3 { get; set; }

        [Column(TypeName = "money")]
        public decimal Rate4 { get; set; }

        [Column(TypeName = "money")]
        public decimal RateK { get; set; }

        [Column(TypeName = "money")]
        public decimal RateD { get; set; }

        public int Order { get; set; }
    }
}
