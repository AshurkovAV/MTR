namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalRefusalPenalty")]
    public partial class globalRefusalPenalty
    {
        [Key]
        public int RefusalPenaltyId { get; set; }

        [StringLength(20)]
        public string Reason { get; set; }

        [Required]
        public int Percent { get; set; }
        
        [Required]
        public decimal Penalty { get; set; }
        
        [Required]
        public decimal Decrease { get; set; }

      
        [StringLength(1000)]
        public string Comments { get; set; }
    }
}
