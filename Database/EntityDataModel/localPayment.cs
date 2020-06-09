namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localPayment")]
    public partial class localPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PaymentId { get; set; }

        public int? Code { get; set; }

        public int? Type { get; set; }

        [StringLength(254)]
        public string Name { get; set; }

        [StringLength(12)]
        public string Inn { get; set; }

        [StringLength(15)]
        public string Ogrn { get; set; }

        [StringLength(9)]
        public string KPP { get; set; }

        [StringLength(50)]
        public string Account { get; set; }

        [StringLength(50)]
        public string CorrespondentAccount { get; set; }

        [StringLength(50)]
        public string BIK { get; set; }

        [StringLength(254)]
        public string BankName { get; set; }

        [StringLength(50)]
        public string AgreementNumber { get; set; }

        public DateTime? AgreementDate { get; set; }

        [StringLength(254)]
        public string Director { get; set; }

        [StringLength(254)]
        public string Accounter { get; set; }

        [StringLength(254)]
        public string Address { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        [StringLength(11)]
        public string OKATO { get; set; }

        [StringLength(5)]
        public string OKONH { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }
    }
}
