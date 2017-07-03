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
            if ((bool)value)
            {
                return Brushes.Orange; 
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
