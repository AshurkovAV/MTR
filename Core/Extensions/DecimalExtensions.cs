using System.Globalization;

namespace Core.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToF8(this decimal? value)
        {
            return value.HasValue ? value.Value.ToString("F8", CultureInfo.InvariantCulture) : null;
        }
        public static string ToF2(this decimal? value)
        {
            return value.HasValue ? value.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
        }

        public static string ToF1(this decimal? value)
        {
            return value.HasValue ? value.Value.ToString("F1", CultureInfo.InvariantCulture) : null;
        }

        /// <summary>
        /// The numbers percentage
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>The result</returns>
        public static decimal PercentageOf(this decimal number, int percent)
        {
            return (decimal)(number * percent / 100);
        }

        /// <summary>
        /// Percentage of the number.
        /// </summary>
        /// <param name="percent">The percent</param>
        /// <param name="total">The Number</param>
        /// <returns>The result</returns>
        public static decimal PercentOf(this decimal percent, int total)
        {
            decimal result = 0;
            if (percent > 0 && total > 0)
                result = (decimal)percent / (decimal)total * 100;
            return result;
        }
    }
}
