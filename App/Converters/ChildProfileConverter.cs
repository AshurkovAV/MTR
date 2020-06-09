using System;
using System.Windows.Data;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class ChildProfileConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            int model = System.Convert.ToInt32(value);
            return model != 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;
            bool model = System.Convert.ToBoolean(value);
            return model ? 1 : 0;
        }
        #endregion
    }
}
