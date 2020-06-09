using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Core.Helpers
{
    class ConverterHelpers
    {
        public static bool TryConvert<T, U>(T t, out U u)
        {
            try
            {
                u = default(U);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(U));
                if (converter!= null)
                {
                    if (!converter.CanConvertFrom(typeof(T)))
                    {
                        return false;
                    }
                    u = (U)converter.ConvertFrom(t);
                    return true;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is FormatException)
                {
                    u = default(U);
                    return false;
                }

                throw;
            }
            return false;
        }

        public static bool TryParse<T>(string s, out T value)
        {
            return Cache<T>.TryParse(s, out value);
        }
        internal static class Cache<T>
        {
            public static bool TryParse(string s, out T value)
            {
                return func(s, out value);
            }
            delegate bool TryPattern(string s, out T value);
            private static readonly TryPattern func;
            static Cache()
            {
                MethodInfo method = typeof(T).GetMethod(
                    "TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType() });
                if (method == null)
                {
                    func = delegate(string x, out T y) { y = default(T); return false; };
                }
                else
                {
                    func = (TryPattern)Delegate.CreateDelegate(typeof(TryPattern), method);
                }
            }
        }
    }

    public static class CleanConverter
    {
        /// <summary>
        /// Stores the cache of all types that can be converted to all types.
        /// </summary>
        private static Dictionary<Type, Dictionary<Type, ConversionCache>> _Types = new Dictionary<Type, Dictionary<Type, ConversionCache>>();

        /// <summary>
        /// Try parsing.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(IComparable s, ref IComparable value)
        {
            // First get the cached conversion method.
            Dictionary<Type, ConversionCache> type1Cache = null;
            ConversionCache type2Cache = null;

            if (!_Types.ContainsKey(s.GetType()))
            {
                type1Cache = new Dictionary<Type, ConversionCache>();

                _Types.Add(s.GetType(), type1Cache);
            }
            else
            {
                type1Cache = _Types[s.GetType()];
            }

            if (!type1Cache.ContainsKey(value.GetType()))
            {
                // We havent converted this type before, so create a new conversion
                type2Cache = new ConversionCache(s.GetType(), value.GetType());

                // Add to the cache
                type1Cache.Add(value.GetType(), type2Cache);
            }
            else
            {
                type2Cache = type1Cache[value.GetType()];
            }

            // Attempt the parse
            return type2Cache.TryParse(s, ref value);
        }

        /// <summary>
        /// Stores the method to convert from Type1 to Type2
        /// </summary>
        internal class ConversionCache
        {
            internal bool TryParse(IComparable s, ref IComparable value)
            {
                if (this._Method != null)
                {
                    // Invoke the cached TryParse method.
                    object[] parameters = new object[] { s, value };
                    bool result = (bool)this._Method.Invoke(null, parameters);

                    if (result)
                        value = parameters[1] as IComparable;

                    return result;
                }
                else
                    return false;

            }

            private MethodInfo _Method;
            internal ConversionCache(Type type1, Type type2)
            {
                // Use reflection to get the TryParse method from it.
                this._Method = type2.GetMethod("TryParse", new Type[] { type1, type2.MakeByRefType() });
            }
        }
    }
}
