using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V029Cache : DictionaryCache<IEnumerable<V029>>
    {
        public V029Cache(IMedicineRepository repository)
        {
            var result = repository.GetV029();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Id.ToString() as object, p => (int?)p.IDMET);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.N_MET.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
