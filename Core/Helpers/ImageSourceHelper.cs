using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Core.Helpers
{
    public static class ImageSourceHelper
    {
        public static ImageSource GetImageSource<T>(string itemPath)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource =
                new Uri("pack://application:,,,/" + Assembly.GetAssembly(typeof (T)).FullName + ";component/" + itemPath);
            img.EndInit();
            return img;
        }

        public static ImageSource GetImageSource(Uri uri)
        {
            if (uri == null) return null;
            BitmapImage bi = null;
            BackgroundHelper.DoInMainThread(() =>
                                                {
                                                    try
                                                    {
                                                        bi = new BitmapImage(uri);
                                                    }
                                                    catch
                                                    {
                                                        bi = null;
                                                    }
                                                });
            return bi;
        }

        public static ImageSource GetImageSource(Stream stream)
        {
            if (stream == null) return null;
            BitmapImage bi = null;
            BackgroundHelper.DoInMainThread(() =>
                                                {
                                                    try
                                                    {
                                                        bi = new BitmapImage();
                                                        bi.BeginInit();
                                                        bi.StreamSource = stream;
                                                        bi.EndInit();
                                                    }
                                                    catch
                                                    {
                                                        bi = null;
                                                    }
                                                });
            return bi;
        }

        public static ImageSource CreateEmptyImageSource()
        {
            var bi = new BitmapImage();
            var ms = new MemoryStream();
            var source = new Bitmap(1, 1);
            source.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
    }
}