using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.UI.Converters
{
    public class AbnormalityTypeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            value ??= AbnormalityType.Buff;
            var val = (AbnormalityType)value;
            //string resName;
            Color ret;
            switch (val)
            {
                case AbnormalityType.Stun:
                    ret = R.Colors.AbnormalityStunColor;
                    //resName = "Stun";
                    break;
                case AbnormalityType.DOT:
                    ret = R.Colors.AbnormalityDotColor;
                    //resName = "Dot";
                    break;
                case AbnormalityType.Debuff:
                    ret = R.Colors.AbnormalityDebuffColor;
                    //resName = "Debuff";
                    break;
                case AbnormalityType.Buff:
                    ret = R.Colors.AbnormalityBuffColor;
                    //resName = "Buff";
                    break;
                case AbnormalityType.Special:
                    ret = R.Colors.GoldColor;
                    break;
                default:
                    return new SolidColorBrush(Colors.White);
            }

            // ReSharper disable once PossibleNullReferenceException
            //var color = (Color)Application.Current.FindResource($"Abnormality{resName}Color");
            if (targetType == typeof(Color)) return ret;
            return new SolidColorBrush(ret);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}