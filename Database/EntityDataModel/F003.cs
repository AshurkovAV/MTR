namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F003
    {
        public F003()
        {
            shareDoctors = new HashSet<shareDoctor>();
            shareMedicalDepartments = new HashSet<shareMedicalDepartment>();
            FactInsuredRegisters = new HashSet<FactInsuredRegister>();
            FactMedicalAccounts = new HashSet<FactMedicalAccount>();
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
            globalMedicalAssistanceVolumes = new HashSet<globalMedicalAssistanceVolume>();
            shareMedicalDepartments1 = new HashSet<shareMedicalDepartment>();
            shareOperatingSchedules = new HashSet<shareOperatingSchedule>();
        }

        public int Id { get; set; }

        [StringLength(5)]
        public string tf_okato { get; set; }

        [StringLength(6)]
        public string mcod { get; set; }

        [StringLength(250)]
        public string nam_mop { get; set; }

        [StringLength(250)]
        public string nam_mok { get; set; }

        [StringLength(12)]
        public string inn { get; set; }

        [StringLength(15)]
        public string Ogrn { get; set; }

        [StringLength(9)]
        public string KPP { get; set; }

        [StringLength(6)]
        public string index_j { get; set; }

        [StringLength(254)]
        public string addr_j { get; set; }

        [StringLength(6)]
        public string okopf { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? vedpri { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? org { get; set; }

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

        [Required]
        [StringLength(50)]
        public string e_mail { get; set; }

        [Required]
        [StringLength(30)]
        public string n_doc { get; set; }

        public DateTime? d_start { get; set; }

        public DateTime? data_e { get; set; }

        [StringLength(30)]
        public string mp { get; set; }

        [StringLength(100)]
        public string www { get; set; }

        public DateTime? d_begin { get; set; }

        public DateTime? d_end { get; set; }

        [StringLength(10)]
        public string name_e { get; set; }

        public DateTime? DUVED { get; set; }

        public DateTime d_edit { get; set; }

        public virtual ICollection<shareDoctor> shareDoctors { get; set; }

        public virtual ICollection<shareMedicalDepartment> shareMedicalDepartments { get; set; }

        public virtual ICollection<FactInsuredRegister> FactInsuredRegisters { get; set; }

        public virtual ICollection<FactMedicalAccount> FactMedicalAccounts { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }

        public virtual ICollection<globalMedicalAssistanceVolume> globalMedicalAssistanceVolumes { get; set; }

        public virtual ICollection<shareMedicalDepartment> shareMedicalDepartments1 { get; set; }

        public virtual ICollection<shareOperatingSchedule> shareOperatingSchedules { get; set; }
    }
}
