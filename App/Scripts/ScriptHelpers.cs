using System.Data;
using Autofac;
using BLToolkit.Data;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Helpers;
using Medical.CoreLayer.Service;
using DataException = BLToolkit.Data.DataException;

namespace Medical.AppLayer.Scripts
{
    /// <summary>
    /// Класс для вызова методов из скриптов во вкладке настроек
    /// </summary>
    public static class ScriptHelpers
    {
        /// <summary>
        /// Тестирование соединения с БД
        /// </summary>
        /// <param name="value">Объект, содержащий настройки соединения с БД</param>
        public static void TestDatabase(object value)
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var messageService = scope.Resolve<IMessageService>();
                var commonService = scope.Resolve<ICommonService>();

                value
                    .ThrowIfArgumentIsNull()
                    .ThrowIfArgumentIsNullAsDynamic();

                var connectionString = commonService.ConstructDatabaseConnectionString(value);
                try
                {
                    using (var db = new DbManager("Sql".ToDataProvider(), connectionString))
                    {
                        if (ConnectionState.Open == db.Connection.State)
                        {
                            messageService.ShowMessage(CoreMessages.ConnectionSuccess);
                        }
                        else
                        {
                            messageService.ShowMessage(CoreMessages.WrongConnectionState);
                        }
                    }
                }
                catch (DataException exception)
                {
                    messageService.ShowException(exception, "Исключение при попытке подключиться к базе данных", typeof(ScriptHelpers));
                }
            }
            
        }

        /// <summary>
        /// Тестирование соединения с сервисом данных WCF
        /// </summary>
        /// <param name="value">Объект, содержащий настройки соединения с сервисом данных WCF</param>
        public static void TestDataService(object value)
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var messageService = scope.Resolve<IMessageService>();
                var remoteService = scope.Resolve<IRemoteService>();

                value
                    .ThrowIfArgumentIsNull()
                    .ThrowIfArgumentIsNullAsDynamic();

                var connectionString = remoteService.ConstructDataServiceConnectionString(value);

                try
                {
                    var bindingResult = remoteService.CreateBinding((value as dynamic).ServiceProtocol);
                    if (!bindingResult.Success)
                    {
                        messageService.ShowWarning(CoreMessages.WrongServiceProtocol);
                        return;
                    }

                    var result = remoteService.Test(connectionString, bindingResult.Data);
                    if (result.Success)
                    {
                        messageService.ShowMessage(CoreMessages.ConnectionSuccess);
                    }
                    else
                    {
                        messageService.ShowException(result.LastError, "Исключение при попытке подключиться к сервису данных", typeof(ScriptHelpers));
                    }
                }
                catch (DataException exception)
                {
                    messageService.ShowException(exception, "Исключение при попытке подключиться к сервису данных", typeof(ScriptHelpers));
                }
            }
        }
    }
}
