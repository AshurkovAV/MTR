using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class ExaminationGroupCache : DictionaryCache
    {
        public ExaminationGroupCache(IMedicineRepository repository)
        {
            var result = repository.GetAll<globalExaminationGroup>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Name as object, p => (int?)p.ExaminationGroupID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ExaminationGroupID, p => p.Name as object);
            }
        }
    }
}
