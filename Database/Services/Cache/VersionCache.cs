using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class VersionCache : DictionaryCache
    {
        public VersionCache(IMedicineRepository repository)
        {
            var result = repository.GetGlobalVersion();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Version as object, p => (int?)p.VersionID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.VersionID, p => p.Version as object);
            }
        }
    }
}
