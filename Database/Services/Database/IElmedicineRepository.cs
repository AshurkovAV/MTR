using System.Collections.Generic;
using System.Data;
using Core.Infrastructure;

namespace Medical.DatabaseCore.Services.Database
{
    public interface IElmedicineRepository
    {
        TransactionResult<IEnumerable<string>> GetTableNameList();
        TransactionResult<IEnumerable<T>> GetDataByTableName<T>(string tableName);
        TransactionResult<DataSet> GetDataSetByTableName(string tableName);
    }
}