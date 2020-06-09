namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class V005
    {
        public V005()
        {
            FactPersons = new HashSet<FactPerson>();
            FactPersons1 = new HashSet<FactPerson>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDPOL { get; set; }

        [StringLength(7)]
        public string POLNAME { get; set; }

        public virtual ICollection<FactPerson> FactPersons { get; set; }

        public virtual ICollection<FactPerson> FactPersons1 { get; set; }
    }
}
