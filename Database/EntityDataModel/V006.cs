namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V006
    {
        public V006()
        {
            FactEconomicPayments = new HashSet<FactEconomicPayment>();
            FactEconomicPaymentDetails = new HashSet<FactEconomicPaymentDetail>();
            FactEconomicRefuseDetails = new HashSet<FactEconomicRefuseDetail>();
            FactEconomicSurchargeDetails = new HashSet<FactEconomicSurchargeDetail>();
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
            shareMedicalOrganizationProfiles = new HashSet<shareMedicalOrganizationProfile>();
            shareMedicalSubdivisions = new HashSet<shareMedicalSubdivision>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDUMP { get; set; }

        [StringLength(254)]
        public string UMPNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactEconomicPayment> FactEconomicPayments { get; set; }

        public virtual ICollection<FactEconomicPaymentDetail> FactEconomicPaymentDetails { get; set; }

        public virtual ICollection<FactEconomicRefuseDetail> FactEconomicRefuseDetails { get; set; }

        public virtual ICollection<FactEconomicSurchargeDetail> FactEconomicSurchargeDetails { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }

        public virtual ICollection<shareMedicalOrganizationProfile> shareMedicalOrganizationProfiles { get; set; }

        public virtual ICollection<shareMedicalSubdivision> shareMedicalSubdivisions { get; set; }
    }
}
