using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class AccountTypeCache : DictionaryCache
    {
        public AccountTypeCache(IMedicineRepository repository)
        {
            var result = repository.GetGlobalAccountType();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.TypeName as object, p => (int?)p.AccountTypeId);
                Dictionary = result.Data.ToDictionary(p => (int?)p.AccountTypeId, p => p.TypeName as object);
            }
        }
    }
}
