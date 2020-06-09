namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalParticularSign")]
    public partial class globalParticularSign
    {
        public globalParticularSign()
        {
            localParticularSignCompositions = new HashSet<localParticularSignComposition>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ParticularSignId { get; set; }

        [StringLength(50)]
        public string ParticularSignName { get; set; }

        public virtual ICollection<localParticularSignComposition> localParticularSignCompositions { get; set; }
    }
}
