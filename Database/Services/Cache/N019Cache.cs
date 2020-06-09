using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N019Cache : DictionaryCache<IEnumerable<N019>>
    {
        public N019Cache(IMedicineRepository repository)
        {
            var result = repository.GetN019();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_CONS.ToString() as object, p => (int?)p.ID_CONS);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_CONS, p => p.CONS_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
