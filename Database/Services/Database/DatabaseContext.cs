using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autofac;
using BLToolkit.Data;
using Core.Services;

namespace Medical.DatabaseCore.Services.Database
{
    public class DatabaseContext : DbManager
    {
        static DatabaseContext()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                //TODO check if user not logged in throw exception!!!
                var settings = scope.Resolve<IAppShareSettings>();
                dynamic databaseConfig = settings.Get("database");
                AddConnectionString((string)databaseConfig.Provider, "medicine", (string)databaseConfig.ConnectionString);
            }
        }

        public DatabaseContext(string provider, string config)
            : base(provider, config)
        {
        }

        public IQueryable<T> GetTableQuery<T>() where T : class
        {
            return GetTable<T>().AsQueryable();
        }

        public IEnumerable<string> GetTableNameList()
        {
            try
            {
                var result = new List<string>(
                    SetCommand(@"SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES
                        WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME")
                        .ExecuteScalarList<string>());
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<T> GetDataByTableName<T>(string tableName)
        {
            try
            {
                Command.CommandTimeout = 0;
                var result = SetCommand("SELECT * FROM " + tableName + " ")
                    .ExecuteList<T>();
                
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataSet GetDataSetByTableName(string tableName)
        {
            try
            {
                Command.CommandTimeout = 0;
                var result = SetCommand("SELECT * FROM " + tableName + " ")
                    .ExecuteDataSet();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void TrancateTable(string tableName)
        {
            try
            {
                Command.CommandTimeout = 0;
                SetCommand("TRUNCATE TABLE " + tableName + " ")
                    .ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                
            }
        }

        public void DeleteTable(string tableName) { 
            try
            {
                Command.CommandTimeout = 0;
                SetCommand("DELETE FROM " + tableName + " ")
                    .ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                
            }
            
        }

        public Exception LastError { get; set; }
    }
}
