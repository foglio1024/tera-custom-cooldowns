using System;
using System.Globalization;
using System.Windows.Data;
namespace TCC.Converters
{
    public class HP_PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float hp = (float)value;
            if(SessionManager.CurrentPlayer.MaxHP > 0)
            {
                return hp / SessionManager.CurrentPlayer.MaxHP;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
