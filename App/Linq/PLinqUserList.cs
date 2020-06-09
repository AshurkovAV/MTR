using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqUserList : PLinqListBase<localUser>
    {
        public PLinqUserList(IMedicineRepository repository, Expression<Func<localUser, bool>> predicate = null)
            : base(repository, predicate)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetLocalUser(Predicate);
            if (result.Success)
            {
                Data = new List<localUser>(result.Data);
            }
        }
    }
}
