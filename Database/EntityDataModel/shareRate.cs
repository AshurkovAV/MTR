namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("shareRate")]
    public partial class shareRate
    {
        public shareRate()
        {
            localParticularSignCompositions = new HashSet<localParticularSignComposition>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RateId { get; set; }

        public int? ProfileId { get; set; }

        public int? IDC10GroupId { get; set; }

        public int? UnitId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        public int? AssistanceConditionsId { get; set; }

        [StringLength(50)]
        public string ParticularSign { get; set; }

        public int? SubdivisionId { get; set; }

        public int? MedicalOrganizationId { get; set; }

        public bool? Children { get; set; }

        public bool? Adult { get; set; }

        public virtual localIDC10Group localIDC10Group { get; set; }

        public virtual ICollection<localParticularSignComposition> localParticularSignCompositions { get; set; }

        public virtual V002 V002 { get; set; }

        public virtual V010 V010 { get; set; }
    }
}
