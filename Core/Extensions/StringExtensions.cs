using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Проверка строка == null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNull(this string source)
        {
            return source == null;
        }

        /// <summary>
        /// Проверка строка != null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotNull(this string source)
        {
            return !source.IsNull();
        }

        /// <summary>
        /// Проверка строка == null или пустая
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Проверка строка != null и не пустая
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Проверка строка == null или состоит из white space characters
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        ///  Проверка строка != null и не состоит из white space characters
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string source)
        {
            return !string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        /// Преобразование в bool (1 true, другое значение false)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool? ToBoolNullable(this string source, bool? defaultValue = null)
        {
            if (source.IsNotNullOrWhiteSpace())
            {
                return string.Equals(source.Trim(), "1");
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Преобразование строки в int, если преобразование невозможно, то возвращается значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32(this string source, int defaultValue = 0)
        {
            int result;
            if (int.TryParse(source, NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование строки в int?, если преобразование невозможно, то возвращается значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ToInt32Nullable(this string source, int? defaultValue = null)
        {
            int result;
            if (int.TryParse(source, NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование строки в decimal?, если преобразование невозможно, то возвращается значение по умолчанию defaultValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal? ToDecimalNullable(this string source, decimal? defaultValue = null)
        {
            decimal result;
            source = source.Replace(",", ".");
            if (decimal.TryParse(source, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Преобразование строки в DateTime?, в точном соответствии с заданным форматом
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeExact(this string source, string format = "yyyy-MM-dd")
        {
            DateTime result;
            if (DateTime.TryParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Преобразование строки в DateTime?, если преобразование невозможно, то возвращается null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string source)
        {
            DateTime result;
            if (DateTime.TryParse(source, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Короткая версия string.Format
        /// </summary>
        /// <param name="s">строка форматирования</param>
        /// <param name="args">аргументы</param>
        /// <returns></returns>
        public static string F(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        /// <summary>
        /// Усекает строку до указанной длины и добавляет ...
        /// </summary>
        /// <param name="value">строка</param>
        /// <param name="maxLength">длина строки</param>
        /// <returns>усеченная строка</returns>
        public static string Truncate(this string text, int maxLength)
        {
            // replaces the truncated string to a ...
            const string suffix = "...";
            string truncatedString = text;

            if (maxLength <= 0) return truncatedString;
            int strLength = maxLength - suffix.Length;

            if (strLength <= 0) return truncatedString;

            if (text == null || text.Length <= maxLength) return truncatedString;

            truncatedString = text.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;
            return truncatedString;
        }

        /// <summary>
        /// аналог string.Trim null безопасный
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimNullable(this string value)
        {
            return value.IsNullOrEmpty() ? null : value.Trim();
        }

        /// <summary>
        /// Заменить в строке вхождения из списка list на ""
        /// </summary>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ReplaceByList(this string value, IEnumerable<string> list)
        {
            return value.IsNullOrEmpty() ? null : list.Aggregate(value, (current, pattern) => current.Replace(pattern, ""));
        }

        /// <summary>
        /// Преобразование строки в список int с использованием в качестве разделителя delimiter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static List<int> ToInt32List(this string source, string delimiter = ";")
        {
            if (source.IsNullOrWhiteSpace())
            {
                return new List<int>();
            }
            var parsed = source.Split(new string[] { delimiter },StringSplitOptions.RemoveEmptyEntries);
            if (!parsed.Any())
            {
                return new List<int>();
            }

            var result = new List<int>();

            parsed.ForEachAction(s =>
            {
                var i = s.ToInt32Nullable();
                if (i.HasValue && i.Value > 0)
                {
                    result.Add(i.Value);
                }
            });

            return result;
        }

        /// <summary>
        /// Преобразование строки в список строк с использованием в качестве разделителя delimiter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static List<string> ToListByDelimiter(this string source, string delimiter = ";")
        {
            if (source.IsNullOrWhiteSpace())
            {
                return new List<string>();
            }
            var parsed = source.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            if (!parsed.Any())
            {
                return new List<string>(); ;
            }

            var result = new List<string>();

            parsed.ForEachAction(s =>
            {
                
                if (s.IsNotNullOrWhiteSpace())
                {
                    result.Add(s);
                }
            });

            return result;
        }

        /// <summary>
        /// Removes <param name="stringToRemove" /> from the start of this string.
        /// Throws ArgumentException if this string does not start with <param name="stringToRemove" />.
        /// </summary>
        public static string RemoveStart(this string s, string stringToRemove)
        {
            if (s == null)
                return null;
            if (string.IsNullOrEmpty(stringToRemove))
                return s;
            if (!s.StartsWith(stringToRemove))
                throw new ArgumentException(string.Format("{0} does not start with {1}", s, stringToRemove));
            return s.Substring(stringToRemove.Length);
        }

        /// <summary>
        /// Removes <param name="stringToRemove" /> from the end of this string.
        /// Throws ArgumentException if this string does not end with <param name="stringToRemove" />.
        /// </summary>
        public static string RemoveEnd(this string s, string stringToRemove)
        {
            if (s == null)
                return null;
            if (string.IsNullOrEmpty(stringToRemove))
                return s;
            if (!s.EndsWith(stringToRemove))
                throw new ArgumentException(string.Format("{0} does not end with {1}", s, stringToRemove));
            return s.Substring(0, s.Length - stringToRemove.Length);
        }

        /// <summary>
        /// Takes at most <param name="length" /> first characters from string. 
        /// String can be null.
        /// </summary>
        public static string TakeStart(this string s, int length)
        {
            if (string.IsNullOrEmpty(s) || length >= s.Length)
                return s;
            return s.Substring(0, length);
        }

        /// <summary>
        /// Takes at most <param name="length" /> first characters from string, and appends '...' if string is longer. 
        /// String can be null.
        /// </summary>
        public static string TakeStartEllipsis(this string s, int length)
        {
            if (string.IsNullOrEmpty(s) || length >= s.Length)
                return s;
            return s.Substring(0, length) + "...";
        }

        /// <summary>
        /// Заменить в строке original согласно петтерну pattern значением replacement с заданныс способом сравнения comparisonType
        /// </summary>
        /// <param name="original"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
        {
            if (original == null)
                throw new ArgumentNullException("original");
            if (pattern == null)
                throw new ArgumentNullException("pattern");
            if (pattern.Length == 0)
                throw new ArgumentException("String cannot be of zero length.", "pattern");
            if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
                throw new NotSupportedException("Currently only ordinal comparisons are implemented.");

            var result = new StringBuilder(original.Length);
            int currentPos = 0;
            int nextMatch = original.IndexOf(pattern, comparisonType);
            while (nextMatch >= 0)
            {
                result.Append(original, currentPos, nextMatch - currentPos);
                // The following line restricts this method to ordinal comparisons:
                // for non-ordinal comparisons, the match length might be different than the pattern length.
                currentPos = nextMatch + pattern.Length;
                result.Append(replacement);

                nextMatch = original.IndexOf(pattern, currentPos, comparisonType);
            }

            result.Append(original, currentPos, original.Length - currentPos);
            return result.ToString();
        }

        /// <summary>
        /// Преобразование строки в JObject
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JObject ToJObject(this string value)
        {
            if (value.IsNotNullOrEmpty())
            {
                return JsonConvert.DeserializeObject<JObject>(value);
            }
            return null;
        }

        /// <summary>
        /// Проверка, является ли строка Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsGuid(this string value)
        {
            Guid result;
            return Guid.TryParse(value, out result);
        }

        /// <summary>
        /// Проверка на вхождение с возможностью указать StringComparison
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }


    }
}
