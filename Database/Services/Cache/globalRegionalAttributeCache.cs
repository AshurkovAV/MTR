using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Medical.DatabaseCore.Services.Database;
using DataModel;

namespace Medical.DatabaseCore.Services.Cache
{
    public class GlobalRegionalAttributeCache : DictionaryCache<IEnumerable<globalRegionalAttribute>>
    {
        public GlobalRegionalAttributeCache(IMedicineRepository repository)
        {
            var result = repository.GetAll<globalRegionalAttribute>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID as object, p => (int?)p.ID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID, p => p.Name as object);
                Data = result.Data;
            }
        }
    }
}
