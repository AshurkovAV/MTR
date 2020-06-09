using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V025Cache : DictionaryCache<IEnumerable<V025>>
    {
        public V025Cache(IMedicineRepository repository)
        {
            var result = repository.GetV025();
            if (result.Success)
            {
                //BackDictionary = result.Data.ToDictionary(p => p.IDPC.ToString() as object, p => (int?)p.IDPC);
                //Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDDKK.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
