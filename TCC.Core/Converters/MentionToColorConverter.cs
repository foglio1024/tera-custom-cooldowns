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
            return (bool?) value ?? false ? R.Brushes.GoldBrush : Brushes.Black; //TODO: use trigger
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
