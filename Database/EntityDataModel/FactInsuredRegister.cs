namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactInsuredRegister")]
    public partial class FactInsuredRegister
    {
        [Key]
        public int InsuredRegisterId { get; set; }

        public int? Territory { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? INP { get; set; }

        public DateTime? INPDate { get; set; }

        public int? Person { get; set; }

        public int? Okato { get; set; }

        public int? Status { get; set; }

        public int? InsuranceNumber { get; set; }

        public DateTime? InsuranceDate { get; set; }

        public int? ApplicationMethod { get; set; }

        [StringLength(1)]
        public string ApplicationExists { get; set; }

        public DateTime? InsuranceChangeDate { get; set; }

        public int? ChangeReason { get; set; }

        public int? InsuranceDocType { get; set; }

        [StringLength(10)]
        public string InsuranceDocSeries { get; set; }

        [StringLength(20)]
        public string InsuranceDocNumber { get; set; }

        public DateTime? InsuranceDocBegin { get; set; }

        public DateTime? InsuranceDocEnd { get; set; }

        public int? InsuranceDocMethod { get; set; }

        public DateTime? InsuranceDocDate { get; set; }

        public int? InsuranceDocForm { get; set; }

        public int? ReasonInsuranceDoc { get; set; }

        public int? Registration { get; set; }

        public virtual F002 F002 { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual F008 F008 { get; set; }

        public virtual F009 F009 { get; set; }

        public virtual R003 R003 { get; set; }

        public virtual R002 R002 { get; set; }

        public virtual R003 R0031 { get; set; }

        public virtual O002 O002 { get; set; }
    }
}
