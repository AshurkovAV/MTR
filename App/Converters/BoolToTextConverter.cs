using System;
using System.Windows.Data;
using Core.Helpers;
using Medical.CoreLayer.Resource;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class BoolToTextConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "Неопределено";
            }
                 
            var result = System.Convert.ToBoolean(value);
            if (result)
            {
                return "Действительно"; 
            }
            return "Недействительно"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}

