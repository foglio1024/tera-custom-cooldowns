using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TCC.Controls.ChatControls
{
    public class EmojiNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.GetDirectoryName(typeof(App).Assembly.Location)+ "/resources/images/emotes/thinking.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}