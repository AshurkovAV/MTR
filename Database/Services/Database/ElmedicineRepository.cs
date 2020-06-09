using System;
using System.Collections.Generic;
using System.Data;
using BLToolkit.Data.DataProvider;
using Core.Infrastructure;

namespace Medical.DatabaseCore.Services.Database
{
    public class ElmedicineRepository : IElmedicineRepository
    {
        private readonly string _defaultProvider;
        private readonly string _defaultName;

        public ElmedicineRepository()
        {
            _defaultProvider = ProviderName.MsSql;
            _defaultName = AppRemoteSettings.ElmedicineConnectionString;
        }

        private DatabaseContext CreateContext()
        {
            return new DatabaseContext(_defaultProvider,_defaultName);
        }

        public TransactionResult<IEnumerable<string>> GetTableNameList()
        {
            var result = new TransactionResult<IEnumerable<string>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = db.GetTableNameList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<IEnumerable<T>> GetDataByTableName<T>(string tableName)
        {
            var result = new TransactionResult<IEnumerable<T>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = db.GetDataByTableName<T>(tableName);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<DataSet> GetDataSetByTableName(string tableName)
        {
            var result = new TransactionResult<DataSet>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = db.GetDataSetByTableName(tableName);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }
    }
}
