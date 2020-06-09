using System.Globalization;
using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V004Cache : DictionaryCache
    {
        public V004Cache(IMedicineRepository repository)
        {
            var result = repository.GetV004();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDMSP.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDMSP.ToString() as object);
            }
        }
    }
}
