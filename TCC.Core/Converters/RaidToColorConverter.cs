using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    internal class RaidToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return App.Current.FindResource("Tier5DungeonColor") as SolidColorBrush;
            return App.Current.FindResource("Tier2DungeonColor") as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
