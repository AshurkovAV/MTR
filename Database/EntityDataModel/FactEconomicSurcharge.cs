namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicSurcharge")]
    public partial class FactEconomicSurcharge
    {
        public FactEconomicSurcharge()
        {
            FactEconomicSurchargeDetails = new HashSet<FactEconomicSurchargeDetail>();
        }

        [Key]
        public int EconomicSurchargeId { get; set; }

        public DateTime SurchargeDate { get; set; }

        [Column(TypeName = "money")]
        public decimal SurchargeTotalAmount { get; set; }

        public int AccountId { get; set; }

        public virtual ICollection<FactEconomicSurchargeDetail> FactEconomicSurchargeDetails { get; set; }

        public virtual FactTerritoryAccount FactTerritoryAccount { get; set; }
    }
}
