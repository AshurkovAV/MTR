namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FactMedicalService
    {
        [Key]
        public int MedicalServicesId { get; set; }

        public int MedicalEventId { get; set; }

        public int? MedicalOrganization { get; set; }

        public int? Departament { get; set; }

        public int? Unit { get; set; }

        public int? Profile { get; set; }

        public bool? IsChildren { get; set; }

        public DateTime? ServiceBegin { get; set; }

        public DateTime? ServiceEnd { get; set; }

        public int? Diagnosis { get; set; }

        [StringLength(20)]
        public string ServiceCode { get; set; }

        [Column(TypeName = "money")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public int? SpecialityCode { get; set; }

        [StringLength(14)]
        public string DoctorId { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public int? ExternalId { get; set; }

        [StringLength(255)]
        public string ServiceName { get; set; }

        [Column(TypeName = "money")]
        public decimal? Rate { get; set; }

        public int? MedicalExternalId { get; set; }

        [StringLength(36)]
        public string ExternalGUID { get; set; }

        [StringLength(15)]
        public string SurgeryType { get; set; }

        [StringLength(6)]
        public string MedicalOrganizationCode { get; set; }

        [StringLength(14)]
        public string MedicalDepartmentCode { get; set; }

        [StringLength(3)]
        public string Subdivision { get; set; }

        public int? SpecialityCodeV015 { get; set; }

        public bool? IsServiceBeforeEvent { get; set; }
        public bool? IsServiceRefuse { get; set; }

        public long? Flag { get; set; }

        //by Ira
        public int? IDKSG { get; set; }

        public virtual FactMedicalEvent FactMedicalEvent { get; set; }
    }
}
