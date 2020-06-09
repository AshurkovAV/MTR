namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localParticularSignComposition")]
    public partial class localParticularSignComposition
    {
        public int Id { get; set; }

        public int RateId { get; set; }

        public int ParticularSignId { get; set; }

        public virtual globalParticularSign globalParticularSign { get; set; }

        public virtual shareRate shareRate { get; set; }
    }
}
