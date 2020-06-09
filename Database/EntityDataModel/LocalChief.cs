namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LocalChief")]
    public partial class LocalChief
    {
        public int LocalChiefId { get; set; }

        [StringLength(254)]
        public string SurnameN { get; set; }

        [StringLength(254)]
        public string NameN { get; set; }

        [StringLength(254)]
        public string PatronymicN { get; set; }

        [StringLength(254)]
        public string PositionN { get; set; }

        [StringLength(254)]
        public string SurnameG { get; set; }

        [StringLength(254)]
        public string NameG { get; set; }

        [StringLength(254)]
        public string PatronymicG { get; set; }

        [StringLength(254)]
        public string PositionG { get; set; }

        [StringLength(254)]
        public string SurnameD { get; set; }

        [StringLength(254)]
        public string NameD { get; set; }

        [StringLength(254)]
        public string PatronymicD { get; set; }

        [StringLength(254)]
        public string PositionD { get; set; }

        public int? Type { get; set; }

        public bool? IsDefault { get; set; }
    }
}
