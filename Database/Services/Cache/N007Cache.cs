using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N007Cache : DictionaryCache<IEnumerable<N007>>
    {
        public N007Cache(IMedicineRepository repository)
        {
            var result = repository.GetN007();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_Mrf.ToString() as object, p => (int?)p.ID_Mrf);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_Mrf, p => p.Mrf_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
