namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalVersion")]
    public partial class globalVersion
    {
        public globalVersion()
        {
            FactExchanges = new HashSet<FactExchange>();
            FactExpertCriterions = new HashSet<FactExpertCriterion>();
            FactExpertCriterion_copy = new HashSet<FactExpertCriterion_copy>();
            FactTerritoryAccounts = new HashSet<FactTerritoryAccount>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VersionID { get; set; }

        [StringLength(10)]
        public string Version { get; set; }

        public virtual ICollection<FactExchange> FactExchanges { get; set; }

        public virtual ICollection<FactExpertCriterion> FactExpertCriterions { get; set; }

        public virtual ICollection<FactExpertCriterion_copy> FactExpertCriterion_copy { get; set; }

        public virtual ICollection<FactTerritoryAccount> FactTerritoryAccounts { get; set; }
    }
}
