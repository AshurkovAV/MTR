using System;

namespace Core.Extensions
{
    public static class DateTimeExtensions
    {
        public const double MsInMinute = 60 * 1000;
        public const double MsInHour = 60 * MsInMinute;
        public const double MsInDay = 24 * MsInHour;

        public static string ToFormatString(this DateTime? value, string format = "yyyy-MM-dd", string defaultValue = null)
        {
            return value.HasValue ? value.Value.ToString(format) : defaultValue;
        }

        /// <summary>
        /// Elapseds the time.
        /// Истекшее время
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan Elapsed(this DateTime datetime)
        {
            return DateTime.Now - datetime;
        }
    }
}
