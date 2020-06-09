using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class ShareDoctorToCodeCache : DictionaryCache
    {
        public ShareDoctorToCodeCache(IMedicineRepository repository)
        {
            var result = repository.GetShareDoctor();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.Code as object, p => (int?)p.DoctorId);
                Dictionary = result.Data.ToDictionary(p => (int?)p.DoctorId, p => p.Code as object);
            }
        }
    }
}
