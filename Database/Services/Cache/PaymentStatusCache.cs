using System.Linq;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class PaymentStatusCache : DictionaryCache
    {
        public PaymentStatusCache(IMedicineRepository repository)
        {
            var result = repository.GetGlobalPaymentStatus();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.StatusName as object, p => p.StatusCode);
                Dictionary = result.Data.ToDictionary(p => (int?)p.StatusCode, p => p.StatusName as object);
            }
        }
    }
}
