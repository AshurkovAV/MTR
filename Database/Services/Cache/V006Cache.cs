using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V006Cache : DictionaryCache<IEnumerable<V006>>
    {
        public V006Cache(IMedicineRepository repository)
        {
            var result = repository.GetV006();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.UMPNAME as object, p => (int?)p.id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.id, p => p.UMPNAME as object);
                Data = result.Data;
            }
        }
    }
}
