namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicPaymentDetail")]
    public partial class FactEconomicPaymentDetail
    {
        [Key]
        public int EconomicPaymentDetailId { get; set; }

        public int AccountId { get; set; }

        public int AssistanceConditionsId { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountPayable { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountFact { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountDebt { get; set; }

        public virtual FactTerritoryAccount FactTerritoryAccount { get; set; }

        public virtual V006 V006 { get; set; }
    }
}
