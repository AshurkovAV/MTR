namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subExpertCertificate")]
    public partial class subExpertCertificate
    {
        [Key]
        public int ExpertCertificateId { get; set; }

        public int ExpertId { get; set; }

        [StringLength(254)]
        public string CertificateSpeciality { get; set; }

        public DateTime? CertificateDate { get; set; }

        public virtual F004 F004 { get; set; }
    }
}
