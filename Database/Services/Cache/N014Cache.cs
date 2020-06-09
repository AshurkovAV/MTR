using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N014Cache : DictionaryCache<IEnumerable<N014>>
    {
        public N014Cache(IMedicineRepository repository)
        {
            var result = repository.GetN014();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_THir.ToString() as object, p => (int?)p.ID_THir);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_THir, p => p.THir_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
