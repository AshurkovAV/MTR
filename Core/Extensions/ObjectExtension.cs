using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Core.Infrastructure;

namespace Core.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Проверка значения == null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNull(this object source)
        {
          return source == null;
        }

        /// <summary>
        /// Проверка значения != null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object source)
        {
            return source != null;
        }

        /// <summary>
        /// Проверка значения после приведения к dynamic == null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullAsDynamic(this object source)
        {
            return (source as dynamic) == null;
        }

        /// <summary>
        /// if the object this method is called on is not null, runs the given function and returns the value.
        /// if the object is null, returns default(TResult)
        /// </summary>
        public static TResult IfNotNull<T, TResult>(this T target, Func<T, TResult> getValue)
        {
            if (target != null)
                return getValue(target);
            else
                return default(TResult);
        }

        /// <summary>
        /// Возвращает null
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static object ToNull(this object o)
        {
            return null;
        }

        /// <summary>
        /// Преобразование объекта к int?, в случае неудачи возвращает значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ToInt32Nullable(this object source, int? defaultValue = null)
        {
            if (source.IsNull())
            {
                return defaultValue;
            }

            int result;
            if (int.TryParse(source.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование объекта к int, в случае неудачи возвращает значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32(this object source, int defaultValue = 0)
        {
            if (source.IsNull())
            {
                return defaultValue;
            }

            int result;
            if (int.TryParse(source.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование объекта к строке, в случае если объект null возвращает пустую строку
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToNullableString<T>(this Nullable<T> param) where T : struct
        {
            if (param.HasValue)
            {
                return param.Value.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Преобразование объекта к типу decimal, в случае неудачи возвращает значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object source, decimal defaultValue = 0m)
        {
            if (source.IsNull())
            {
                return defaultValue;
            }

            decimal result;
            var s = source.ToString().Replace(",", ".");
            if (decimal.TryParse(s, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование объекта к типу DateTime?, в случае неудачи возвращает значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeNullable(this object source, DateTime? defaultValue = null)
        {
            DateTime result;
            if (DateTime.TryParse(source.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование объекта к типу DateTime?, в случае неудачи возвращает значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeNullableExactFormat(this object source, string format, DateTime? defaultValue = null)
        {
            DateTime result;
            if (DateTime.TryParseExact(source.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование объекта к типу bool
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ToBool(this object source)
        {
            return Convert.ToBoolean(source);
        }

        /// <summary>
        /// Копирование значений свойств объекта src объекту dest, если propertyNames не равен null, копируются свойства с именами из этого списка
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static MappingResult<T> MapObject<T>(this object src, object dest, IEnumerable<string> propertyNames = null)
        {
            var result = new MappingResult<T>();

            var srcType = src.GetType();
            var propertiesSrc = srcType.GetProperties();

            var destType = dest.GetType();
            var propertiesDest = destType.GetProperties();

            foreach (var piSrc in propertiesSrc)
            {
                if (propertyNames != null && !propertyNames.Contains(piSrc.Name))
                {
                    continue;
                }

                if (!piSrc.CanWrite)
                {
                    continue;
                }

                var val = piSrc.GetValue(src, null);
                var piDest = propertiesDest.FirstOrDefault(r => r.Name == piSrc.Name);
                if (piDest != null)
                {
                    var oldVal = piDest.GetValue(dest, null);

                    if (oldVal == null || val == null || (oldVal.ToString() != val.ToString()))
                    {
                        piDest.SetValue(dest, val, null);
                        result.Affected.Add(piSrc.Name);
                    }

                }
            }

            result.Data = (T)dest;
            return result;
        }

        /// <summary>
        /// Преобразование объекта в последовательность байт
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this object data)
        {
            if (data == null)
            {
                return null;
            }
                
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, data);
            return ms.ToArray();
        }

        /// <summary>
        /// Проверка объявен ли в объекта метод и именем methodName
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static bool HasMethod(this object obj, string methodName)
        {
            if (obj.IsNull())
            {
                return false;
            }
            var type = obj.GetType();
            return type.GetMethod(methodName) != null;
        }

        /// <summary>
        /// Получить из объекта метод с именем methodName и вызвать его с параметрами parameters
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public static void GetMethodAndInvoke(this object obj, string methodName, object[] parameters = null)
        {
            if (obj.IsNull())
            {
                return;
            }
            var type = obj.GetType();
            var method = type.GetMethod(methodName);
            if (method.IsNull())
            {
                return;
            }

            method.Invoke(obj, parameters ?? new object[] {});
        }

        /// <summary>
        /// Проверка есть ли в объекте свойство с именем propertyName
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this object obj, string propertyName)
        {
            if (obj.IsNull())
            {
                return false;
            }
            var type = obj.GetType();
            return type.GetProperty(propertyName) != null;
        }
    }
}
