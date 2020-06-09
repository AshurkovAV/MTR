using System.ComponentModel.DataAnnotations;

namespace Medical.DatabaseCore.EntityDataModel
{
    public class localUserSettings
    {
        [Key]
        public int UserSettingsID { get; set; }
        [MaxLength(200)]
        public string Key { get; set; }
        public string Value { get; set; }
        public string Metadata { get; set; }
        public int UserID { get; set; } 
        public localUser User { get; set; } 
    }
}
