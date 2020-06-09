using System;
using System.Globalization;
using System.Windows.Data;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// Конвертирует double величину в 3/4 величины (для высплывающих окон)
    /// </summary>
    class ThreeFourthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleResult;
            if (double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out doubleResult))
            {
                return doubleResult / 4 * 3;
            }
            int intResult;
            if (int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out intResult))
            {
                return intResult / 4 * 3;
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
