namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class F001
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(2)]
        public string tf_code { get; set; }

        [StringLength(5)]
        public string tf_okato { get; set; }

        [Required]
        [StringLength(15)]
        public string tf_ogrn { get; set; }

        [Required]
        [StringLength(250)]
        public string name_tfp { get; set; }

        [Required]
        [StringLength(250)]
        public string name_tfk { get; set; }

        [StringLength(6)]
        public string post_index { get; set; }

        [Required]
        [StringLength(250)]
        public string address { get; set; }

        [Required]
        [StringLength(40)]
        public string fam_dir { get; set; }

        [Required]
        [StringLength(40)]
        public string im_dir { get; set; }

        [Required]
        [StringLength(40)]
        public string ot_dir { get; set; }

        [Required]
        [StringLength(40)]
        public string phone { get; set; }

        [Required]
        [StringLength(40)]
        public string fax { get; set; }

        [Required]
        [StringLength(50)]
        public string e_mail { get; set; }

        [Column(TypeName = "numeric")]
        public decimal kf_tf { get; set; }

        [StringLength(100)]
        public string www { get; set; }

        public DateTime d_edit { get; set; }

        public DateTime? d_end { get; set; }
    }
}
