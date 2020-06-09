using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.DatabaseCore.Services.Cache
{
    public class F004Cache : DictionaryCache<IEnumerable<F004>>
    {
        public F004Cache(IMedicineRepository repository)
        {
            var result = repository.GetF004();
            if (result.Success)
            {
                //BackDictionary = result.Data.ToDictionary(p => p.code.ToString() as object, p => (int?)p.Id);
                //Dictionary = result.Data.ToDictionary(p => (int?)p.Id, p => p.code.ToString() as object);
                Data = result.Data;
            }
        }
    }
}
