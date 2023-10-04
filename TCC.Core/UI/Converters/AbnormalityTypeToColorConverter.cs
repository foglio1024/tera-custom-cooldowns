using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;
using Colors = TCC.R.Colors;

namespace TCC.UI.Converters;

public class AbnormalityTypeToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value ??= AbnormalityType.Buff;
        var val = (AbnormalityType)value;
        //string resName;
        Color ret;
        switch (val)
        {
            case AbnormalityType.Stun:
                ret = Colors.AbnormalityStunColor;
                //resName = "Stun";
                break;
            case AbnormalityType.DOT:
                ret = Colors.AbnormalityDotColor;
                //resName = "Dot";
                break;
            case AbnormalityType.Debuff:
                ret = Colors.AbnormalityDebuffColor;
                //resName = "Debuff";
                break;
            case AbnormalityType.Buff:
                ret = Colors.AbnormalityBuffColor;
                //resName = "Buff";
                break;
            case AbnormalityType.Special:
                ret = Colors.GoldColor;
                break;
            default:
                return new SolidColorBrush(System.Windows.Media.Colors.White);
        }

        // ReSharper disable once PossibleNullReferenceException
        //var color = (Color)Application.Current.FindResource($"Abnormality{resName}Color");
        if (targetType == typeof(Color)) return ret;
        return new SolidColorBrush(ret);

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}