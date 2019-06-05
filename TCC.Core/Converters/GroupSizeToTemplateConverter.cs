using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Settings;

namespace TCC.Converters
{
    public class GroupSizeToTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int?)value ?? 0;
            return Application.Current.FindResource(val > App.Settings.GroupSizeThreshold ? "RaidDataTemplate" : "PartyDataTemplate");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
