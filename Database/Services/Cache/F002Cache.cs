using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F002Cache : DictionaryCache<IEnumerable<F002>>
    {
        public F002Cache(IMedicineRepository repository)
        {
            var result = repository.GetF002();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.smocod as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.smocod as object);
                Data = result.Data;
            }
        }
    }
}
