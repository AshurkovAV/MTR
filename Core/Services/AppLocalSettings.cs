using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Core.Services
{
    /// <summary>
    /// Хранение настроек на хосте пользователя
    /// </summary>
    public class AppLocalSettings : IAppLocalSettings
    {
        private const string _defaultFileName = "medicine.cfg";
        private Dictionary<string, object> _storage;
        public string DefaultFileName { get; set; }
        public string StoragePath {
            get { return Path.Combine(GlobalConfig.SettingsFolder, _defaultFileName); }
        }

        private string _defaultSettings = @"{
            'user':{
                'username':'admin',
                'password':''
            }
        }";

        public AppLocalSettings()
        {
            Load();
        }

        public bool Save()
        {
            try
            {
                File.WriteAllText(StoragePath, JsonConvert.SerializeObject(_storage, Formatting.Indented));
                Load();
                return true;
            }
            catch (Exception)
            {
                
            }
            return false;
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(StoragePath))
                {
                    _storage = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(StoragePath));
                }
                else
                {
                    _storage = JsonConvert.DeserializeObject<Dictionary<string, object>>(_defaultSettings);
                    Save();
                }

                return true;
            }
            catch (Exception)
            {
                //TODO add error
            }
            return false;
        }

        public object Get(string section)
        {
            if (_storage.ContainsKey(section))
            {
                return _storage[section];
            }
            return null;
        }

        public bool Has(string section)
        {
            return _storage.ContainsKey(section);
        }

        public Dictionary<string, object> GetAll()
        {
            return _storage;
        }

        public void Put(string section, object cfg)
        {
            if (_storage.ContainsKey(section))
            {
                _storage.Remove(section);
            }

            _storage.Add(section,cfg);
            Save();
        }

    }
}
