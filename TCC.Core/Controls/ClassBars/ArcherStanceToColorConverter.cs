using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Controls.ClassBars
{
    public class ArcherStanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var s = (ArcherStance)value;
                switch (s)
                {
                    default:
                        return Brushes.DimGray;
                    case ArcherStance.SniperEye:
                        return Application.Current.FindResource("SniperEyeColor");
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