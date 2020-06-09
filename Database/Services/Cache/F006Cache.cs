using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F006Cache : DictionaryCache<IEnumerable<F006>>
    {
        public F006Cache(IMedicineRepository repository)
        {
            var result = repository.GetF006();
            if (result.Success)
            {
                BackDictionary = result.Data.ToDictionary(p => p.IDVID.ToString() as object, p => (int?)p.Id);
                Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.IDVID.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
