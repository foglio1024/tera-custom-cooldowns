using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class GroupSizeToTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int?)value ?? 0;
            return val > App.Settings.GroupWindowSettings.GroupSizeThreshold
                ? R.DataTemplates.RaidDataTemplate
                : R.DataTemplates.PartyDataTemplate;
            //return Application.Current.FindResource(val > App.Settings.GroupWindowSettings.GroupSizeThreshold ? "RaidDataTemplate" : "PartyDataTemplate");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
