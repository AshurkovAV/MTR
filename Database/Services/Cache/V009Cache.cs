using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V009Cache : DictionaryCache<IEnumerable<V009>>
    {
        public V009Cache(IMedicineRepository repository)
        {
            var result = repository.GetV009();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDRMP.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDRMP.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
