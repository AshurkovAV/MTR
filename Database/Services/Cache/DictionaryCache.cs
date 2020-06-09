using System.Collections.Generic;
using Core.Extensions;

namespace Medical.DatabaseCore.Services.Cache
{
    public class DictionaryCache<T> : DictionaryCache
    {
        public T Data { get; set; }
    }
    public class DictionaryCache : ICache
    {
        protected Dictionary<int?, object> Dictionary;
        protected Dictionary<object, int?> BackDictionary;
        
        public DictionaryCache()
        {
            Dictionary = new Dictionary<int?, object>();
            BackDictionary = new Dictionary<object, int?>();
        }

        public DictionaryCache(Dictionary<int?, object> dictionary, Dictionary<object, int?> backDictionary)
        {
            Dictionary = dictionary;
            BackDictionary = backDictionary;
        }

        public void Put(int? key, object value)
        {
            if (key.HasValue && !Dictionary.ContainsKey(key))
            {
                Dictionary.Add(key, value);
            }
        }

        public object Get(int? key)
        {
            Dictionary.ThrowIfArgumentIsNull("Get doesn't allow");

            if (key.HasValue && Dictionary.ContainsKey(key))
            {
                return Dictionary[key];
            }
            return null;
        }
        public string GetString(int? key)
        {
            Dictionary.ThrowIfArgumentIsNull("Get string doesn't allow");

            if (key.HasValue && Dictionary.ContainsKey(key))
            {
                return Dictionary[key] as string;
            }
            return null;
        }

        public void PutBack(object key, int? value)
        {
            if (value.HasValue && !BackDictionary.ContainsKey(key))
            {
                BackDictionary.Add(key, value);
            }
        }

        public int? GetBack(object key)
        {
            if (key == null)
            {
                return null;
            }
            BackDictionary.ThrowIfArgumentIsNull("Get back doesn't allow");
            if (BackDictionary.ContainsKey(key))
            {
                return BackDictionary[key];
            }
            return null;
        }

        public int GetBackInt(object key)
        {
            return GetBack(key) ?? 0;
        }

        public void Info()
        {
            //TODO
        }

        public bool KeyExists(int? key)
        {
            return Dictionary.IsNotNull() && Dictionary.ContainsKey(key);
        }

        public bool ValueExists(object key)
        {
            return BackDictionary.IsNotNull() && BackDictionary.ContainsKey(key);
        }
    }
}
