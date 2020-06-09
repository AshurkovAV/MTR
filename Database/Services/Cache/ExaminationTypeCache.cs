using System.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class ExaminationTypeCache : DictionaryCache
    {
        public ExaminationTypeCache(IMedicineRepository repository)
        {
            var result = repository.GetAll<globalExaminationType>();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Name as object, p => (int?)p.ExaminationTypeID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ExaminationTypeID, p => p.Name as object);
            }
        }
    }
}
