using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TeraPacketParser.Messages;

namespace TCC.Converters
{
    public class LogoInfoToImageConverter : IValueConverter
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var guildId = (uint?) value ?? 0;
            if (!S_IMAGE_DATA.Database.ContainsKey(guildId)) return R.MiscResources.DefaultGuildLogo; 


            var ip = S_IMAGE_DATA.Database[guildId].GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                    IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
