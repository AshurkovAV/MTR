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
    public class RefusalTypeToImageConverter : IValueConverter
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


            var model = (RefusalType)value;
            if (model.IsNull())
            {
                return null;
            }

            if (model == RefusalType.MEC || model == RefusalType.ExternalMEC)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/mec22.png");
            }

            if (Constants.Mee.Contains((int?)model) || model == RefusalType.ExternalMEE)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/mee22.png");
            }

            if (Constants.Eqma.Contains((int?)model) || model == RefusalType.ExternalEQMA)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/eqma22.png");
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

