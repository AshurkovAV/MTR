using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Core.Helpers;
using Medical.CoreLayer.Resource;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class BoolToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            {
                return null;
            }

            if (value == null)
            {
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/dialog-question64.png");
            }
                 
            var result = System.Convert.ToBoolean(value);
            if (result)
            {
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/dialog-apply64.png"); 
            }
            return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/dialog-error64.png"); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}

