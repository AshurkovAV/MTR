using System.Collections.Generic;

namespace Core.Services
{
    public interface IAppLocalSettings
    {
        string DefaultFileName { get; set; }
        bool Save();
        bool Load();
        object Get(string section);
        bool Has(string section);
        Dictionary<string, object> GetAll();
        void Put(string section, object cfg);
    }


}
