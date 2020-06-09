namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F011
    {
        public F011()
        {
            FactDocuments = new HashSet<FactDocument>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IDDoc { get; set; }

        [Required]
        [StringLength(254)]
        public string DocName { get; set; }

        [StringLength(10)]
        public string DocSer { get; set; }

        [StringLength(20)]
        public string DocNum { get; set; }

        public DateTime DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public virtual ICollection<FactDocument> FactDocuments { get; set; }
    }
}
