using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TCC.Converters
{
    /// <summary>
    /// Converts chat message's header background to orange if player is mentioned.
    /// </summary>
    public class MentionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = false;
            var transparent = false;
            var col = (bool?)value ?? false
                ? R.Colors.GoldColor
                : Colors.Black; //TODO: use trigger
            if (parameter is string par)
            {
                color = par.IndexOf("color") != -1;
                transparent = par.IndexOf("transparent") != -1;
            }
            if (transparent) col.A = 0;

            if (color) return col;
            else return new SolidColorBrush(col);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
