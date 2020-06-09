namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F004
    {
        public F004()
        {
            subExpertAcademicDegrees = new HashSet<subExpertAcademicDegree>();
            subExpertCertificates = new HashSet<subExpertCertificate>();
            subQualificationCategories = new HashSet<subQualificationCategory>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(7)]
        public string code { get; set; }

        [Required]
        [StringLength(40)]
        public string fname { get; set; }

        [Required]
        [StringLength(40)]
        public string surname { get; set; }

        [Required]
        [StringLength(40)]
        public string patronymic { get; set; }

        public DateTime? birth_date { get; set; }

        [Required]
        [StringLength(14)]
        public string SNILS { get; set; }

        [Required]
        [StringLength(40)]
        public string phone1 { get; set; }

        [StringLength(40)]
        public string phone2 { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [Required]
        [StringLength(13)]
        public string organization_code { get; set; }

        [StringLength(254)]
        public string speciality { get; set; }

        [Required]
        [StringLength(254)]
        public string employment_place { get; set; }

        [Required]
        [StringLength(254)]
        public string position { get; set; }

        public int? experience { get; set; }

        public DateTime date_begin { get; set; }

        public DateTime? date_end { get; set; }

        public DateTime? date_edit { get; set; }

        [StringLength(6)]
        public string exclusion_reason { get; set; }

        public virtual ICollection<subExpertAcademicDegree> subExpertAcademicDegrees { get; set; }

        public virtual ICollection<subExpertCertificate> subExpertCertificates { get; set; }

        public virtual ICollection<subQualificationCategory> subQualificationCategories { get; set; }
    }
}
