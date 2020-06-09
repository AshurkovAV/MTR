using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class N013Cache : DictionaryCache<IEnumerable<N013>>
    {
        public N013Cache(IMedicineRepository repository)
        {
            var result = repository.GetN013();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.ID_TLech.ToString() as object, p => (int?)p.ID_TLech);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ID_TLech, p => p.TLech_NAME.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
