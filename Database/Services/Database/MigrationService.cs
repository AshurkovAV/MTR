using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using Core.Infrastructure;
using Core.Services;
using Medical.DatabaseCore.Migrations;

namespace Medical.DatabaseCore.Services.Database
{

    public class MigrationService : IMigrationService, IError
    {
        private readonly IAppShareSettings _settings;

        private Configuration _configuration;
        private readonly DbMigrator _migrator;
        

        public MigrationService(IAppShareSettings settings)
        {
            _settings = settings;
            Errors = new List<object>();
            dynamic databaseConfig = settings.Get("database");
            _configuration = new Configuration
            {
                TargetDatabase = new DbConnectionInfo((string) databaseConfig.ConnectionString, "System.Data.SqlClient")
            };
            _migrator = new DbMigrator(_configuration);
        }
        public IEnumerable<string> GetLocalMigrations()
        {
            return _migrator.GetLocalMigrations().Reverse();
        }

        public IEnumerable<string> GetDatabaseMigrations()
        {
            return _migrator.GetDatabaseMigrations().Reverse();
        }

        public IEnumerable<string> GetPendingMigrations()
        {
            return _migrator.GetPendingMigrations().Reverse();
        }

        public void Update(string migration)
        {
            try
            {
                _migrator.Update(migration);
                //TODO зачем это нужно хранить в настройках? 
                //dynamic migrationSettings = _settings.Get("migrations");
                //migrationSettings.lastMigrationName = migration;
                //_settings.Put("migrations", migrationSettings);
            }
            catch (Exception exceptioin)
            {
                AddError(exceptioin);
            }
           
        }

        public bool HasError
        {
            get { return Errors.Count > 0; }
        }

        public bool Success
        {
            get { return Errors.Count == 0; }
        }
        public IList<object> Errors { get; private set; }
        public void AddError(object error)
        {
            Errors.Add(error);
        }

        public Exception LastError
        {
            get
            {
                return HasError ? Errors.LastOrDefault() as Exception : null;
            }
        }
    }
}
