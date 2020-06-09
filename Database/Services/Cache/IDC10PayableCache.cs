using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class IDC10PayableCache : DictionaryCache
    {
        public IDC10PayableCache(IMedicineRepository repository)
        {
            var result = repository.GetM001();
            if (result.Success)
            {
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.Payable as object);
            }
        }
    }
}
