using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V024Cache : DictionaryCache<IEnumerable<V024>>
    {
        public V024Cache(IMedicineRepository repository)
        {
            var result = repository.GetV024();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDDKK as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDDKK.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
