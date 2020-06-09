using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F002IdToNameCache : DictionaryCache
    {
        public F002IdToNameCache(IMedicineRepository repository)
        {
            var result = repository.GetF002();
            if (result.Success)
            {
                BackDictionary = result.Data.Distinct(new DistinctNameF002Comparer()).ToDictionary(p => p.nam_smok as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.nam_smok as object);
            }
        }

        
    }

    class DistinctNameF002Comparer : IEqualityComparer<F002>
    {

        public bool Equals(F002 x, F002 y)
        {
            return x.nam_smok == y.nam_smok;
        }

        public int GetHashCode(F002 obj)
        {
            return obj.nam_smok.GetHashCode();
        }
    }
}
