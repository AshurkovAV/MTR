using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class IDC10Cache : DictionaryCache
    {
        public IDC10Cache(IMedicineRepository repository)
        {
            var result = repository.GetM001();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDDS as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDDS as object);
            }
        }
    }
}
