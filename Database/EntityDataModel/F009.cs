namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F009
    {
        public F009()
        {
            FactInsuredRegisters = new HashSet<FactInsuredRegister>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int IDStatus { get; set; }

        [Required]
        [StringLength(254)]
        public string StatusName { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public virtual ICollection<FactInsuredRegister> FactInsuredRegisters { get; set; }
    }
}
