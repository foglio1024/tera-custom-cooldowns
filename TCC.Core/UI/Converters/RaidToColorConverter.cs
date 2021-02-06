using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Brush = System.Drawing.Brush;

namespace TCC.UI.Converters
{
    public class RaidToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = R.Colors.ChatPartyColor;
            if ((bool?)value ?? false) color = R.Colors.Tier5DungeonColor;
            if (targetType == typeof(Brush))
                return new SolidColorBrush(color);
            else
                return color;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
