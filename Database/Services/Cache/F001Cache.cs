using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F001Cache : DictionaryCache<IEnumerable<F001>>
    {
        public F001Cache(IMedicineRepository repository)
        {
            var result = repository.GetF001();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.tf_code as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.tf_code as object);
                Data = result.Data;
            }
        }
    }
}
