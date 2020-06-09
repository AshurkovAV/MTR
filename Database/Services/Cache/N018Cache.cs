using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N018Cache : DictionaryCache<IEnumerable<N018>>
    {
        public N018Cache(IMedicineRepository repository)
        {
            var result = repository.GetN018();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_REAS.ToString() as object, p => (int?)p.ID_REAS);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_REAS, p => p.REAS_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
