using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Core.Helpers;

namespace Medical.AppLayer.Converters
{
    public class IntoToColorConverter2 : MarkupExtension, IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, System.Type targetType,
                    object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value == null)
                return Brushes.White;

            var result = SafeConvert.ToInt32(value.ToString());
            if(result == null)
                return Brushes.White;

            switch (result)
            {
                case 1:
                    return new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0),Color.FromArgb(0, 255, 0, 0), 0);
                case 2:
                    return new LinearGradientBrush(Color.FromArgb(100, 0, 255, 0), Color.FromArgb(0, 255, 0, 0), 0);
                case 3:
                    return new LinearGradientBrush(Color.FromArgb(100, 0, 0, 255), Color.FromArgb(0, 255, 0, 0), 0);
                default:
                     return Brushes.White;
                    
            }
            
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
