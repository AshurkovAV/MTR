using System;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Linq
{
    public static class LinqExtension
    {
        public static IQueryable<TSource> Between<TSource, TKey>
            (this IQueryable<TSource> source,
             Expression<Func<TSource, TKey>> keySelector,
             TKey low, TKey high) where TKey : IComparable<TKey>
        {
            Expression key = Expression.Invoke(keySelector, keySelector.Parameters.ToArray());
            Expression lowerBound = Expression.LessThanOrEqual(Expression.Constant(low), key);
            Expression upperBound = Expression.LessThanOrEqual(key, Expression.Constant(high));
            Expression and = Expression.AndAlso(lowerBound, upperBound);
            Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(and, keySelector.Parameters);
            return source.Where(lambda);
        }

        /*
         * 
         *  if (e.PropertyName == GetPropertyName(() => Customer.FirstName))
            {
              //Do Something
            }
         */
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
    }
}
