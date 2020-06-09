namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class R002
    {
        public R002()
        {
            FactInsuredRegisters = new HashSet<FactInsuredRegister>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(1)]
        public string Kod { get; set; }

        [Required]
        [StringLength(250)]
        public string Opis { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactInsuredRegister> FactInsuredRegisters { get; set; }
    }
}
