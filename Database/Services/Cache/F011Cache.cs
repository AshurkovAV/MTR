using System.Collections.Generic;
using System.Linq;
using Medical.DatabaseCore.Services.Database;
using DataModel;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F011Cache : DictionaryCache<IEnumerable<F011>>
    {
        public F011Cache(IMedicineRepository repository)
        {
            var result = repository.GetF011();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDDoc as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.DocName as object);
                Data = result.Data;
            }
        }
    }
}
