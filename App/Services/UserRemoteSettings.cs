using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Medical.DatabaseCore.Services.Database;
using Newtonsoft.Json;

namespace Medical.AppLayer.Services
{
    /// <summary>
    /// Хранение настроек пользователя в БД
    /// </summary>
    public class UserRemoteSettings : IUserRemoteSettings
    {
        private readonly IMedicineRepository _repository;
        private IUserService _userService;

        private Dictionary<string, object> _storage;

        public UserRemoteSettings(IMedicineRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
            Load();
        }

        public bool Save()
        {
            try
            {
                foreach (var entry in _storage)
                {
                    var result = _repository.UpdateUserSettings(entry.Key, entry.Value);
                    if (result.Success)
                    {
                        //TODO 
                    }
                }
                
                return true;
            }
            catch (Exception exception)
            {
                //TODO add error
            }
            return false;
        }

        public bool Load()
        {
            try
            {
                var result = _repository.GetUserSettings(_userService.UserId);
                if (result.Success)
                {
                    _storage = new Dictionary<string, object>();
                    result.Data.Select(p=>new
                    {
                        key = p.Key,
                        value = p.Value
                    }).ForEachAction(p => _storage.Add(p.key, JsonConvert.DeserializeObject<object>(p.value)));
                }
                return true;
            }
            catch (Exception exception)
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
