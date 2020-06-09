using System;
using System.Globalization;
using System.Windows.Data;

namespace Medical.AppLayer.Converters
{
    class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleResult;
            if (double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out doubleResult))
            {
                return doubleResult / 2;
            }
            int intResult;
            if (int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out intResult))
            {
                return intResult / 2;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
