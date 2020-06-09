using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class M001Cache : DictionaryCache<IEnumerable<M001>>
    {
        public M001Cache(IMedicineRepository repository)
        {
            var result = repository.GetAll<M001>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDDS.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.DSNAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
