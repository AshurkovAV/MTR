using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N004Cache : DictionaryCache<IEnumerable<N004>>
    {
        public N004Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<N004>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_N.ToString() as object, p => (int?)p.ID_N);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_N, p => p.ID_N.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
