using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F014aCache : DictionaryCache
    {
        public F014aCache(IMedicineRepository repository)
        {
            var result = repository.GetF014();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Osn as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.Osn as object);
            }
        }
    }
}
