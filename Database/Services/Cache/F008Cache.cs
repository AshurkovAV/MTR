using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F008Cache : DictionaryCache<IEnumerable<F008>>
    {
        public F008Cache(IMedicineRepository repository)
        {
            var result = repository.GetF008();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDDOC.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.DOCNAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
