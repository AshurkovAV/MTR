using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Core.Services
{
    /// <summary>
    /// Хранение настроек на сетевом хосте (общие настройки)
    /// </summary>
    public class AppShareSettings : IAppShareSettings
    {
        private const string _defaultFileName = "medicine.cfg";
        private Dictionary<string, Dictionary<string, object>> _storage;
        private List<string> _profiles;
        public string DefaultFileName { get; set; }
        public IEnumerable<string> Profiles{
            get
            {
                return _profiles;
            } 
        }
        public string DefaultProfileName { get; set; }
        public string StoragePath {
            get { return Path.Combine(GlobalConfig.UserDataFolder, _defaultFileName); }
        }

        private string _defaultSettings =
        @"{
            'development':{
                'default': true,
                'database': {
                    'Name': 'medicine',
                    'DataSource': '77.241.17.98,1412',
                    'Database': 'medicine_ins2018',
                    'Timeout': 15,
                    'Provider': 'Sql',
                    'IsWindowsAuth': true,
                    'ConnectionString' : 'Data Source=77.241.17.98,1412;Initial Catalog=medicine_ins2018;Application Name=Medicine;User Id=sa; Password=nfyjnjvjh;Connect Timeout=15;'
                }
            },
            'production':{
                'default': false,
                'database': {
                    'Name': 'medicine',
                    'DataSource': '.\\SQLEXPRESS',
                    'Database': 'medicine_ins',
                    'Timeout': 30,
                    'Provider': 'Sql',
                    'IsWindowsAuth': true,
                    'ConnectionString' : 'Data Source=.\\SQLEXPRESS;Initial Catalog=medicine_ins;Application Name=Medicine;Integrated Security=True;Connect Timeout=30;'
                }
            },
            'tfoms-test': {
                'database': {
                    'Name': 'medicine',
                    'DataSource': 'tfoms-2012',
                    'Database': 'medicine_ins_new',
                    'Timeout': 30,
                    'Provider': 'Sql',
                    'IsWindowsAuth': false,
	            'Username' : 'sa',
	            'Password' : 'nfyjnjvjh',
                    'ConnectionString': 'Data Source=tfoms-2012;Initial Catalog=medicine_ins_new;Application Name=Medicine;User Id=sa; Password=nfyjnjvjh;Connect Timeout=30;'
                }
            },
            'tfoms': {
                'database': {
                    'Name': 'medicine',
                    'DataSource': 'tfoms-2012',
                    'Database': 'medicine_ins',
                    'Timeout': 30,
                    'Provider': 'Sql',
                    'IsWindowsAuth': false,
	            'Username' : 'sa',
	            'Password' : 'nfyjnjvjh',
                    'ConnectionString': 'Data Source=tfoms-2012;Initial Catalog=medicine_ins;Application Name=Medicine;User Id=sa; Password=nfyjnjvjh;Connect Timeout=30;'
                }
            }
            
        }";

    


        public AppShareSettings()
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
                //TODO add error
            }
            return false;
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(StoragePath))
                {
                    _storage = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(StoragePath));
                }
                else
                {
                    _storage = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(_defaultSettings);
                    Save();
                }

                _profiles = _storage.Select(p => p.Key).ToList();
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
            if (_storage[DefaultProfileName].ContainsKey(section))
            {
                return _storage[DefaultProfileName][section];
            }
            return null;
        }

        public bool Has(string section)
        {
            return _storage[DefaultProfileName].ContainsKey(section);
        }

        public Dictionary<string, object> GetProfileAll()
        {
            return _storage[DefaultProfileName];
        }

        public Dictionary<string, Dictionary<string, object>> GetAll()
        {
            return _storage;
        }

        public void Put(string section, object cfg)
        {
            if (_storage[DefaultProfileName].ContainsKey(section))
            {
                _storage[DefaultProfileName].Remove(section);
            }

            _storage[DefaultProfileName].Add(section, cfg);
            Save();
        }

    }
}
