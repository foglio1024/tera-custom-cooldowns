using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class AbnormalityTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (AbnormalityType?)value ?? AbnormalityType.Buff;
            string resName;
            switch (val)
            {
                case AbnormalityType.Stun:
                    resName = "Stun";
                    break;
                case AbnormalityType.DOT:
                    resName = "Dot";
                    break;
                case AbnormalityType.Debuff:
                    resName = "Debuff";
                    break;
                case AbnormalityType.Buff:
                    resName = "Buff";
                    break;
                default:
                    return new SolidColorBrush(Colors.White);
            }

            // ReSharper disable once PossibleNullReferenceException
            var color = (Color)Application.Current.FindResource($"Abnormality{resName}Color");
            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}