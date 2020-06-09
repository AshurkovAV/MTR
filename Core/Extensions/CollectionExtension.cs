using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Core.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Преобразовать перечисляемый тип Т в строку значений с разделителем (; по умолчанию)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string AggregateToString<T>(this IEnumerable<T> list, string delimiter = ";")
        {
            if (list == null)
            {
                return null;
            }

            var enumerable = list as IList<T> ?? list.ToList();
            if (!enumerable.Any())
            {
                return null;
            }

            if (enumerable.Count() == 1)
            {
                return enumerable.FirstOrDefault().ToString();
            }
            
            return enumerable.Aggregate("",(current, next) => current.ToString(CultureInfo.InvariantCulture) + delimiter + next.ToString());
        }

        /// <summary>
        /// Преобразовать перечисляемый строковый тип в строку значений с разделителем (; по умолчанию)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string AggregateToString(this IEnumerable<string> list, string delimiter = ";")
        {
            if (list == null)
            {
                return null;
            }

            if (!list.Any())
            {
                return null;
            }

            if (list.Count() == 1)
            {
                return list.FirstOrDefault();
            }

            return list.Aggregate((current, next) => current.ToString(CultureInfo.InvariantCulture) + delimiter + next.ToString());
        }

        /// <summary>
        /// Выполнить action для всех элементов входной последовательности
        /// </summary>
        public static void ForEachAction<T>(this IEnumerable<T> input, Action<T> action)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            foreach (T element in input)
            {
                action(element);
            }
        }

        /// <summary>
        /// Добавить все elements в list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="elements"></param>
        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
        {
            try
            {
                foreach (T o in elements)
                    list.Add(o);
            }
            catch (Exception ex)
            {
            }
            
        }

        /// <summary>
        /// Преобразовать редактируемый список в readonly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> arr)
        {
            return new ReadOnlyCollection<T>(arr);
        }

        /// <summary>
        /// Преобразовать рекурсивный тип данных в "плоский" список
        /// Converts a recursive data structure into a flat list.
        /// </summary>
        /// <param name="input">The root elements of the recursive data structure.</param>
        /// <param name="recursion">The function that gets the children of an element.</param>
        /// <returns>Iterator that enumerates the tree structure in preorder.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
        {
            var stack = new Stack<IEnumerator<T>>();
            try
            {
                stack.Push(input.GetEnumerator());
                while (stack.Count > 0)
                {
                    while (stack.Peek().MoveNext())
                    {
                        T element = stack.Peek().Current;
                        yield return element;
                        IEnumerable<T> children = recursion(element);
                        if (children != null)
                        {
                            stack.Push(children.GetEnumerator());
                        }
                    }
                    stack.Pop().Dispose();
                }
            }
            finally
            {
                while (stack.Count > 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }

        /// <summary>
        /// Создать массив содержащий подмножество элементов (тоже что и string.Substring).
        /// </summary>
        
        public static T[] Splice<T>(this T[] array, int startIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            return Splice(array, startIndex, array.Length - startIndex);
        }

        /// <summary>
        /// Создать массив содержащий подмножество элементов (тоже что и string.Substring).
        /// </summary>
        public static T[] Splice<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException("startIndex", startIndex,
                                                      "Value must be between 0 and " + array.Length);
            if (length < 0 || length > array.Length - startIndex)
                throw new ArgumentOutOfRangeException("length", length,
                                                      "Value must be between 0 and " + (array.Length - startIndex));
            var result = new T[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }

        /// <summary>
        /// Получить индекс первого элемента удовлетворяющего значению предиката
        /// если не найден ни один подходящий элемент, то -1
        /// </summary>
        public static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Добавляет элемент в список в случае если элемент != null
        /// </summary>
        public static void AddIfNotNull<T>(this IList<T> list, T itemToAdd) where T : class
        {
            if (itemToAdd != null)
                list.Add(itemToAdd);
        }

        /// <summary>
        /// Возвращает список без дублей согласно функции keySelector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="this"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> @this, Func<T, TKey> keySelector)
        {
            return @this.GroupBy(keySelector).Select(grps => grps).Select(e => e.First());
        }

        /// <summary>
        /// Преобразует список битовых флагов в соответствующее перечисление
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T ToEnumFlag<T>(this IEnumerable<object> list) where T : struct, IConvertible
        {
            int? result = 0;

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (list.IsNull())
            {
                return (T)(object)result.Value;
            }

            foreach (var p in list)
            {
                var i = p.ToInt32Nullable();
                if (i.HasValue)
                {
                    result |= i;
                }
            }

            return (T)(object)result.Value;
        }

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            return source.SelectMany(e => Traverse(e, childrenSelector));
        }

        public static IEnumerable<T> Traverse<T>(T item, Func<T, IEnumerable<T>> childrenSelector)
        {
            yield return item;
            foreach (var subItem in childrenSelector(item).Traverse(childrenSelector))
            {
                yield return subItem;
            }
        }
    }
}
