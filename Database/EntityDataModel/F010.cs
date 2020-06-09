namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F010
    {
        public F010()
        {
            FactAddresses = new HashSet<FactAddress>();
            FactPatients = new HashSet<FactPatient>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(2)]
        public string KOD_TF { get; set; }

        [Required]
        [StringLength(5)]
        public string KOD_OKATO { get; set; }

        [Required]
        [StringLength(254)]
        public string SUBNAME { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? OKRUG { get; set; }

        public virtual ICollection<FactAddress> FactAddresses { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }
    }
}
