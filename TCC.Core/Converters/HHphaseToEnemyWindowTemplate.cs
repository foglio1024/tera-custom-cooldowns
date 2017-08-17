using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TCC.Converters
{
    public class HHphaseToEnemyWindowTemplate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((HarrowholdPhase)value)
            {
                case HarrowholdPhase.Phase1:
                    return App.Current.FindResource("Phase1EnemyWindowLayout");
                case HarrowholdPhase.Phase2:
                    return App.Current.FindResource("Phase2EnemyWindowLayout");
                case HarrowholdPhase.Phase3:
                    return App.Current.FindResource("Phase3EnemyWindowLayout");
                case HarrowholdPhase.Phase4:
                    return App.Current.FindResource("Phase4EnemyWindowLayout");
                case HarrowholdPhase.Balistas:
                    return App.Current.FindResource("Phase2BEnemyWindowLayout");
                default:
                    return App.Current.FindResource("DefaultEnemyWindowLayout");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
