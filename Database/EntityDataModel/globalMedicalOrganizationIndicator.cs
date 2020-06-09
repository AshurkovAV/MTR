namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalMedicalOrganizationIndicator")]
    public partial class globalMedicalOrganizationIndicator
    {
        public globalMedicalOrganizationIndicator()
        {
            globalMedicalAssistanceVolumes = new HashSet<globalMedicalAssistanceVolume>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MedicalOrganizationTypeID { get; set; }

        public int Code { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public virtual ICollection<globalMedicalAssistanceVolume> globalMedicalAssistanceVolumes { get; set; }
    }
}
