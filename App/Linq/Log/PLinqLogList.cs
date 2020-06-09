using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Log
{
    public class PLinqLogList : PLinqMedicineLogBase<LogEntries>
    {
        public PLinqLogList(IMedicineLogRepository repository, Expression<Func<LogEntries, bool>> predicate = null)
            : base(repository, predicate)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetLog(Predicate);
            if (result.Success)
            {
                Data = new List<LogEntries>(result.Data);
            }
        }
    }
}
