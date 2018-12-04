using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class PieceToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (GearPiece?) value ?? 0;
            switch (p)
            {
                case GearPiece.Weapon:
                    return R.SVG.SvgWeapon;
                case GearPiece.Armor:
                    return R.SVG.SvgArmor;
                case GearPiece.Hands:
                    return R.SVG.SvgHands;
                case GearPiece.Feet:
                    return R.SVG.SvgFeet;
                case GearPiece.CritNecklace:
                    return R.SVG.SvgNecklace;
                case GearPiece.CritEarring:
                    return R.SVG.SvgEarring;
                case GearPiece.CritRing:
                    return R.SVG.SvgRing;
                case GearPiece.PowerNecklace:
                    return R.SVG.SvgNecklace;
                case GearPiece.PowerEarring:
                    return R.SVG.SvgEarring;
                case GearPiece.PowerRing:
                    return R.SVG.SvgRing;
                case GearPiece.Circlet:
                    return R.SVG.SvgCirclet;
                case GearPiece.Belt:
                    return R.SVG.SvgBelt;
            }
            return R.SVG.SvgClose;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
