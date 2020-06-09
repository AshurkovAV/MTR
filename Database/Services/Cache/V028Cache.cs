using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V028Cache : DictionaryCache<IEnumerable<V028>>
    {
        public V028Cache(IMedicineRepository repository)
        {
            var result = repository.GetV028();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDVN.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.N_VN.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
