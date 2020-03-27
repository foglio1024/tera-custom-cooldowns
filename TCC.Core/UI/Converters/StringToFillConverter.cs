using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Nostrum;
using Nostrum.Extensions;
using TCC.Utils;

namespace TCC.UI.Converters
{
    public class StringToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (string)value;
            try
            {
                if (string.IsNullOrEmpty(c)) return R.Brushes.ChatSystemGenericBrush;
                if (targetType == typeof(Brush)) return new SolidColorBrush(MiscUtils.ParseColor(c));
                if (targetType == typeof(Color)) return MiscUtils.ParseColor(c);

            }
            catch (FormatException e)
            {
                Log.F($"[StringToFillConverter] Failed to parse color from {c}: {e}");
                Log.Chat(ChatUtils.Font("An error occured while parsing last chat message. Please report to the deveolper attaching error.log.", R.Colors.HpColor.ToHex()));
            }
            return R.Brushes.ChatSystemGenericBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
