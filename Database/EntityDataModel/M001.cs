namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class M001
    {
        public M001()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
            FactMedicalEvents1 = new HashSet<FactMedicalEvent>();
            FactMedicalEvents2 = new HashSet<FactMedicalEvent>();
            globalIDC10Modernization = new HashSet<globalIDC10Modernization>();
            localIDC10GroupComposition = new HashSet<localIDC10GroupComposition>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(10)]
        public string IDDS { get; set; }

        [StringLength(254)]
        public string DSNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public int? Class { get; set; }

        public int? Section { get; set; }

        public int? Payable { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents1 { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents2 { get; set; }

        public virtual ICollection<globalIDC10Modernization> globalIDC10Modernization { get; set; }

        public virtual ICollection<localIDC10GroupComposition> localIDC10GroupComposition { get; set; }

        public virtual M001Classes M001Classes { get; set; }

        public virtual M001Sections M001Sections { get; set; }
    }
}
