using System;
using System.Globalization;

namespace Core.Extensions
{
    public static class Int32Extension
    {
        /// <summary>
        /// Преобразовать int? в строку
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringNullable(this int? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Преобразовать int? в строку используя формат
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToStringNullable(this int? value, string format)
        {
            return value.HasValue ? value.Value.ToString(format, CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// Преобразовать int? в строку используя формат
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToStringNullable(this int? value, string format, string defaultValue)
        {
            return value.HasValue ? value.Value.ToString(format, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Проверка числа на четность
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;
        }

        /// <summary>
        /// Проверка числа на нечетность
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(this int value)
        {
            return !IsOdd(value);
        }

        /// <summary>
        /// Проверка что число четное или нечетное
        /// </summary>
        /// <param name="value"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool IsSimilarOddOrEven(this int value, int src)
        {
            if (value == 0 || src == 0)
            {
                return false;
            }

            if (src.IsOdd())
            {
                return value.IsOdd();
            }

            if (src.IsEven())
            {
                return value.IsEven();
            }

            return false;
        }

        /// <summary>
        /// Проверка что число неявляется четным или нечетным
        /// </summary>
        /// <param name="value"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool IsNotSimilarOddOrEven(this int value, int src)
        {
            return !IsSimilarOddOrEven(value, src);
        }

    }
}
