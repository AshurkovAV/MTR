using System.Collections;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IAppShareSettings
    {
        IEnumerable<string> Profiles { get; }
        string DefaultProfileName { get; set; }
        string DefaultFileName { get; set; }
        bool Save();
        bool Load();
        object Get(string section);
        bool Has(string section);
        Dictionary<string, object> GetProfileAll();
        Dictionary<string, Dictionary<string, object>> GetAll();
        void Put(string section, object cfg);
    }


}
