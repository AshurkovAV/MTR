using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.Utils;

namespace MedicineNext.Converters {
    public class ImageConverter : IValueConverter {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value != null) {
                var uri = AssemblyHelper.GetResourceUri(typeof(ImageConverter).Assembly, value.ToString());
                return new System.Windows.Media.Imaging.BitmapImage(uri);
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
