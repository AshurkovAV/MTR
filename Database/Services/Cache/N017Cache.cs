using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N017Cache : DictionaryCache<IEnumerable<N017>>
    {
        public N017Cache(IMedicineRepository repository)
        {
            var result = repository.GetN017();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_TLuch.ToString() as object, p => (int?)p.ID_TLuch);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_TLuch, p => p.TLuch_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
