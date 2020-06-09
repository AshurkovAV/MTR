using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F003Cache : DictionaryCache<IEnumerable<F003>>
    {
        public F003Cache(IMedicineRepository repository)
        {
            var result = repository.GetF003();
            if (result.Success)
            {
                BackDictionary = result.Data.Distinct(new DistinctF003Comparer()).ToDictionary(p => p.mcod as object, p => (int?)p.Id);
                Dictionary = result.Data.Distinct(new DistinctF003Comparer()).ToDictionary(p => (int?)p.Id, p => p.mcod as object);
                Data = result.Data;
            }
        }
    }

    class DistinctF003Comparer : IEqualityComparer<F003>
    {

        public bool Equals(F003 x, F003 y)
        {
            return x.mcod == y.mcod;
        }

        public int GetHashCode(F003 obj)
        {
            return obj.mcod.GetHashCode();
        }
    }
}
