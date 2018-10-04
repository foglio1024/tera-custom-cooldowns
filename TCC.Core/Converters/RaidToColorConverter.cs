using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    internal class RaidToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool?)value ?? false) return Application.Current.FindResource("Tier5DungeonBrush") as SolidColorBrush;
            return Application.Current.FindResource("Tier2DungeonBrush") as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
