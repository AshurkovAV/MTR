using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Length > 0).ToList();
        }

        public static IEnumerable<T> GetAttributeFromAssembly<T>(Assembly assembly)
        {
            return assembly.GetTypes().Select(Attribute.GetCustomAttributes).OfType<T>().ToList();
        }
    }
}
