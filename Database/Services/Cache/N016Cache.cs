using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N016Cache : DictionaryCache<IEnumerable<N016>>
    {
        public N016Cache(IMedicineRepository repository)
        {
            var result = repository.GetN016();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_TLek_V.ToString() as object, p => (int?)p.ID_TLek_V);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_TLek_V, p => p.TLek_NAME_V.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
