namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalAccountType")]
    public partial class globalAccountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountTypeId { get; set; }

        public int? TypeCode { get; set; }

        [StringLength(40)]
        public string TypeName { get; set; }
    }
}
