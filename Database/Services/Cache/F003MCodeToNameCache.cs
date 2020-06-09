using System.Linq;
using Core.Extensions;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F003MCodeToNameCache : DictionaryCache
    {
        public F003MCodeToNameCache(IMedicineRepository repository)
        {
            var result = repository.GetF003();// repository.GetF003ByOkato("38000");
            if (result.Success)
            {
                BackDictionary = result.Data.Distinct(new DistinctF003Comparer()).ToDictionary(p => p.mcod as object, p => p.Id.ToInt32Nullable());
                Dictionary = result.Data.Distinct(new DistinctF003Comparer()).ToDictionary(p => p.Id.ToInt32Nullable(), p => p.mcod as object);
            }
        }
    }
}
