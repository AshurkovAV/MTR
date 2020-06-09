using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N020Cache : DictionaryCache<IEnumerable<N020>>
    {
        public N020Cache(IMedicineRepository repository)
        {
            var result = repository.GetN020();
            if (result.Success)
            {
                //BackDictionary = result.Data.ToDictionary(p => p.ID_LEKP.ToString() as object, p => (int?)p.ID_LEKP);
                //Dictionary = result.Data.ToDictionary(p => p.ID_LEKP, p => p.MNN.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
