namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalLicense")]
    public partial class globalLicense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LicenseId { get; set; }

        [StringLength(6)]
        public string MedicalOrganization{ get; set; }

        public DateTime? DateBegin{ get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateStop { get; set; }

        [StringLength(255)]
        public string LicenseNumber { get; set; }
    }
}
