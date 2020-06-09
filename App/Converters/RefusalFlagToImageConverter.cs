using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Core;
using Core.Extensions;
using Core.Helpers;
using Medical.AppLayer.Resources;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class RefusalFlagToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
            {
                return null;
            }

            if (value.IsNull() || value.ToString().IsNullOrWhiteSpace())
            {
                return null; 
            }


            var model = (RefusalStatusFlag)value;
            if (model.IsNull())
            {
                return null;
            }

            if (model.HasFlag(RefusalStatusFlag.Lock))
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/lock22.png");
            }

            if (model.HasFlag(RefusalStatusFlag.Apply))
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/dialog-apply22.png");
            }

            if (model.HasFlag(RefusalStatusFlag.Dismiss))
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/dialog-block22.png");
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

