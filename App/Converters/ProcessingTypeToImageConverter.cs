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
    public class ProcessingTypeToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.IsNull() || value.ToString().IsNullOrWhiteSpace())
            {
                return null; 
            }


            var model = (ProcessingType)value;
            if (model.IsNull())
            {
                return null;
            }

            if (model == ProcessingType.BuiltIn)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/buildin22.png");
            }

            if (model == ProcessingType.Sql)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/sql22.png");
            }

            if (model == ProcessingType.CSharp)
            {
                return ImageSourceHelper.GetImageSource<IAppLayerResourceCatalog>("/Resources/Icons/csharp22.png");
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

