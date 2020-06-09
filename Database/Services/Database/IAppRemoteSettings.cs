using System.Collections.Generic;

namespace Medical.DatabaseCore.Services.Database
{
    public interface IAppRemoteSettings
    {
        /// <summary>
        /// Получение всех ключей конфигураций
        /// </summary>
        /// <returns>Список ключей</returns>
        IEnumerable<string> GetAllKeys();
        bool Save(string key, object value);
        bool SaveAll();
        bool Load();
        object Get(string section);
        object GetMetadata(string section);
        bool Has(string section);
        Dictionary<string, object> GetAll();
        Dictionary<string, object> GetAllMetadata();
        void Put(string section, object cfg);
    }


}
