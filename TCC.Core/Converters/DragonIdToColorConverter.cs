using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    public class DragonIdToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO:TRIGGERS?
            var id = (uint?)value;
            if (id == 1100) return R.Brushes.IgnidraxBrush; //Application.Current.FindResource("IgnidraxBrush");
            else if (id == 1101) return R.Brushes.TerradraxBrush; //Application.Current.FindResource("TerradraxBrush");
            else if (id == 1102) return R.Brushes.UmbradraxBrush; //Application.Current.FindResource("UmbradraxBrush");
            else if (id == 1103) return R.Brushes.AquadraxBrush; //Application.Current.FindResource("AquadraxBrush");
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
