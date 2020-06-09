namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicRefuse")]
    public partial class FactEconomicRefuse
    {
        public FactEconomicRefuse()
        {
            FactEconomicRefuseDetails = new HashSet<FactEconomicRefuseDetail>();
        }

        [Key]
        public int EconomicRefuseId { get; set; }

        public DateTime RefuseDate { get; set; }

        [Column(TypeName = "money")]
        public decimal RefuseTotalAmount { get; set; }

        public int AccountId { get; set; }

        public virtual ICollection<FactEconomicRefuseDetail> FactEconomicRefuseDetails { get; set; }

        public virtual FactTerritoryAccount FactTerritoryAccount { get; set; }
    }
}
