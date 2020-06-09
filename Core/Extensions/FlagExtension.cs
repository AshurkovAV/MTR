using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.DataStructure;

namespace Core.Extensions
{
    public static class FlagExtension
    {
        /// <summary>
        /// Проверка на наличие флага
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Has<T>(this T flags, T flag) where T : struct
        {
            var flagsValue = (int)(object)flags;
            var flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        /// <summary>
        /// Проверка на отсутствие флага
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Missing<T>(this T flags, T flag) where T : struct
        {
            return !FlagExtension.Has<T>(flags, flag);
        }

        /// <summary>
        /// Добавить флаг
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static T Add<T>(this Enum value, T append)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            _Value parsed = new _Value(append, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Добавить флаг если значение check == true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="append"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static T AddIfTrue<T>(this Enum value, T append, bool? check)
        {
            if (check == true)
            {
                return value.Add(append);
            }

            return (T)Enum.Parse(value.GetType(), value.ToString()); ;
        }

        /// <summary>
        /// Удалить флаг
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static T Remove<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            var parsed = new _Value(remove, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) & ~(long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) & ~(ulong)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Преобразовать enum в список кортежей ключ/значение
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<CommonTuple> ToList<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(p => new CommonTuple { ValueField = Convert.ToInt32(p), DisplayField = GetDisplayShortName(p)});
        }

        /// <summary>
        /// Преобразовать enum в int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt32(this Enum value)
        {
            return Convert.ToInt32(value);
        }
        
        /// <summary>
        /// Получить из типа отображаемые имена с помощью атрибута DisplayAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayShortName<T>(this T value) where T : struct
        {
            var type = typeof(T);
            var memInfo = type.GetMember(value.ToString());
            if (!memInfo.Any())
            {
                return string.Empty;
            }
            var attributes = memInfo.First().GetCustomAttributes(typeof(DisplayAttribute), false);
            if (!attributes.Any())
            {
                return string.Empty;
            }

            return ((DisplayAttribute)attributes.First()).ShortName;
        }

        #region Helper Classes
        //class to simplfy narrowing values between 
        //a ulong and long since either value should
        //cover any lesser value
        private class _Value
        {

            //cached comparisons for tye to use
            private static Type _UInt64 = typeof(ulong);
            private static Type _UInt32 = typeof(long);

            public long? Signed;
            public ulong? Unsigned;

            public _Value(object value, Type type)
            {

                //make sure it is even an enum to work with
                if (!type.IsEnum)
                {
                    throw new
            ArgumentException("Value provided is not an enumerated type!");
                }

                //then check for the enumerated value
                Type compare = Enum.GetUnderlyingType(type);

                //if this is an unsigned long then the only
                //value that can hold it would be a ulong
                if (compare.Equals(_Value._UInt32) || compare.Equals(_Value._UInt64))
                {
                    this.Unsigned = Convert.ToUInt64(value);
                }
                //otherwise, a long should cover anything else
                else
                {
                    this.Signed = Convert.ToInt64(value);
                }

            }
        #endregion

        }

    }
}
