using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using TCC.Utils;
using Brushes = TCC.R.Brushes;
using Colors = TCC.R.Colors;

namespace TCC.UI.Converters;

public class StringToFillConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string c) return Brushes.ChatSystemGenericBrush;
        try
        {
            if (string.IsNullOrWhiteSpace(c)) return Brushes.ChatSystemGenericBrush;
            if (targetType == typeof(Brush)) return new SolidColorBrush(MiscUtils.ParseColor(c));
            if (targetType == typeof(Color)) return MiscUtils.ParseColor(c);

        }
        catch (FormatException e)
        {
            Log.F($"[StringToFillConverter] Failed to parse color from {c}: {e}");
            Log.Chat(ChatUtils.Font("An error occured while parsing last chat message. Please report to the deveolper attaching error.log.", Colors.HpColor.ToHex()));
        }
        return Brushes.ChatSystemGenericBrush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}