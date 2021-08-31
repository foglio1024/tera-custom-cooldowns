using System;
using System.Globalization;
using System.Windows.Data;
using TCC.R;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class PieceToPathConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (GearPiece?) value ?? 0;
            return p switch
            {
                GearPiece.Weapon => SVG.SvgWeapon,
                GearPiece.Armor => SVG.SvgArmor,
                GearPiece.Hands => SVG.SvgHands,
                GearPiece.Feet => SVG.SvgFeet,
                GearPiece.CritNecklace => SVG.SvgNecklace,
                GearPiece.CritEarring => SVG.SvgEarring,
                GearPiece.CritRing => SVG.SvgRing,
                GearPiece.PowerNecklace => SVG.SvgNecklace,
                GearPiece.PowerEarring => SVG.SvgEarring,
                GearPiece.PowerRing => SVG.SvgRing,
                GearPiece.Circlet => SVG.SvgCirclet,
                GearPiece.Belt => SVG.SvgBelt,
                _ => Nostrum_WPF_SVG.SvgClose
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
