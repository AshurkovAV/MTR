namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class O002
    {
        public O002()
        {
            FactAddresses = new HashSet<FactAddress>();
            FactInsuredRegisters = new HashSet<FactInsuredRegister>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(2)]
        public string TER { get; set; }

        [Required]
        [StringLength(3)]
        public string KOD1 { get; set; }

        [Required]
        [StringLength(3)]
        public string KOD2 { get; set; }

        [Required]
        [StringLength(3)]
        public string KOD3 { get; set; }

        [StringLength(1)]
        public string RAZDEL { get; set; }

        [StringLength(250)]
        public string NAME1 { get; set; }

        [StringLength(80)]
        public string CENTRUM { get; set; }

        [StringLength(250)]
        public string NOMDESCR { get; set; }

        [StringLength(3)]
        public string NOMAKT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? STATUS { get; set; }

        public DateTime? DATA_UPD { get; set; }

        public virtual ICollection<FactAddress> FactAddresses { get; set; }

        public virtual ICollection<FactInsuredRegister> FactInsuredRegisters { get; set; }
    }
}
