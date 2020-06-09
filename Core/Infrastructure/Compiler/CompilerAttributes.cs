using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Infrastructure.Compiler
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CallPriorityAttribute : Attribute
    {
        private int _priority;

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public CallPriorityAttribute(int priority)
        {
            _priority = priority;
        }
    }

    public class CallPriorityComparer : IComparer<MethodInfo>
    {
        public int Compare(MethodInfo x, MethodInfo y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            return GetPriority(x) - GetPriority(y);
        }

        private int GetPriority(MethodInfo mi)
        {
            object[] objs = mi.GetCustomAttributes(typeof(CallPriorityAttribute), true);

            if (objs == null)
                return 0;

            if (objs.Length == 0)
                return 0;

            CallPriorityAttribute attr = objs[0] as CallPriorityAttribute;

            if (attr == null)
                return 0;

            return attr.Priority;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TypeAliasAttribute : Attribute
    {
        private string[] _aliases;

        public string[] Aliases
        {
            get
            {
                return _aliases;
            }
        }

        public TypeAliasAttribute(params string[] aliases)
        {
            _aliases = aliases;
        }
    }
}
