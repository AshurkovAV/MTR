namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V009
    {
        public V009()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDRMP { get; set; }

        [StringLength(254)]
        public string RMPNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DL_USLOV { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }
    }
}
