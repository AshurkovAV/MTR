namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicSurchargeDetail")]
    public partial class FactEconomicSurchargeDetail
    {
        [Key]
        public int EconomicSurchargeDetailId { get; set; }

        public int EconomicSurchargeId { get; set; }

        public int AssistanceConditionsId { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountSurcharge { get; set; }

        public virtual FactEconomicSurcharge FactEconomicSurcharge { get; set; }

        public virtual V006 V006 { get; set; }
    }
}
