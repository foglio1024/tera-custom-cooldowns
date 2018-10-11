using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Controls.ClassBars
{
    public class WarriorStanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var s = (WarriorStance)value;
                switch (s)
                {
                    default:
                        return Application.Current.FindResource("RevampBorderBrush");
                    case WarriorStance.Assault:
                        return Application.Current.FindResource("AssaultStanceBrush");
                    case WarriorStance.Defensive:
                        return Application.Current.FindResource("DefensiveStanceBrush");
                }
            }
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}