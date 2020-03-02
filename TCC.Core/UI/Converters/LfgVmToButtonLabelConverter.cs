using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class LfgVmToButtonLabelConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var creating = (bool)values[0];
            var text = (string)values[1];

            if (creating)
            {
                if (string.IsNullOrEmpty(text)) return "Cancel";
                return "Post message";
            }

            return "Create message";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
