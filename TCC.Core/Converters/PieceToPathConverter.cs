using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    internal class PieceToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (GearPiece?) value ?? 0;
            switch (p)
            {
                case GearPiece.Weapon:
                    return Application.Current.FindResource("SvgWeapon");
                case GearPiece.Armor:
                    return Application.Current.FindResource("SvgArmor");
                case GearPiece.Hands:
                    return Application.Current.FindResource("SvgHands");
                case GearPiece.Feet:
                    return Application.Current.FindResource("SvgFeet");
                case GearPiece.CritNecklace:
                    return Application.Current.FindResource("SvgNecklace");
                case GearPiece.CritEarring:
                    return Application.Current.FindResource("SvgEarring");
                case GearPiece.CritRing:
                    return Application.Current.FindResource("SvgRing");
                case GearPiece.PowerNecklace:
                    return Application.Current.FindResource("SvgNecklace");
                case GearPiece.PowerEarring:
                    return Application.Current.FindResource("SvgEarring");
                case GearPiece.PowerRing:
                    return Application.Current.FindResource("SvgRing");
                case GearPiece.Circlet:
                    return Application.Current.FindResource("SvgCirclet");
                case GearPiece.Belt:
                    return Application.Current.FindResource("SvgBelt");
            }
            return Application.Current.FindResource("SvgClose");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
