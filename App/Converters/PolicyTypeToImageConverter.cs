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
    public class PolicyTypeToImageConverter : IValueConverter
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


            var model = (PolicyType)value;
            if (model.IsNull())
            {
                return null;
            }

            if (model == PolicyType.Old)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/user-trash-full22.png");
            }

            if (model == PolicyType.Temporary)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/user-away22.png");
            }

            if (model == PolicyType.INP)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/user-online22.png");
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

