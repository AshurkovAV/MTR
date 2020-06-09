using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.DatabaseCore.Services.Database
{
    public interface IMigrationService
    {
        IEnumerable<string> GetLocalMigrations();
        IEnumerable<string> GetDatabaseMigrations();
        IEnumerable<string> GetPendingMigrations();
        void Update(string migration);
    }
}
