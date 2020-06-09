using System.ComponentModel.DataAnnotations;

namespace Medical.DatabaseCore.EntityDataModel
{
    public class localSettings
    {
        [Key]
        public int SettingsID { get; set; }
        [MaxLength(200)]
        public string Key { get; set; }
        public string Value { get; set; }
        public string Metadata { get; set; }
    }
}
