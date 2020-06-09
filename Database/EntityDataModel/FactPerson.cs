namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactPerson")]
    public partial class FactPerson
    {
        public FactPerson()
        {
            FactDocuments = new HashSet<FactDocument>();
            FactPatients = new HashSet<FactPatient>();
        }

        [Key]
        public int PersonId { get; set; }

        [StringLength(40)]
        public string PName { get; set; }

        [StringLength(40)]
        public string Surname { get; set; }

        [StringLength(40)]
        public string Patronymic { get; set; }

        public int? Sex { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(100)]
        public string BirthPlace { get; set; }

        public int? AddressReg { get; set; }

        public int? AddressLive { get; set; }

        [StringLength(14)]
        public string SNILS { get; set; }

        [StringLength(40)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(40)]
        public string RepresentativeName { get; set; }

        [StringLength(40)]
        public string RepresentativeSurname { get; set; }

        [StringLength(40)]
        public string RepresentativePatronymic { get; set; }

        public int? RepresentativeSex { get; set; }

        public DateTime? RepresentativeBirthday { get; set; }

        [StringLength(200)]
        public string RepresentativeContacts { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [StringLength(24)]
        public string Reliability { get; set; }

        [StringLength(24)]
        public string ReliabilityRepresentative { get; set; }

        public virtual ICollection<FactDocument> FactDocuments { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }

        public virtual V005 V005 { get; set; }

        public virtual V005 V0051 { get; set; }
    }
}
