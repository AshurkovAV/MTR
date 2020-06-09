using System.Collections.Generic;

namespace Core.Infrastructure
{
    public class CoreMessages
    {
        public const string DbTransactionError = "Ошибка транзакции";
        public const string DbTransactionSuccess = "Транзакция успешно завершена";
        public const string DbException = "Исключение при работе с базой данных";

        public const string OperationCancel = "Операция отменена";

        public const string DataNotFound = "Данные типа: {0} ID: {1} не найдены";
        public const string DataSaveSuccess = "Данные успешно записаны";

        
        public const string ConnectionSuccess = "Соединение прошло успешно";
        public static string WrongAnswer = "Неверный ответ сервера";
        public static string WrongConnectionState = "Неверное состояние соединения с сервером";

        public const string Info = "Информация";
        public static string Warning = "Внимание";
        public static string Error = "Ошибка";
        public static string WrongServiceProtocol = "Неверный протокол взаимодействия";

        public static IEnumerable<string> GetMessages(params string[] list)
        {
            return new List<string>(list);
        }
    }
}
