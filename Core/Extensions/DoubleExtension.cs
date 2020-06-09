using System;

namespace Core.Extensions
{
    public static class DoubleExtension
    {
        public static string FormatTimeInterval(this double ms)
        {
            var days = (int)Math.Floor(ms / DateTimeExtensions.MsInDay);
            var hours = (int)Math.Floor((ms - days * DateTimeExtensions.MsInDay) / DateTimeExtensions.MsInHour);
            var minutes = (int)Math.Floor((ms % DateTimeExtensions.MsInHour) / DateTimeExtensions.MsInMinute);
            var seconds = (int)Math.Floor((ms % DateTimeExtensions.MsInMinute) / 1000);

            var res = string.Empty;

            if (hours == 0 && days == 0 && minutes == 0)
            {
                res += " " + (seconds + 1) + "сек";

            }
            else
            {
                if (days > 0)
                    res += "" + days + "д";

                if (hours > 0 || days > 0)
                    res += " " + hours + "ч";

                if (hours > 0 || days > 0 || minutes > 0)
                    res += " " + minutes + "мин";

                if (hours > 0 || days > 0 || minutes > 0 || seconds > 0)
                    res += " " + seconds + "сек";
            }

            return res;
        }
    }
}
