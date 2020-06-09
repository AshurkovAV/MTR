namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactEconomicAccount")]
    public partial class FactEconomicAccount
    {
        public FactEconomicAccount()
        {
            FactEconomicPayments = new HashSet<FactEconomicPayment>();
        }

        [Key]
        public int EconomicAccountId { get; set; }

        [StringLength(254)]
        public string PaymentOrder { get; set; }

        public DateTime? PaymentOrderDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? TotalAmount { get; set; }

        public int? PaymentStatus { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        public int AccountId { get; set; }

        public virtual ICollection<FactEconomicPayment> FactEconomicPayments { get; set; }

        public virtual FactTerritoryAccount FactTerritoryAccount { get; set; }
    }
}
