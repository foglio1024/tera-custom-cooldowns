using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class RaidToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool?) value ?? false) return R.Brushes.Tier5DungeonBrush;
            return R.Brushes.Tier2DungeonBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
