using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    /// <summary>
    /// Rotates whisper's message arrow depending on message direction.
    /// </summary>
    public class SentReceivedToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (ChatChannel)value;

            switch (c)
            {
                case ChatChannel.SentWhisper:
                    return 1;
                case ChatChannel.ReceivedWhisper:
                    return -1;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
