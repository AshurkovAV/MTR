using System;
using System.Windows.Data;
using Core.Helpers;
using Medical.CoreLayer.Resource;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class DirectionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/error.png"); 
            
            var model = SafeConvert.ToInt32(value.ToString());
            if (model == 0)
            {
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/go-up.png");
            }
            return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/go-down.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}

