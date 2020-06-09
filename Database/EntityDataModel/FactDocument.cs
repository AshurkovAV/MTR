namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactDocument")]
    public partial class FactDocument
    {
        public FactDocument()
        {
            FactPatients = new HashSet<FactPatient>();
        }

        [Key]
        public int DocumentId { get; set; }

        public int PersonId { get; set; }

        public int? DocType { get; set; }

        [StringLength(10)]
        public string DocSeries { get; set; }

        [StringLength(20)]
        public string DocNum { get; set; }

        public DateTime? DocDate { get; set; }

        [StringLength(80)]
        public string IssueName { get; set; }

        public virtual F011 F011 { get; set; }

        public virtual FactPerson FactPerson { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }
    }
}
