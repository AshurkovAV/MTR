using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Infrastructure;
using DataModel;

namespace Medical.DatabaseCore.Services.Database
{
    public interface IMedicineLogRepository
    {
        TransactionResult<IEnumerable<LogEntries>> GetLog(Expression<Func<LogEntries, bool>> predicate = null);   
    }
}