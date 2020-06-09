namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class globalIDC10Modernization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDC10ModernizationId { get; set; }

        public int? IDC10Id { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? LocalPrice { get; set; }

        [Column(TypeName = "money")]
        public decimal? FederalPrice { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        [StringLength(10)]
        public string IDC10Name { get; set; }

        public virtual M001 M001 { get; set; }
    }
}
