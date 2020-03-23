using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class PieceToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (GearPiece?) value ?? 0;
            return p switch
            {
                GearPiece.Weapon => R.SVG.SvgWeapon,
                GearPiece.Armor => R.SVG.SvgArmor,
                GearPiece.Hands => R.SVG.SvgHands,
                GearPiece.Feet => R.SVG.SvgFeet,
                GearPiece.CritNecklace => R.SVG.SvgNecklace,
                GearPiece.CritEarring => R.SVG.SvgEarring,
                GearPiece.CritRing => R.SVG.SvgRing,
                GearPiece.PowerNecklace => R.SVG.SvgNecklace,
                GearPiece.PowerEarring => R.SVG.SvgEarring,
                GearPiece.PowerRing => R.SVG.SvgRing,
                GearPiece.Circlet => R.SVG.SvgCirclet,
                GearPiece.Belt => R.SVG.SvgBelt,
                _ => R.Nostrum_SVG.SvgClose
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
