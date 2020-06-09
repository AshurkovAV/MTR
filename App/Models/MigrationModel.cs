using System;
using Core.Extensions;

namespace Medical.AppLayer.Models
{
    public class MigrationModel
    {
        public MigrationModel()
        {
            
        }

        public MigrationModel(string migrationName)
        {
            RawName = migrationName;
            var index = migrationName.LastIndexOf('_');
            if (index != -1)
            {
                Name = migrationName.Substring(index + 1);
                Date = migrationName.Substring(0, 8).ToDateTimeExact("yyyyMMdd");
            }
            
        }

        public string RawName { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public bool Applied { get; set; }
    }
}
