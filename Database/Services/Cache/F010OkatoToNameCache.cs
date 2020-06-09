using System.Linq;
using Core.Extensions;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F010OkatoToNameCache : DictionaryCache
    {
        public F010OkatoToNameCache(IMedicineRepository repository)
        {
            var result = repository.GetF010();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.SUBNAME as object, p => p.KOD_OKATO.ToInt32Nullable());
                Dictionary = result.Data.ToDictionary(p => p.KOD_OKATO.ToInt32Nullable(), p => p.SUBNAME as object);
            }
        }
    }
}
