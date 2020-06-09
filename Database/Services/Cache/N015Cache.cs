using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N015Cache : DictionaryCache<IEnumerable<N015>>
    {
        public N015Cache(IMedicineRepository repository)
        {
            var result = repository.GetN015();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_TLek_L.ToString() as object, p => (int?)p.ID_TLek_L);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_TLek_L, p => p.TLek_NAME_L.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
