namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("globalClinicalExamination")]
    public partial class globalClinicalExamination
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClinicalExaminationId { get; set; }

        public string ServiceCode { get; set; }

        public int ProfileId { get; set; }

        public int SpecialityId { get; set; }

        public int Age { get; set; }

        public int AgeMonths { get; set; }

        public int Sex { get; set; }

        public int Type { get; set; }

        public bool? IsChildren { get; set; }

        public int Quantity { get; set; }
    }
}
