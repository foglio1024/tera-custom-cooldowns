using System;
using System.Globalization;
using System.Windows.Data;

namespace TCC.Converters
{
    class PieceToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (GearPiece) value;
            switch (p)
            {
                case GearPiece.Weapon:
                    return App.Current.FindResource("SvgWeapon");
                case GearPiece.Armor:
                    return App.Current.FindResource("SvgArmor");
                case GearPiece.Hands:
                    return App.Current.FindResource("SvgHands");
                case GearPiece.Feet:
                    return App.Current.FindResource("SvgFeet");
                case GearPiece.CritNecklace:
                    return App.Current.FindResource("SvgNecklace");
                case GearPiece.CritEarring:
                    return App.Current.FindResource("SvgEarring");
                case GearPiece.CritRing:
                    return App.Current.FindResource("SvgRing");
                case GearPiece.PowerNecklace:
                    return App.Current.FindResource("SvgNecklace");
                case GearPiece.PowerEarring:
                    return App.Current.FindResource("SvgEarring");
                case GearPiece.PowerRing:
                    return App.Current.FindResource("SvgRing");
                case GearPiece.Circlet:
                    return App.Current.FindResource("SvgCirclet");
                case GearPiece.Belt:
                    return App.Current.FindResource("SvgBelt");
            }
            return App.Current.FindResource("SvgClose");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
