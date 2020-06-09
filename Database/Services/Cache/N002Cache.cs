using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N002Cache : DictionaryCache<IEnumerable<N002>>
    {
        public N002Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<N002>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_St.ToString() as object, p => (int?)p.ID_St);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_St, p => p.DS_St.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
