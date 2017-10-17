using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    return App.Current.FindResource("SVG.Weapon");
                case GearPiece.Armor:
                    return App.Current.FindResource("SVG.Armor");
                case GearPiece.Hands:
                    return App.Current.FindResource("SVG.Hands");
                case GearPiece.Feet:
                    return App.Current.FindResource("SVG.Feet");
                case GearPiece.CritNecklace:
                    return App.Current.FindResource("SVG.Necklace");
                case GearPiece.CritEarring:
                    return App.Current.FindResource("SVG.Earring");
                case GearPiece.CritRing:
                    return App.Current.FindResource("SVG.Ring");
                case GearPiece.PowerNecklace:
                    return App.Current.FindResource("SVG.Necklace");
                case GearPiece.PowerEarring:
                    return App.Current.FindResource("SVG.Earring");
                case GearPiece.PowerRing:
                    return App.Current.FindResource("SVG.Ring");
                case GearPiece.Circlet:
                    return App.Current.FindResource("SVG.Circlet");
                case GearPiece.Belt:
                    return App.Current.FindResource("SVG.Belt");
            }
            return App.Current.FindResource("SVG.Close");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
