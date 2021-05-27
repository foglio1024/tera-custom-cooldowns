using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TCC.R;
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
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {

            var laurel = (Laurel?) value ?? Laurel.None;

            return (LaurelType, l: laurel) switch
            {
                (LaurelType.Old,     Laurel.Bronze)   => MiscResources.BronzeLaurel, 
                (LaurelType.Old,     Laurel.Silver)   => MiscResources.SilverLaurel, 
                (LaurelType.Old,     Laurel.Gold)     => MiscResources.GoldLaurel, 
                (LaurelType.Old,     Laurel.Diamond)  => MiscResources.DiamondLaurel, 
                (LaurelType.Old,     Laurel.Champion) => MiscResources.ChampionLaurel, 
                (LaurelType.Rhomb,     Laurel.Bronze)   => MiscResources.BronzeLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Silver)   => MiscResources.SilverLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Gold)     => MiscResources.GoldLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Diamond)  => MiscResources.DiamondLaurelRhomb, 
                (LaurelType.Rhomb,     Laurel.Champion) => MiscResources.ChampionLaurelRhomb, 
                (LaurelType.RhombBig,  Laurel.Bronze)   => MiscResources.BronzeLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Silver)   => MiscResources.SilverLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Gold)     => MiscResources.GoldLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Diamond)  => MiscResources.DiamondLaurelRhombBig, 
                (LaurelType.RhombBig,  Laurel.Champion) => MiscResources.ChampionLaurelRhombBig,
                (LaurelType.RhombBottom, Laurel.Bronze)   => MiscResources.BronzeLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Silver)   => MiscResources.SilverLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Gold)     => MiscResources.GoldLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Diamond)  => MiscResources.DiamondLaurelRhombBottom, 
                (LaurelType.RhombBottom, Laurel.Champion) => MiscResources.ChampionLaurelRhombBottom, 
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
