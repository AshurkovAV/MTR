using System.Linq;
using Core.Extensions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class RefusalPenaltyCache : DictionaryCache
    {
        public RefusalPenaltyCache(IMedicineRepository repository)
        {
            var result = repository.GetAll<globalRefusalPenalty>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p as object, p => p.Reason.Replace(".","").ToInt32Nullable());
                Dictionary = result.Data.Distinct(p => p.Reason.Replace(".", "").ToInt32Nullable()).ToDictionary(p => p.Reason.Replace(".", "").ToInt32Nullable(), p => p as object);
            }
        }
    }
}
