namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalLicenseEntry")]
    public partial class globalLicenseEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LicenseEntryId { get; set; }

        public globalLicense License { get; set; }

        public virtual V002 Profile { get; set; }

        public virtual V006 AssistanceCondition { get; set; }

        public virtual V008 AssistanceType { get; set; }
    }
}
