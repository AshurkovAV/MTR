using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F014Cache : DictionaryCache<IEnumerable<F014>>
    {
        public F014Cache(IMedicineRepository repository)
        {
            var result = repository.GetF014();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Kod as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.Kod as object);
                Data = result.Data;
            }
        }
    }
}
