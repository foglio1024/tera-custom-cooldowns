using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;
using TCC.R;

namespace TCC.UI.Converters;

public class CooldownWindowModeToTemplateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //return Application.Current.FindResource((CooldownBarMode?)value == CooldownBarMode.Fixed ? "FixedCooldownTemplate" : "NormalCooldownTemplate")!;
        return (CooldownBarMode?) value == CooldownBarMode.Fixed
            ? DataTemplates.FixedCooldownTemplate
            : DataTemplates.NormalCooldownTemplate;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}