using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Core;

namespace Medical.AppLayer.Search.Converters
{
    public class SexEnumConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DevExpress.Mvvm.EnumMemberInfo)
            {
                value = ((DevExpress.Mvvm.EnumMemberInfo)value).Id;
                var type = typeof(SexEnum);
                var memInfo = type.GetMember(value.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if (!attributes.Any())
                    return value;
                return ((DisplayAttribute)attributes[0]).ShortName;
            }
            else return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
