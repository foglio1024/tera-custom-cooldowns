using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    public class GroupSizeToTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int)value;

            if(val > SettingsManager.GroupSizeThreshold)
            {
                return App.Current.FindResource("RaidDataTemplate");
            }
            else
            {
                return App.Current.FindResource("PartyDataTemplate");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
