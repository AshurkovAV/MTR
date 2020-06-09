namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalDoctorParticularSign")]
    public partial class globalDoctorParticularSign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DoctorParticularSignId { get; set; }

        [Required]
        [StringLength(50)]
        public string DoctorParticularSignName { get; set; }
    }
}
