using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N008Cache : DictionaryCache<IEnumerable<N008>>
    {
        public N008Cache(IMedicineRepository repository)
        {
            var result = repository.GetN008();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_R_M.ToString() as object, p => (int?)p.ID_R_M);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_R_M, p => p.R_M_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
