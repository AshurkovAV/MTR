using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V020Cache : DictionaryCache<IEnumerable<V020>>
    {
        public V020Cache(IMedicineRepository repository)
        {
            var result = repository.GetV020();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDK_PR.ToString() as object, p => (int?)p.IDK_PR);
                Dictionary = result.Data.ToDictionary(p => (int?)p.IDK_PR, p => p.K_PRNAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
