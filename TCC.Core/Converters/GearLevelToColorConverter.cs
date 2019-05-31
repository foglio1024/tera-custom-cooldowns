using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.Converters
{
    public class GearLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (GearTier? ) value ?? GearTier.Low;
            switch (val)
            {
                case GearTier.Low:
                    return R.Brushes.Tier2DungeonBrush; 
                case GearTier.Mid:
                    return R.Brushes.Tier3DungeonBrush; 
                case GearTier.High:
                    return R.Brushes.Tier4DungeonBrush; 
                case GearTier.Top:
                    return R.Brushes.Tier5DungeonBrush; 
                default:
                    return R.Brushes.Tier1DungeonBrush; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
