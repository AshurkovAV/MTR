using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqProcessingList : PLinqListBase<FactProcessing>
    {
        public PLinqProcessingList(IMedicineRepository repository, Expression<Func<FactProcessing, bool>> predicate = null)
            : base(repository, predicate)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetProcessing(Predicate);
            if (result.Success)
            {
                Data = new List<FactProcessing>(result.Data);
            }
        }
    }
}
