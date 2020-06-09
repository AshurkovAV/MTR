using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Utils
{
    public static class TypeUtil
    {
        public static bool IsNumeric(Type t)
        {
            bool result;
            if (t == null)
            {
                result = false;
            }
            else
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    t = t.GetGenericArguments()[0];
                }
                result = (t == typeof (decimal) || (t.IsPrimitive && t != typeof (char) && t != typeof (bool)));
            }
            return result;
        }

        public static string FormatTypeName(Type t)
        {
            return FormatTypeName(t, false);
        }

        public static string FormatTypeName(Type t, bool fullname)
        {
            return FormatTypeName(t, fullname, false, 0, new HashSet<Type>());
        }

        private static string FormatTypeName(Type t, bool fullname, bool emptyAnon, int nestLevel,
                                             HashSet<Type> visitedTypes)
        {
            string result;
            if (t == null)
            {
                result = "";
            }
            else
            {
                if (t.IsByRef)
                {
                    t = t.GetElementType();
                }
                if (!t.IsPublic)
                {
                    Type type = (
                                    from i in t.GetInterfaces()
                                    where i.IsPublic
                                    where
                                        i.Namespace.StartsWith("System.Collections", StringComparison.Ordinal) ||
                                        i.Namespace.StartsWith("System.Linq", StringComparison.Ordinal)
                                    select i).OrderByDescending(itype => itype, new SubTypeComparer()).
                        FirstOrDefault();
                    if (type != null)
                    {
                        t = type;
                    }
                }
                if ((!t.IsGenericType && !t.IsArray) || nestLevel > 5 || visitedTypes.Contains(t))
                {
                    result = (fullname ? t.FullName : t.Name);
                }
                else
                {
                    visitedTypes.Add(t);
                    var stringBuilder = new StringBuilder();
                    string text = "";
                    string str = "";
                    string value;
                    if (t.IsGenericType)
                    {
                        if (t.GetGenericTypeDefinition() == typeof (Nullable<>))
                        {
                            value = "?";
                        }
                        else
                        {
                            text = t.GetGenericTypeDefinition().Name.Split(new[]
                                                                               {
                                                                                   '`'
                                                                               })[0];
                            if (text.Contains("<"))
                            {
                                if (!fullname)
                                {
                                    result = (emptyAnon ? "" : "ø");
                                    return result;
                                }
                                text = "";
                                str = "{";
                                value = "}";
                            }
                            else
                            {
                                str = "<";
                                value = ">";
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            value = "[".PadRight(t.GetArrayRank() - 1, ',') + "]";
                        }
                        catch
                        {
                            result = "";
                            return result;
                        }
                    }
                    stringBuilder.Append(text + str);
                    if (t.IsGenericType)
                    {
                        bool flag = true;
                        Type[] genericArguments = t.GetGenericArguments();
                        foreach (Type t2 in genericArguments)
                        {
                            if (flag)
                            {
                                flag = false;
                            }
                            else
                            {
                                stringBuilder.Append(',');
                            }
                            stringBuilder.Append(FormatTypeName(t2, fullname, true, nestLevel + 1, visitedTypes));
                        }
                    }
                    else
                    {
                        try
                        {
                            stringBuilder.AppendLine(FormatTypeName(t.GetElementType(), fullname, false, nestLevel + 1,
                                                                    visitedTypes));
                        }
                        catch
                        {
                            result = "";
                            return result;
                        }
                    }
                    stringBuilder.Append(value);
                    result = stringBuilder.ToString();
                }
            }
            return result;
        }
    }
}