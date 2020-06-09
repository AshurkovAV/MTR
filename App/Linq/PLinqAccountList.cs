using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqAccountList : PLinqListBase<FactTerritoryAccount>
    {
        public PLinqAccountList(IMedicineRepository repository, Expression<Func<FactTerritoryAccount, bool>> predicate = null)
            : base(repository, predicate)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetFactTerritoryAccount(Predicate);
            if (result.Success)
            {
                Data = result.Data.ToList();
            }
        }
    }
}
