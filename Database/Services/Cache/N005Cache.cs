using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N005Cache : DictionaryCache<IEnumerable<N005>>
    {
        public N005Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<N005>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_M.ToString() as object, p => (int?)p.ID_M);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_M, p => p.ID_M.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
