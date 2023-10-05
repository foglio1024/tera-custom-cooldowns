using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;

namespace TCC.UI.Converters;

public class GroupSizeToTemplateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = (int?)value ?? 0;
        return val > App.Settings.GroupWindowSettings.GroupSizeThreshold
            ? DataTemplates.RaidDataTemplate
            : DataTemplates.PartyDataTemplate;
        //return Application.Current.FindResource(val > App.Settings.GroupWindowSettings.GroupSizeThreshold ? "RaidDataTemplate" : "PartyDataTemplate");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}