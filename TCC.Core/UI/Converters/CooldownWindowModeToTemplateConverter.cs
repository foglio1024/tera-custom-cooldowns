using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.UI.Converters
{
    public class CooldownWindowModeToTemplateConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            //return Application.Current.FindResource((CooldownBarMode?)value == CooldownBarMode.Fixed ? "FixedCooldownTemplate" : "NormalCooldownTemplate")!;
            return (CooldownBarMode?) value == CooldownBarMode.Fixed
                ? R.DataTemplates.FixedCooldownTemplate
                : R.DataTemplates.NormalCooldownTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
