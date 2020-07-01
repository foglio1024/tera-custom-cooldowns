using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public enum LaurelType
    {
        Old,
        Rhomb,
        RhombBig,
        RhombBottom
    }
    public class LaurelImageConverter : MarkupExtension, IValueConverter
    {
        public LaurelType LaurelType { get; set; }
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var laurel = (Laurel?) value ?? Laurel.None;

            return (LaurelType, l: laurel) switch
            {
                (LaurelType.Old,     Laurel.Bronze)   => R.MiscResources.BronzeLaurel, 
                (LaurelType.Old,     Laurel.Silver)   => R.MiscResources.SilverLaurel, 
                (LaurelType.Old,     Laurel.Gold)     => R.MiscResources.GoldLaurel, 
                (LaurelType.Old,     Laurel.Diamond)  => R.MiscResources.DiamondLaurel, 
                (LaurelType.Old,     Laurel.Champion) => R.MiscResources.ChampionLaurel, 
                (LaurelType.Rhomb,     Laurel.Bronze)   => R.MiscResources.BronzeLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Silver)   => R.MiscResources.SilverLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Gold)     => R.MiscResources.GoldLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Diamond)  => R.MiscResources.DiamondLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Champion) => R.MiscResources.ChampionLaurelRhomb, 
                (LaurelType.RhombBig,  Laurel.Bronze)   => R.MiscResources.BronzeLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Silver)   => R.MiscResources.SilverLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Gold)     => R.MiscResources.GoldLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Diamond)  => R.MiscResources.DiamondLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Champion) => R.MiscResources.ChampionLaurelRhombBig,
                (LaurelType.RhombBottom, Laurel.Bronze)   => R.MiscResources.BronzeLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Silver)   => R.MiscResources.SilverLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Gold)     => R.MiscResources.GoldLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Diamond)  => R.MiscResources.DiamondLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Champion) => R.MiscResources.ChampionLaurelRhombBottom, 
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
