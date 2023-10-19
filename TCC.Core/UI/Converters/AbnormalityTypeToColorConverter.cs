using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.UI.Converters;

public class AbnormalityTypeToColorConverter : IValueConverter
{
    public SolidColorBrush? Stun { get; set; }
    public SolidColorBrush? DOT { get; set; }
    public SolidColorBrush? Debuff { get; set; }
    public SolidColorBrush? Buff { get; set; }
    public SolidColorBrush? Special { get; set; }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AbnormalityType val) val = AbnormalityType.Buff;

        var brush = val switch
        {
            AbnormalityType.Stun => Stun,
            AbnormalityType.DOT => DOT,
            AbnormalityType.Debuff => Debuff,
            AbnormalityType.Buff => Buff,
            AbnormalityType.Special => Special,
            _ => Brushes.White
        };

        if (targetType == typeof(Color)) return brush?.Color;
        return brush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}