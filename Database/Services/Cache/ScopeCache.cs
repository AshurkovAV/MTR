using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class ScopeCache : DictionaryCache
    {
        public ScopeCache(IMedicineRepository repository)
        {
            var result = repository.GetGlobalScope();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Name as object, p => (int?)p.ScopeID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ScopeID, p => p.Name as object);
            }
        }
    }
}
