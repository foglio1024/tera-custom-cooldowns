using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class HHphaseToEnemyWindowTemplate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            switch ((HarrowholdPhase)value)
            {
                case HarrowholdPhase.Phase1:
                    return R.DataTemplates.Phase1EnemyWindowLayout; //Application.Current.FindResource("Phase1EnemyWindowLayout");
                default:
                    return R.DataTemplates.DefaultEnemyWindowLayout; //Application.Current.FindResource("DefaultEnemyWindowLayout");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
