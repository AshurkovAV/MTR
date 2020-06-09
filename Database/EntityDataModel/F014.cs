namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F014
    {
        public F014()
        {
            FactEQMAs = new HashSet<FactEQMA>();
            FactMEEs = new HashSet<FactMEE>();
            FactExternalRefuses = new HashSet<FactExternalRefuse>();
            FactMECs = new HashSet<FactMEC>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(4)]
        public string Kod { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IDVID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Naim { get; set; }

        [Required]
        [StringLength(150)]
        public string Osn { get; set; }

        [StringLength(20)]
        public string KodPG { get; set; }

        [StringLength(150)]
        public string Comments { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactEQMA> FactEQMAs { get; set; }

        public virtual ICollection<FactMEE> FactMEEs { get; set; }

        public virtual ICollection<FactExternalRefuse> FactExternalRefuses { get; set; }

        public virtual ICollection<FactMEC> FactMECs { get; set; }
    }
}
