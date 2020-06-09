using System;

namespace Medical.CoreLayer.Models
{
    public class XmlExportModel
    {
        public string Version { get; set; }
        public bool IsRegister { get; set; }
        public bool IsAccount { get; set; }
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? Direction { get; set; }
        public int? Destination { get; set; }
        public string Target { get; set; }
        public string TargetDirectory { get; set; }
    }
}
