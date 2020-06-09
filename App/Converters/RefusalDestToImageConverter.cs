using System;
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
    public class RefusalDestToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.IsNull() || value.ToString().IsNullOrWhiteSpace())
            {
                return null; 
            }

            try
            {
                var model = value as RefusalDest? ?? RefusalDest.None;
                if (model.IsNull())
                {
                    return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/help22.png");
                }

                if (model == RefusalDest.None)
                {
                    return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/view-refresh22.png");
                }

                if (model == RefusalDest.Out)
                {
                    return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/go-up22.png");
                }

                if (model == RefusalDest.In)
                {
                    return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/go-down22.png");
                }
            }
            catch (Exception)
            {

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

