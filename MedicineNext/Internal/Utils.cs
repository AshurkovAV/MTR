using System.Windows.Media;
using System.Windows.Media.Imaging;
using Core.Extensions;
using DevExpress.Utils;

namespace MedicineNext.Internal
{
    public static class Utils
    {
        public static class GlyphHelper
        {
            public static ImageSource GetGlyph(string itemPath)
            {
                if (itemPath.IsNotNullOrEmpty())
                {
                    return new BitmapImage(AssemblyHelper.GetResourceUri(typeof(MainWindow).Assembly, itemPath));
                }

                return null;
            }
        }

        
    }
}
