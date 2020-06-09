using System;
using System.Linq;

namespace Core.Utils
{
    internal static class StringUtil
    {
        public static string Pascal(string input)
        {
            string result;
            if (string.IsNullOrEmpty(input))
            {
                result = input;
            }
            else
            {
                result = char.ToUpper(input[0]) + input.Substring(1);
            }
            return result;
        }

        public static string GetDistinguishingIdentifier(string name, string parentTableName, string parentKeyName,
                                                         bool stripStem)
        {
            name = StripTrailingKey(name, parentKeyName, stripStem);
            parentTableName = parentTableName.ToLowerInvariant();
            string text = name.ToLowerInvariant();
            string result;
            if (text.StartsWith(parentTableName))
            {
                name = name.Substring(parentTableName.Length);
                if (name.StartsWith("_"))
                {
                    name = name.Substring(1);
                }
                result = name;
            }
            else
            {
                result = StripTrailingKey(name, parentTableName, stripStem);
            }
            return result;
        }

        public static string GetSingularParentName(string s, string childPropName)
        {
            string result;
            if (!s.EndsWith("s", StringComparison.Ordinal))
            {
                result = null;
            }
            else
            {
                if (GetPluralName(childPropName).ToLowerInvariant() == s.ToLowerInvariant())
                {
                    result = childPropName;
                }
                else
                {
                    if (childPropName.Length < s.Length || childPropName.Length > s.Length + 5)
                    {
                        result = null;
                    }
                    else
                    {
                        string text = childPropName.ToLowerInvariant();
                        string text2;
                        if (text.EndsWith("_key"))
                        {
                            text2 = childPropName.Substring(0, childPropName.Length - 4);
                        }
                        else
                        {
                            if (text.EndsWith("_id") || text.EndsWith("key"))
                            {
                                text2 = childPropName.Substring(0, childPropName.Length - 3);
                            }
                            else
                            {
                                if (!text.EndsWith("id"))
                                {
                                    result = null;
                                    return result;
                                }
                                text2 = childPropName.Substring(0, childPropName.Length - 2);
                            }
                        }
                        if (text2.Length < 2)
                        {
                            result = null;
                        }
                        else
                        {
                            string a = GetPluralName(text2).ToLowerInvariant();
                            if (a != text2.ToLowerInvariant() && a == s.ToLowerInvariant())
                            {
                                result = text2;
                            }
                            else
                            {
                                result = null;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static string StripTrailingKey(string s, string key, bool stripStem)
        {
            string text = s.ToLowerInvariant();
            string result;
            if (stripStem && key != null && text.EndsWith(key.ToLowerInvariant()))
            {
                s = s.Substring(0, s.Length - key.Length);
            }
            else
            {
                if (text.EndsWith("_ref"))
                {
                    s = s.Substring(0, s.Length - 4);
                }
                else
                {
                    if (text.EndsWith("_id") || text.EndsWith("_key") || s.EndsWith("Key") || s.EndsWith("Ref"))
                    {
                        s = s.Substring(0, s.Length - 3);
                    }
                    else
                    {
                        if (!s.EndsWith("ID") || s.Length <= 4 || !char.IsLower(s, s.Length - 3))
                        {
                            result = s;
                            return result;
                        }
                        s = s.Substring(0, s.Length - 2);
                    }
                }
            }
            while (s.EndsWith("_"))
            {
                s = s.Substring(0, s.Length - 1);
            }
            result = s;
            return result;
        }

        public static string StripTrailingWord(string s, int wordLen)
        {
            string result;
            if (s == null || s.Length < wordLen + 2)
            {
                result = s;
            }
            else
            {
                result = s.Substring(0, s.Length - ((s[s.Length - wordLen - 1] == '_') ? (wordLen + 1) : wordLen));
            }
            return result;
        }

        public static string GetPluralName(string name)
        {
            int num = name.IndexOf("Of", StringComparison.Ordinal);
            string result;
            if (num > 3 && num < name.Length - 4 && char.IsLower(name, num - 1) && name[num - 1] != 's' &&
                char.IsUpper(name, num + 2))
            {
                result = name.Substring(0, num) + "s" + name.Substring(num);
            }
            else
            {
                string str = "";
                string text = name.ToLowerInvariant();
                if (text.EndsWith("created", StringComparison.Ordinal) ||
                    name.EndsWith("updated", StringComparison.Ordinal))
                {
                    if (name.Length < 10)
                    {
                        result = name;
                        return result;
                    }
                    string a = text.Substring(name.Length - 9, 2);
                    string a2 = text.Substring(name.Length - 10, 3);
                    if (a == "co" || a == "re" || a == "un")
                    {
                        result = name;
                        return result;
                    }
                    if (a2 == "mis" || a2 == "pro")
                    {
                        result = name;
                        return result;
                    }
                    str = name.Substring(name.Length - 7);
                    name = name.Substring(0, name.Length - 7);
                }
                text = name.ToLowerInvariant();
                if (text.EndsWith("information") || text.EndsWith("complete") || text.EndsWith("_info") ||
                    text.EndsWith("_data") || name.EndsWith("Info") || name.EndsWith("Data") || name.EndsWith("Staff"))
                {
                    result = name + str;
                }
                else
                {
                    if (text.EndsWith("x") || text.EndsWith("ch") || text.EndsWith("ss") || text.EndsWith("status"))
                    {
                        name += "es";
                    }
                    else
                    {
                        if (text.EndsWith("y") && text.Length > 1 && !IsVowel(text[text.Length - 2]))
                        {
                            name = name.Substring(0, name.Length - 1) + "ies";
                        }
                        else
                        {
                            if (!text.EndsWith("s"))
                            {
                                name += "s";
                            }
                        }
                    }
                    result = name + str;
                }
            }
            return result;
        }

        private static bool IsVowel(char c)
        {
            return "aeiuo".Contains(char.ToLowerInvariant(c));
        }

        private static bool IsVowelRu(char c)
        {
            return "аеёиоуыэюя".Contains(char.ToLowerInvariant(c));
        }
    }
}