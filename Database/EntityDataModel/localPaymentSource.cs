namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("localPaymentSource")]
    public partial class localPaymentSource
    {
        public localPaymentSource()
        {
            FactMedicalEvents = new HashSet<FactMedicalEvent>();
        }

        [Key]
        public int PaymentSourceId { get; set; }

        [StringLength(40)]
        public string PaymentSourceName { get; set; }

        public virtual ICollection<FactMedicalEvent> FactMedicalEvents { get; set; }
    }
}
