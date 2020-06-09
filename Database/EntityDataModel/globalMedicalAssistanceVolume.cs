namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalMedicalAssistanceVolume")]
    public partial class globalMedicalAssistanceVolume
    {
        [Key]
        public int MedicalAssistanceVolumeId { get; set; }

        public int MedicalOrganization { get; set; }

        public int Indicator { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public int? Profile { get; set; }

        public int? Volume { get; set; }

        public virtual F003 F003 { get; set; }

        public virtual globalMedicalOrganizationIndicator globalMedicalOrganizationIndicator { get; set; }

        public virtual V002 V002 { get; set; }
    }
}
