using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N003Cache : DictionaryCache<IEnumerable<N003>>
    {
        public N003Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<N003>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_T.ToString() as object, p => (int?)p.ID_T);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_T, p => p.ID_T.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
