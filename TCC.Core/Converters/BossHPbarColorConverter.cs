using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    public class BossHPbarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            if (value == null) return new SolidColorBrush(Colors.DodgerBlue);
            return (bool)value ? R.Brushes.HpBrush /*Application.Current.FindResource("HpBrush")*/ : R.Brushes.DefensiveStanceBrush /*new SolidColorBrush(Colors.DodgerBlue)*/;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}