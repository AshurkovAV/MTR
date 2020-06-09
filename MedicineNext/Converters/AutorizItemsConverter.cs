using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MedicineNext.Converters
{
    class AutorizItemsConverter : MarkupExtension, IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var result = (bool) value != true;
            return result;
        }
       
    }
}
