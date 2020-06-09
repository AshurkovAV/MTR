namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicRefuseDetail")]
    public partial class FactEconomicRefuseDetail
    {
        [Key]
        public int EconomicRefuseDetailId { get; set; }

        public int EconomicRefuseId { get; set; }

        public int AssistanceConditionsId { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountRefuse { get; set; }

        public virtual FactEconomicRefuse FactEconomicRefuse { get; set; }

        public virtual V006 V006 { get; set; }
    }
}
