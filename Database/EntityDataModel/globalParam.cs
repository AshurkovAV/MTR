namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalParam")]
    public partial class globalParam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ParamID { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(20)]
        public string ReportName { get; set; }
    }
}
