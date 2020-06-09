
using System;

namespace Core.Extensions
{
    public static class BoolExtension
    {
        /// <summary>
        /// Преобразование nullable bool в строковую 1 или 0
        /// </summary>
        /// <param name="value">bool?</param>
        /// <returns>"1" или "0"</returns>
        public static string ToStringNullable(this bool? value)
        {
            return value.HasValue ? (value.Value ? "1" : "0") : null;
        }

        /// <summary>
        /// Выполнение action в случае если условие true
        /// </summary>
        /// <param name="value">bool?</param>
        /// <param name="action">action</param>
        public static void IfTrue(this bool? value, Action action)
        {
            if (value.HasValue)
            {
                value.Value.IfTrue(action);
            }
        }

        /// <summary>
        /// Выполнение action в случае если условие true
        /// </summary>
        /// <param name="value">bool</param>
        /// <param name="action">action</param>
        public static void IfTrue(this bool value, Action action)
        {
            if (value && action.IsNotNull())
            {
                action();
            }
        }

        /// <summary>
        /// Выполнение success в случае если условие true, fail в случае если false (и fail != null)
        /// </summary>
        /// <param name="value">bool</param>
        /// <param name="action">action</param>
        public static void DoCond(this bool value, Action success, Action fail = null)
        {
            if (value && success.IsNotNull())
            {
                success();
            }

            if (!value && fail.IsNotNull())
            {
                fail();
            }
        }
    }
}
