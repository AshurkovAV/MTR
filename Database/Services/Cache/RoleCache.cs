using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class RoleCache : DictionaryCache
    {
        public RoleCache(IMedicineRepository repository)
        {
            var result = repository.GetRoles();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Name as object, p => (int?)p.RoleID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.RoleID, p => p.Name as object);
            }
        }
    }
}
