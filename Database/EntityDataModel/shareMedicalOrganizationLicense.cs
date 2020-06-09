namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareMedicalOrganizationLicense")]
    public partial class shareMedicalOrganizationLicense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MOLicenseId { get; set; }

        public int? V003WorkId { get; set; }

        public int? MedicalOrganizationId { get; set; }

        public int? AssistanceConditionsId { get; set; }

        public bool? Children { get; set; }

        public bool? Adult { get; set; }

        [StringLength(250)]
        public string License { get; set; }

        public DateTime? LicenseDateBegin { get; set; }

        public DateTime? LicenseDateEnd { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }
    }
}
