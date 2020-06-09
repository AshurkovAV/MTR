namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F008
    {
        public F008()
        {
            FactInsuredRegisters = new HashSet<FactInsuredRegister>();
            FactPatients = new HashSet<FactPatient>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IDDOC { get; set; }

        [Required]
        [StringLength(254)]
        public string DOCNAME { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactInsuredRegister> FactInsuredRegisters { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }
    }
}
