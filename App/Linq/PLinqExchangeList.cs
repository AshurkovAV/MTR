using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqExchangeList : PLinqListBase<FactExchange>
    {
        public PLinqExchangeList(IMedicineRepository repository, Expression<Func<FactExchange, bool>> predicate = null)
            : base(repository, predicate)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetFactExchange(Predicate);
            if (result.Success)
            {
                Data = new List<FactExchange>(result.Data);
            }
        }
    }
}
