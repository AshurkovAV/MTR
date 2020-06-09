using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V010Cache : DictionaryCache<IEnumerable<V010>>
    {
        public V010Cache(IMedicineRepository repository)
        {
            var result = repository.GetV010();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDSP.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.SPNAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
