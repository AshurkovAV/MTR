namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V004
    {
        public V004()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
            globalEquivalentDatas = new HashSet<globalEquivalentData>();
            shareDoctors = new HashSet<shareDoctor>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDMSP { get; set; }

        [StringLength(100)]
        public string MSPNAME { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }

        public virtual ICollection<globalEquivalentData> globalEquivalentDatas { get; set; }

        public virtual ICollection<shareDoctor> shareDoctors { get; set; }
    }
}
