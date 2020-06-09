using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N010Cache : DictionaryCache<IEnumerable<N010>>
    {
        public N010Cache(IMedicineRepository repository)
        {
            var result = repository.GetN010();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_Igh.ToString() as object, p => (int?)p.ID_Igh);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_Igh, p => p.Igh_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
