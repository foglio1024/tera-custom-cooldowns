using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class StaminaBarColorConverter : IValueConverter
    {
        private Color ResolveColor = Color.FromRgb(0x66, 0xbb, 0xff);
        private Color RageColor = Colors.OrangeRed;
        private Color WillpowerColor = Colors.Orange;
        private Color ChiColor = Color.FromRgb(208, 165, 255);
        private Color RagnarokColor = Color.FromRgb(194, 214, 255);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Class)value)
            {
                case Class.Warrior:
                    return new SolidColorBrush(ResolveColor);
                case Class.Lancer:
                    return new SolidColorBrush(ResolveColor);
                case Class.Gunner:
                    return new SolidColorBrush(WillpowerColor);
                case Class.Brawler:
                    return new SolidColorBrush(RageColor);
                case Class.Ninja:
                    return new SolidColorBrush(ChiColor);
                case Class.Valkyrie:
                    return new SolidColorBrush(RagnarokColor);
                default:
                    return new SolidColorBrush(ResolveColor);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
