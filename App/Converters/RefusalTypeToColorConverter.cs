using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Core;
using Core.Extensions;
using Core.Helpers;
using Medical.AppLayer.Resources;

namespace Medical.AppLayer.Converters
{
    public class RefusalTypeToColorConverter : MarkupExtension, IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, System.Type targetType,
                    object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
            {
                return null;
            }

            if (value == null)
                return Brushes.White;

            var model = (RefusalType)value;
            if (model.IsNull())
            {
                return Brushes.White;
            }

            if (model == RefusalType.MEC || model == RefusalType.ExternalMEC)
            {
                return new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0), Color.FromArgb(50, 255, 0, 0), 0);
            }

            if (model == RefusalType.MEE || model == RefusalType.ExternalMEE)
            {
                return new LinearGradientBrush(Color.FromArgb(100, 106, 0, 255), Color.FromArgb(50, 106, 0, 255), 0);
            }

            if (model == RefusalType.EQMA || model == RefusalType.ExternalEQMA)
            {
                return new LinearGradientBrush(Color.FromArgb(100, 240, 163, 10), Color.FromArgb(50, 240, 163, 10), 0);
            }

            if (model == RefusalType.External)
            {
                return new LinearGradientBrush(Color.FromArgb(100, 27, 161, 226), Color.FromArgb(50, 27, 161, 226), 0);
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, System.Type targetType,
                    object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
