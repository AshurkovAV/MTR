using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V005Cache : DictionaryCache<IEnumerable<V005>>
    {
        public V005Cache(IMedicineRepository repository)
        {
            var result = repository.GetV005();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDPOL.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDPOL.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
