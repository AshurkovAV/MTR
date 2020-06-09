using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Medical.DatabaseCore.Services.Database;
using DataModel;

namespace Medical.DatabaseCore.Services.Cache
{
    public class ConsultationOnkCache : DictionaryCache<IEnumerable<globalConsultationOnk>>
    {
        public ConsultationOnkCache(IMedicineRepository repository)
        {
            var result = repository.GetAll<globalConsultationOnk>();
            if (result.Success)
            {
                //BackDictionary = result.Data.ToDictionary(p => p.PRNAME as object, p => (int?)p.Id);
                //Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.PRNAME as object);
                Data = result.Data;
            }
        }
    }
}
