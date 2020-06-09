using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V023Cache : DictionaryCache<IEnumerable<V023>>
    {
        public V023Cache(IMedicineRepository repository)
        {
            var result = repository.GetV023();
            if (result.Success)
            {
               // BackDictionary = result.Data.ToDictionary(p => p.IDUMP.ToString() as object, p => (int?)p.IDUMP);
               // Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDDKK.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
