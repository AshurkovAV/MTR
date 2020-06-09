using System;
using System.Windows.Data;
using Core;
using Core.Extensions;
using Core.Helpers;
using Medical.AppLayer.Resources;
using Medical.CoreLayer.Resource;

namespace Medical.AppLayer.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class RefusalSourceToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.IsNull() || value.ToString().IsNullOrWhiteSpace())
            {
                return null; 
            }


            var model = (RefusalSource)value;
            if (model.IsNull())
            {
                return null;
            }

            if (model.HasFlag(RefusalSource.InterTerritorial) || model.HasFlag(RefusalSource.InterTerritorialTotal))
            {
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/go-up.png");
            }

            if (model.HasFlag(RefusalSource.Local) || model.HasFlag(RefusalSource.LocalCorrected))
            {
                return ImageSourceHelper.GetImageSource<IResourceCatalog>("/Resource/go-down.png");
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

