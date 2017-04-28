using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : BaseINPC
    {
        public Player Player
        {
            get => SessionManager.CurrentPlayer;
        }
        public CharacterWindowViewModel()
        {
        }

        public double HPfactor { get => ValueToFactor(Player.CurrentHP, Player.MaxHP); }
        public double MPfactor { get => ValueToFactor(Player.CurrentMP, Player.MaxMP); }
        public double STfactor { get => ValueToFactor(Player.CurrentST, Player.MaxST); }

        double ValueToFactor(object val, object max)
        {
            var v = Convert.ToDouble(val);
            var m = Convert.ToDouble(max);

            if(m == 0)
            {
                return 1;
            }
            else
            {
                return v / m;
            }
        }

    }
}
namespace TCC.Converters
{
    public class StaminaBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Class)value)
            {
                case Class.Warrior:
                    return GridLength.Auto;
                case Class.Lancer:
                    return GridLength.Auto;
                case Class.Engineer:
                    return GridLength.Auto;
                case Class.Fighter:
                    return GridLength.Auto;
                case Class.Assassin:
                    return GridLength.Auto;
                case Class.Glaiver:
                    return GridLength.Auto;
                default:
                    return new GridLength(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StaminaBarColorConverter : IValueConverter
    {
        private Color ResolveColor = Color.FromRgb(0x66, 0xbb, 0xff);
        private Color RageColor = Colors.OrangeRed;
        private Color WillpowerColor = Colors.Orange;
        private Color ChiColor = Color.FromRgb(208, 165, 255);
        private Color RagnarokColor = Color.FromRgb(194, 214, 255);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("Converting stamina color.");
            switch ((Class)value)
            {
                case Class.Warrior:
                    return new SolidColorBrush(ResolveColor);
                case Class.Lancer:
                    return new SolidColorBrush(ResolveColor);
                case Class.Engineer:
                    return new SolidColorBrush(WillpowerColor);
                case Class.Fighter:
                    return new SolidColorBrush(RageColor);
                case Class.Assassin:
                    return new SolidColorBrush(ChiColor);
                case Class.Glaiver:
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
    public class FactorFromValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("Converting stamina value.");

            var v = System.Convert.ToSingle(values[0]);
            var m = System.Convert.ToInt32(values[1]);
            if (m == 0)
            {
                return 1;
            }
            else
            {
                return (double)v/(double)m;
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
