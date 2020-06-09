using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F005Cache : DictionaryCache<IEnumerable<F005>>
    {
        public F005Cache(IMedicineRepository repository)
        {
            var result = repository.GetF005();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDIDST.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDIDST.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
