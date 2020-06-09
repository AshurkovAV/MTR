using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F002ToTfOkatoCache : DictionaryCache
    {
        public F002ToTfOkatoCache(IMedicineRepository repository)
        {
            var result = repository.GetF002();
            if (result.Success)
            {
                BackDictionary = null;
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.tf_okato as object);
            }
        }
    }

    class DistinctSmocodeF002Comparer : IEqualityComparer<F002>
    {

        public bool Equals(F002 x, F002 y)
        {
            return x.smocod == y.smocod;
        }

        public int GetHashCode(F002 obj)
        {
            return obj.smocod.GetHashCode();
        }
    }

    class DistinctOkatoF002Comparer : IEqualityComparer<F002>
    {

        public bool Equals(F002 x, F002 y)
        {
            return x.tf_okato == y.tf_okato;
        }

        public int GetHashCode(F002 obj)
        {
            return obj.tf_okato.GetHashCode();
        }
    }
}
