using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class ParamCache : DictionaryCache
    {
        public ParamCache(IMedicineRepository repository)
        {
            var result = repository.GetGlobalParam();
            if (result.Success)
            {
                //BackDictionary = result.Data.ToDictionary(p => p.ReportName as object, p => (int?)p.ParamID);
                Dictionary = result.Data.ToDictionary(p => (int?)p.ParamID, p => p.ReportName as object);
            }
        }
    }
}
