using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Core.Extensions
{
    public static class ClassExtensions
    {
        /// <summary>
        /// Вызвать исключение если агумент == null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T ThrowIfArgumentIsNull<T>(this T obj, string text = null) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(text + " not allowed to be null");
            }
            return obj;
        }

        /// <summary>
        /// Вызвать исключение если агумент при преобразовании в dynamic == null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T ThrowIfArgumentIsNullAsDynamic<T>(this T obj, string text = null) where T : class
        {
            if ((obj as dynamic) == null)
            {
                throw new ArgumentNullException(text + " not allowed to be null as dynamic");
            }
            return obj;
        }

        /// <summary>
        /// Получить пользовательские атрибуты типа Т
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this Type type)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            return attrs.OfType<T>().ToList();
        }

        /// <summary>
        ///  Получить пользовательские атрибуты PropertyInfo (Выявляет атрибуты свойства и обеспечивает доступ к его метаданным.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this PropertyInfo info)
        {
            var attrs = info.GetCustomAttributes(true);
            return attrs.OfType<T>().ToList();
        }
    }
}
