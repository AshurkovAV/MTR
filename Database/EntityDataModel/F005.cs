namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F005
    {
        public F005()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IDIDST { get; set; }

        [Required]
        [StringLength(254)]
        public string STNAME { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }
    }
}
