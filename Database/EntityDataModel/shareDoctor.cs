namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareDoctor")]
    public partial class shareDoctor
    {
        public int? MedicalOrganizationId { get; set; }

        [StringLength(14)]
        public string Code { get; set; }

        [StringLength(40)]
        public string DName { get; set; }

        [StringLength(40)]
        public string Surname { get; set; }

        [StringLength(40)]
        public string Patronymic { get; set; }

        public int? SpecialityId { get; set; }

        public int? MedicalDepartmentId { get; set; }

        public int? PersonalNumber { get; set; }

        public int? AssistanceConditionsId { get; set; }

        public int? DoctorParticularSign { get; set; }

        [StringLength(5)]
        public string Room { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(6)]
        public string MedicalOrganizationCode { get; set; }

        [Key]
        public int DoctorId { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual shareMedicalDepartment shareMedicalDepartment { get; set; }

        public virtual V004 V004 { get; set; }
    }
}
