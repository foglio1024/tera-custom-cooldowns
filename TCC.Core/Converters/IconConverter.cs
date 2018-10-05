using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
namespace TCC.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconName = "unknown";
            if (value != null)
            {
                if (value.ToString() != "")
                {
                    iconName = value.ToString();
                    iconName = iconName.Replace(".", "/");
                }
            }
            return Path.GetDirectoryName(typeof(App).Assembly.Location)+ "/resources/images/" + iconName + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
