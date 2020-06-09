namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V002
    {
        public V002()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
            globalEquivalentDatas = new HashSet<globalEquivalentData>();
            globalMedicalAssistanceVolumes = new HashSet<globalMedicalAssistanceVolume>();
            shareMedicalOrganizationProfiles = new HashSet<shareMedicalOrganizationProfile>();
            shareRates = new HashSet<shareRate>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDPR { get; set; }

        [StringLength(254)]
        public string PRNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }

        public virtual ICollection<globalEquivalentData> globalEquivalentDatas { get; set; }

        public virtual ICollection<globalMedicalAssistanceVolume> globalMedicalAssistanceVolumes { get; set; }

        public virtual ICollection<shareMedicalOrganizationProfile> shareMedicalOrganizationProfiles { get; set; }

        public virtual ICollection<shareRate> shareRates { get; set; }
    }
}
