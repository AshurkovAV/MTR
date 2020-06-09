namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V008
    {
        public V008()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDVMP { get; set; }

        [StringLength(254)]
        public string VMPNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }
    }
}
