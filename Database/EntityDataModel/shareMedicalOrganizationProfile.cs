namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareMedicalOrganizationProfile")]
    public partial class shareMedicalOrganizationProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MOProfileId { get; set; }

        public int? V002ProfileId { get; set; }

        public int? MedicalOrganizationId { get; set; }

        public int? AssistanceConditionsId { get; set; }

        public bool? Children { get; set; }

        public bool? Adult { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public virtual V006 V006 { get; set; }

        public virtual V002 V002 { get; set; }
    }
}
