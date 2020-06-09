using System.Globalization;
using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class V015Cache : DictionaryCache
    {
        public V015Cache(IMedicineRepository repository)
        {
            var result = repository.GetV015();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.CODE.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.CODE.ToString() as object);
            }
        }
    }
}
