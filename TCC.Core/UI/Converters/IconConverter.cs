using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconName = "unknown";
            if (string.IsNullOrEmpty(value?.ToString())) 
                return Path.Combine(App.ResourcesPath, "images/" + iconName + ".png");

            iconName = value.ToString();
            iconName = iconName.Replace(".", "/");

            return Path.Combine(App.ResourcesPath, "images/" + iconName + ".png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
