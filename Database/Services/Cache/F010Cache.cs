using System.Collections.Generic;
using System.Linq;
using Medical.DatabaseCore.Services.Database;
using DataModel;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F010Cache : DictionaryCache<IEnumerable<F010>>
    {
        public F010Cache(IMedicineRepository repository)
        {
            var result = repository.GetF010();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.KOD_OKATO as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.KOD_OKATO as object);
                Data = result.Data;
            }
        }
    }
}
