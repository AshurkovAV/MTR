namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicPayment")]
    public partial class FactEconomicPayment
    {
        [Key]
        public int EconomicPaymentId { get; set; }

        public int EconomicAccountId { get; set; }

        public int AssistanceConditionsId { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public virtual FactEconomicAccount FactEconomicAccount { get; set; }

        public virtual V006 V006 { get; set; }
    }
}
