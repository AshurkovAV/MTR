using System;
using System.Globalization;
using System.Windows.Data;

namespace Medical.AppLayer.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            if (value.ToString().Length < 10)
                return null;

            DateTime result;
            if (DateTime.TryParse(value.ToString().Substring(0,10), out result))
            {
                return result.ToString("dd.MM.yyyy");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}