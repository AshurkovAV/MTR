namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicDebt")]
    public partial class FactEconomicDebt
    {
        [Key]
        public int EconomicDebtId { get; set; }

        [Required]
        [StringLength(6)]
        public string Territory { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "money")]
        public decimal TerritoryAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal OwnAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal TerritoryAmount25 { get; set; }

        [Column(TypeName = "money")]
        public decimal OwnAmount25 { get; set; }
    }
}
