namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F002
    {
        public F002()
        {
            FactInsuredRegisters = new HashSet<FactInsuredRegister>();
            FactMedicalAccounts = new HashSet<FactMedicalAccount>();
            FactPatients = new HashSet<FactPatient>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string smocod { get; set; }

        [Required]
        [StringLength(5)]
        public string tf_okato { get; set; }

        [Required]
        [StringLength(254)]
        public string nam_smop { get; set; }

        [Required]
        [StringLength(250)]
        public string nam_smok { get; set; }

        [Required]
        [StringLength(12)]
        public string inn { get; set; }

        [Required]
        [StringLength(15)]
        public string Ogrn { get; set; }

        [Required]
        [StringLength(9)]
        public string KPP { get; set; }

        [StringLength(6)]
        public string index_j { get; set; }

        [Required]
        [StringLength(254)]
        public string addr_j { get; set; }

        [StringLength(6)]
        public string index_f { get; set; }

        [Required]
        [StringLength(254)]
        public string addr_f { get; set; }

        [StringLength(6)]
        public string okopf { get; set; }

        [Required]
        [StringLength(40)]
        public string fam_ruk { get; set; }

        [Required]
        [StringLength(40)]
        public string im_ruk { get; set; }

        [Required]
        [StringLength(40)]
        public string ot_ruk { get; set; }

        [Required]
        [StringLength(40)]
        public string phone { get; set; }

        [Required]
        [StringLength(40)]
        public string fax { get; set; }

        [StringLength(40)]
        public string e_mail { get; set; }

        [StringLength(100)]
        public string www { get; set; }

        [Column(TypeName = "numeric")]
        public decimal org { get; set; }

        public DateTime? d_begin { get; set; }

        public DateTime? d_end { get; set; }

        [StringLength(15)]
        public string name_e { get; set; }

        [StringLength(1)]
        public string Nal_p { get; set; }

        public DateTime? DUVED { get; set; }

        [Column(TypeName = "numeric")]
        public decimal kol_zl { get; set; }

        public DateTime? d_edit { get; set; }

        [Required]
        [StringLength(20)]
        public string n_doc { get; set; }

        public DateTime? d_start { get; set; }

        public DateTime? data_e { get; set; }

        [StringLength(254)]
        public string year_work { get; set; }

        public virtual ICollection<FactInsuredRegister> FactInsuredRegisters { get; set; }

        public virtual ICollection<FactMedicalAccount> FactMedicalAccounts { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }
    }
}
