using System;
using System.Windows.Data;
using Core.Helpers;
using Medical.CoreLayer.Resource;

namespace Medical.CoreLayer
{
    public class BoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            var model = System.Convert.ToBoolean(value);
            if (model)
            {
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/apply.png");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
