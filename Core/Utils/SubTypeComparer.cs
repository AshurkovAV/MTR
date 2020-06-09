using System;
using System.Collections.Generic;

namespace Core.Utils
{
    internal class SubTypeComparer : Comparer<Type>
    {
        public override int Compare(Type x, Type y)
        {
            int result;
            if (Equals(x, y))
            {
                result = 0;
            }
            else
            {
                if (x == null)
                {
                    result = -1;
                }
                else
                {
                    if (y == null)
                    {
                        result = 1;
                    }
                    else
                    {
                        int num = y.IsAssignableFrom(x).CompareTo(x.IsAssignableFrom(y));
                        if (num != 0)
                        {
                            result = num;
                        }
                        else
                        {
                            if (x.IsGenericType && !y.IsGenericType)
                            {
                                result = 1;
                            }
                            else
                            {
                                if (y.IsGenericType && !x.IsGenericType)
                                {
                                    result = -1;
                                }
                                else
                                {
                                    result = 0;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}