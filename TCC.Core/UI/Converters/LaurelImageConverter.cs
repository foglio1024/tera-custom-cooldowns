using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TCC.R;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.UI.Converters;

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
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var laurel = value switch
        {
            CustomLaurel cl => cl,
            Laurel l => TccUtils.CustomFromLaurel(l),
            _ => CustomLaurel.None
        };

        return (LaurelType, l: laurel) switch
        {
            (LaurelType.Old,         CustomLaurel.Bronze)        => MiscResources.BronzeLaurel, 
            (LaurelType.Old,         CustomLaurel.Silver)        => MiscResources.SilverLaurel, 
            (LaurelType.Old,         CustomLaurel.Gold)          => MiscResources.GoldLaurel, 
            (LaurelType.Old,         CustomLaurel.Diamond)       => MiscResources.DiamondLaurel, 
            (LaurelType.Old,         CustomLaurel.Champion)      => MiscResources.ChampionLaurel, 
            (LaurelType.Rhomb,       CustomLaurel.Bronze)        => MiscResources.BronzeLaurelRhomb, 
            (LaurelType.Rhomb,       CustomLaurel.Silver)        => MiscResources.SilverLaurelRhomb, 
            (LaurelType.Rhomb,       CustomLaurel.Gold)          => MiscResources.GoldLaurelRhomb, 
            (LaurelType.Rhomb,       CustomLaurel.Diamond)       => MiscResources.DiamondLaurelRhomb, 
            (LaurelType.Rhomb,       CustomLaurel.Champion)      => MiscResources.ChampionLaurelRhomb, 
            (LaurelType.Rhomb,       CustomLaurel.ChampionPink)  => MiscResources.ChampionPinkLaurelRhomb, 
            (LaurelType.Rhomb,       CustomLaurel.ChampionBlack) => MiscResources.ChampionBlackLaurelRhomb, 
            (LaurelType.RhombBig,    CustomLaurel.Bronze)        => MiscResources.BronzeLaurelRhombBig, 
            (LaurelType.RhombBig,    CustomLaurel.Silver)        => MiscResources.SilverLaurelRhombBig, 
            (LaurelType.RhombBig,    CustomLaurel.Gold)          => MiscResources.GoldLaurelRhombBig, 
            (LaurelType.RhombBig,    CustomLaurel.Diamond)       => MiscResources.DiamondLaurelRhombBig, 
            (LaurelType.RhombBig,    CustomLaurel.Champion)      => MiscResources.ChampionLaurelRhombBig,
            (LaurelType.RhombBig,    CustomLaurel.ChampionPink)  => MiscResources.ChampionPinkLaurelRhombBig,
            (LaurelType.RhombBig,    CustomLaurel.ChampionBlack) => MiscResources.ChampionBlackLaurelRhombBig,
            (LaurelType.RhombBottom, CustomLaurel.Bronze)        => MiscResources.BronzeLaurelRhombBottom, 
            (LaurelType.RhombBottom, CustomLaurel.Silver)        => MiscResources.SilverLaurelRhombBottom, 
            (LaurelType.RhombBottom, CustomLaurel.Gold)          => MiscResources.GoldLaurelRhombBottom, 
            (LaurelType.RhombBottom, CustomLaurel.Diamond)       => MiscResources.DiamondLaurelRhombBottom, 
            (LaurelType.RhombBottom, CustomLaurel.Champion)      => MiscResources.ChampionLaurelRhombBottom, 
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}