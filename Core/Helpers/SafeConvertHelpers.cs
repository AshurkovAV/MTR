using System;
using System.Globalization;

namespace Core.Helpers
{
    public static class SafeConvert
    {
        public static int? ToInt32(string value)
        {
            int result;
            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static int? ToInt32(object value)
        {
            if (value == null)
            {
                return null;
            }

            int result;
            if (int.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static int? ToInt32(string value,bool canBeZero)
        {
            int result;
            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result))
            {
                if (!canBeZero && result == 0)
                    return null;

                return result;
            }
            return null;
        }

        public static decimal? ToDecimal(string value)
        {
            decimal result;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Replace(",", ".");
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static decimal? ToDecimal(string value, bool canBeZero)
        {
            decimal result;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Replace(",", ".");
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
            {
                if (!canBeZero && result == 0)
                    return null;

                return result;
            }
            return null;
        }

        public static double? ToDouble(string value)
        {
            double result;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Replace(",", ".");
            if (double.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static double? ToDouble(string value, bool canBeZero)
        {
            double result;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            value = value.Replace(",", ".");
            if (double.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
            {
                if (!canBeZero && result == 0)
                    return null;

                return result;
            }
            return null;
        }

        public static DateTime? ToDateTimeExact(string value,string format)
        {
            DateTime result;
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }
    }
    class SafeConvertHelpers
    {
    }
}
