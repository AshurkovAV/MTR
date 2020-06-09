using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Extensions
{
    public static class Linq
    {
        /// <summary>
        /// Проверка является ли тип перечеслимым (IEnumerable)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIEnumerable(this Type type)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return type.IsGenericType
                && genericTypeDefinition == typeof(IEnumerable<>);
        }
        public static MethodBase GetGenericMethod(this Type type, string name, Type[] typeArgs, Type[] argTypes, BindingFlags flags)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        public static Type GetIEnumerableImpl(this Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsIEnumerable(type))
                return type;
            Type[] t = type.FindInterfaces((m, o) => IsIEnumerable(m), null);
            Debug.Assert(t.Length == 1);
            return t[0];
        }

        public static Expression CallAny(Expression collection, Delegate predicate)
        {
            Type cType = collection.Type.GetIEnumerableImpl();
            collection = Expression.Convert(collection, cType);

            Type elemType = cType.GetGenericArguments()[0];
            Type predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));

            // Enumerable.Any<T>(IEnumerable<T>, Func<T,bool>)
            MethodInfo anyMethod = (MethodInfo)
                GetGenericMethod(typeof(Enumerable), "Any", new[] { elemType },
                    new[] { cType, predType }, BindingFlags.Static);

            return Expression.Call(
                anyMethod,
                    collection,
                    Expression.Constant(predicate));
        }
    }

    
}
