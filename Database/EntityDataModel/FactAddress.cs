namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactAddress")]
    public partial class FactAddress
    {
        [Key]
        public int AddressId { get; set; }

        public int? Okato { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(6)]
        public string PostIndex { get; set; }

        [StringLength(80)]
        public string RegionName { get; set; }

        [StringLength(80)]
        public string PlaceName { get; set; }

        [StringLength(80)]
        public string Street { get; set; }

        [StringLength(7)]
        public string House { get; set; }

        [StringLength(6)]
        public string Building { get; set; }

        [StringLength(6)]
        public string Flat { get; set; }

        public int? RegionFederation { get; set; }

        public virtual F010 F010 { get; set; }

        public virtual O002 O002 { get; set; }
    }
}
