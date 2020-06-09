using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N001Cache : DictionaryCache<IEnumerable<N001>>
    {
        public N001Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<N001>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_PrOt.ToString() as object, p => (int?)p.ID_PrOt);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_PrOt, p => p.PrOt_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
