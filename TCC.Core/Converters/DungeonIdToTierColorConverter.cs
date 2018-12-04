using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class DungeonIdToTierColorConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var id = 0U;
            try
            {
                id = (uint?)value ?? 0;
            }
            catch
            {
                return R.Brushes.TierSoloDungeonBrush;// Application.Current.FindResource("TierSoloDungeonBrush");
            }

            if (!SessionManager.DungeonDatabase.DungeonDefs.ContainsKey(id)) return R.Brushes.TierSoloDungeonBrush; //Application.Current.FindResource("TierSoloDungeonBrush");
            var t = SessionManager.DungeonDatabase.DungeonDefs[id].Tier;
            switch (t)
            {
                case DungeonTier.Tier2:
                    return R.Brushes.Tier2DungeonBrush; //Application.Current.FindResource("Tier2DungeonBrush");
                case DungeonTier.Tier3:
                    return R.Brushes.Tier3DungeonBrush; //Application.Current.FindResource("Tier3DungeonBrush");
                case DungeonTier.Tier4:
                    return R.Brushes.Tier4DungeonBrush; //Application.Current.FindResource("Tier4DungeonBrush");
                case DungeonTier.Tier5:
                    return R.Brushes.Tier5DungeonBrush; //Application.Current.FindResource("Tier5DungeonBrush");
                default:
                    return R.Brushes.TierSoloDungeonBrush; //Application.Current.FindResource("TierSoloDungeonBrush");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
