using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Converters
{
    public class StaminaBarColorConverter : IValueConverter
    {
        private readonly Color _resolveColor = Color.FromRgb(0x66, 0xbb, 0xff);
        private readonly Color _rageColor = Colors.OrangeRed;
        private readonly Color _willpowerColor = Colors.Orange;
        private readonly Color _chiColor = Color.FromRgb(208, 165, 255);
        private readonly Color _ragnarokColor = Color.FromRgb(194, 214, 255);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Class?)value)
            {
                case Class.Warrior:
                    return new SolidColorBrush(_resolveColor);
                case Class.Lancer:
                    return new SolidColorBrush(_resolveColor);
                case Class.Gunner:
                    return new SolidColorBrush(_willpowerColor);
                case Class.Brawler:
                    return new SolidColorBrush(_rageColor);
                case Class.Ninja:
                    return new SolidColorBrush(_chiColor);
                case Class.Valkyrie:
                    return new SolidColorBrush(_ragnarokColor);
                default:
                    return new SolidColorBrush(_resolveColor);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
