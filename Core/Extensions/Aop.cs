using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Extensions
{
    /// <summary>
    /// АОП extension
    /// </summary>
    public static class Aop
    {
        // Dictionary to hold type initialization methods' cache 
        static private Dictionary<Type, Action<Object>> _typesInitializers = new Dictionary<Type, Action<Object>>();

        /// <summary>
        /// Реализация прекомпиленных сеттеров с встроенными постоянными значениями из DefaultValueAttributes
        /// </summary>
        public static void ApplyDefaultValues(this Object _this)
        {
            Action<Object> setter = null;

            // Attempt to get it from cache
            if (!_typesInitializers.TryGetValue(_this.GetType(), out setter))
            {
                // If no initializers are added do nothing
                setter = (o) => { };

                // Iterate through each property
                ParameterExpression objectTypeParam = Expression.Parameter(typeof(object), "this");
                foreach (PropertyInfo prop in _this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Expression dva;

                    // Skip read only properties
                    if (!prop.CanWrite)
                        continue;

                    // There are no more then one attribute of this type
                    DefaultValueAttribute[] attr = prop.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

                    // Skip properties with no DefaultValueAttribute
                    if ((null == attr) || (attr.Length == 0))
                        continue;

                    // Build the Lambda expression
#if DEBUG
                    // Make sure types do match
                    try
                    {
                        dva = Expression.Convert(Expression.Constant(attr[0].Value), prop.PropertyType);
                    }
                    catch (InvalidOperationException e)
                    {
                        string error = String.Format("Type of DefaultValueAttribute({3}{0}{3}) does not match type of property {1}.{2}",
                            attr[0].Value.ToString(), _this.GetType().Name, prop.Name, ((typeof(string) == attr[0].Value.GetType()) ? "\"" : ""));

                        throw (new InvalidOperationException(error, e));
                    }
#else
                    dva = Expression.Convert(Expression.Constant(attr[0].Value), prop.PropertyType);
#endif
                    Expression setExpression = Expression.Call(Expression.TypeAs(objectTypeParam, _this.GetType()), prop.GetSetMethod(), dva);
                    Expression<Action<Object>> setLambda = Expression.Lambda<Action<Object>>(setExpression, objectTypeParam);

                    // Add this action to multicast delegate
                    setter += setLambda.Compile();
                }

                // Save in the type cache
                _typesInitializers.Add(_this.GetType(), setter);
            }

            // Initialize member properties
            setter(_this);
        }

        /// <summary>
        /// Реализация кеша делегатов ResetValue 
        /// </summary>
        public static void ResetDefaultValues(this Object _this)
        {
            Action<Object> setter = null;

            // Attempt to get it from cache
            if (!_typesInitializers.TryGetValue(_this.GetType(), out setter))
            {
                // Init delegate with empty body,
                // If no initializers are added do nothing
                setter = (o) => { };

                // Go throu each property and compile Reset delegates
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(_this))
                {
                    // Add only these which values can be reset
                    if (prop.CanResetValue(_this))
                        setter += prop.ResetValue;
                }

                // Save in the type cache
                _typesInitializers.Add(_this.GetType(), setter);
            }

            // Initialize member properties
            setter(_this);
        }
    }
}
