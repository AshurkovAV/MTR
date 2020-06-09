using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class ComparerExtensions
    {
        /// <summary>
        /// Comparer с использованием delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        sealed class DelegateComparer<T, TKey> : IEqualityComparer<T>
        {
            static readonly EqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;
            readonly Func<T, TKey> selector;

            public DelegateComparer(Func<T, TKey> selector)
            {
                this.selector = selector;
            }

            public bool Equals(T x, T y)
            {
                return comparer.Equals(this.selector(x), this.selector(y));
            }

            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(this.selector(obj));
            }
        }

        /// <summary>
        /// Получение Comparer с использованием delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        public static IEqualityComparer<T> ComparerFromDelegate<T, TKey>(Func<T, TKey> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            return new DelegateComparer<T, TKey>(selector);
        }

        /// <summary>
        /// Получение списока без дублей с использованием DelegateComparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return Enumerable.Distinct(source, ComparerFromDelegate(selector));
        }
    }
}
