using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Nostrum.WinAPI;
using TCC.R;
using TeraPacketParser.Messages;

namespace TCC.UI.Converters
{
    public class LogoInfoToImageConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            var guildId = (uint?) value ?? 0;
            if (!S_IMAGE_DATA.Database.ContainsKey(guildId)) return MiscResources.DefaultGuildLogo; 


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
                Gdi32.DeleteObject(ip);
            }

            return bs;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
