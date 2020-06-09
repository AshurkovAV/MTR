using System.Collections.Generic;
using System.Linq;
using Medical.DatabaseCore.Services.Database;
using DataModel;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V008Cache : DictionaryCache<IEnumerable<V008>>
    {
        public V008Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<V008>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.VMPNAME as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.VMPNAME as object);
                Data = result.Data;
            }
        }
    }
}
