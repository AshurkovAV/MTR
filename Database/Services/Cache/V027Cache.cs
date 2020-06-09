using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V027Cache : DictionaryCache<IEnumerable<V027>>
    {
        public V027Cache(IMedicineRepository repository)
        {
            var result = repository.GetV027();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Id.ToString() as object, p => (int?)p.IDCZ);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.N_CZ.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
