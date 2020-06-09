using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Data.DataProvider;
using BLToolkit.Data.Linq;
using Core.Infrastructure;
using DataModel;

namespace Medical.DatabaseCore.Services.Database
{
    /// <summary>
    /// Доступ к базе данных логов
    /// </summary>
    public class MedicineLogRepository : IMedicineLogRepository
    {
        /// <summary>
        /// Логгер NLog
        /// <remarks>Инициализация происходит посредством иньекции через свойства (Autofac.Extras.NLog)</remarks>
        /// </summary>
        //public ILogger Logger { get; set; }

        //Провайдер по умолчанию
        private readonly string _defaultProvider;
        //Название конфигурации
        private readonly string _defaultName;

        #region Queries
        //запрос на получение записей логов
        private static readonly Func<MedicineLogContext, IEnumerable<LogEntries>> GetLogQuery =
            CompiledQuery.Compile<MedicineLogContext, IEnumerable<LogEntries>>(db =>
            from e in db.LogEntries
            orderby e.TimeStamp descending 
            select e
        );
        #endregion

        public MedicineLogRepository()
        {
            //инициализация провайдера и строки по умолчанию
            _defaultProvider = ProviderName.MsSql;
            _defaultName = AppRemoteSettings.MedicineLogConnectionString;
        }

        private MedicineLogContext CreateContext()
        {
            return new MedicineLogContext(_defaultProvider,_defaultName);
        }

        /// <summary>
        /// Получение всех логов или согласно предикату
        /// </summary>
        /// <param name="predicate">Предикат (фильтр) логов</param>
        /// <returns>Список записей логов</returns>
        public TransactionResult<IEnumerable<LogEntries>> GetLog(Expression<Func<LogEntries, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<LogEntries>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate == null ? 
                        GetLogQuery(db).ToList():
                        db.LogEntries.Where(predicate).OrderByDescending(p=>p.TimeStamp).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }
    }
        

}
