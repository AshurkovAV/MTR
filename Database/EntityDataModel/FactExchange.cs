namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FactExchange")]
    public partial class FactExchange
    {
        public FactExchange()
        {
            FactPatients = new HashSet<FactPatient>();
        }

        [Key]
        public int ExchangeId { get; set; }

        public DateTime Date { get; set; }

        public int Direction { get; set; }

        public int PacketNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string FileName { get; set; }

        [Required]
        [StringLength(10)]
        public string Source { get; set; }

        [Required]
        [StringLength(10)]
        public string Destination { get; set; }

        public int RecordCounts { get; set; }

        public DateTime ActionDate { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public int Type { get; set; }

        public int AccountId { get; set; }

        public int? Version { get; set; }

        public virtual globalVersion globalVersion { get; set; }

        public virtual ICollection<FactPatient> FactPatients { get; set; }
    }
}
