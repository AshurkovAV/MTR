namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TerritoryAccountView")]
    public partial class TerritoryAccountView
    {
        public DateTime? Date { get; set; }

        [StringLength(15)]
        public string AccountNumber { get; set; }

        public DateTime? AccountDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        public decimal? MECPenalties { get; set; }

        [Column(TypeName = "money")]
        public decimal? MEEPenalties { get; set; }

        [Column(TypeName = "money")]
        public decimal? EQMAPenalties { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        [StringLength(6)]
        public string Source { get; set; }

        [StringLength(6)]
        public string Destination { get; set; }

        public int? Status { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TerritoryAccountId { get; set; }

        public int? ExternalId { get; set; }

        public int? PacketNumber { get; set; }

        public int? Type { get; set; }

        public int? Direction { get; set; }

        public int? Parent { get; set; }

        public int? Generation { get; set; }

        [Column(TypeName = "money")]
        public decimal? TotalPaymentAmount { get; set; }

        public int? Version { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(254)]
        public string SourceName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(254)]
        public string DestinationName { get; set; }

        public DateTime? EconomicDate { get; set; }
    }
}
