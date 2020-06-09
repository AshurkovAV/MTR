using System.Collections.Generic;

namespace Medical.AppLayer.Services
{
    public interface IUserRemoteSettings
    {
        bool Save();
        bool Load();
        object Get(string section);
        bool Has(string section);
        Dictionary<string, object> GetAll();
        void Put(string section, object cfg);
    }


}
