using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class AccountStatusCache : DictionaryCache
    {
        public AccountStatusCache(IMedicineRepository repository)
        {
            var result = repository.GetGlobalAccountStatus();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.StatusName as object, p => p.StatusCode);
                Dictionary = result.Data.ToDictionary(p => (int?)p.StatusCode, p => p.StatusName as object);
            }
        }
    }
}
