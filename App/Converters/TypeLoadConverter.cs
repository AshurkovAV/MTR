using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Core;
using Core.Extensions;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// Конвертер типа загрузки (файл, папка, папка рекурсивно)
    /// </summary>
    class TypeLoadConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is TypeLoad)
            {
                var type = typeof(TypeLoad);
                var memInfo = type.GetMember(value.ToString());
                if (!memInfo.Any())
                {
                    return value;
                }
                var attributes = memInfo.First().GetCustomAttributes(typeof(DisplayAttribute), false);
                if (!attributes.Any())
                {
                    return value;
                }
                   
                return ((DisplayAttribute)attributes.First()).ShortName;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
