using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.UI.Converters
{
    public class LaurelImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Laurel l)) l = Laurel.None;
            bool kr = false, big = false, half = false;
            if (parameter != null)
            {
                kr = parameter.ToString().Contains("kr");
                big = parameter.ToString().Contains("big");
                half = parameter.ToString().Contains("half");
            }

            if (half)
            {
                return l switch
                {
                    Laurel.Bronze => R.MiscResources.BronzeLaurelNewBottom,
                    Laurel.Silver => R.MiscResources.SilverLaurelNewBottom,
                    Laurel.Gold => R.MiscResources.GoldLaurelNewBottom,
                    Laurel.Diamond => R.MiscResources.DiamondLaurelNewBottom,
                    Laurel.Champion => R.MiscResources.ChampionLaurelNewBottom,
                    _ => null
                };
            }

            if (big)
            {
                return l switch
                {
                    Laurel.Bronze => R.MiscResources.BronzeLaurelNewBig,
                    Laurel.Silver => R.MiscResources.SilverLaurelNewBig,
                    Laurel.Gold => R.MiscResources.GoldLaurelNewBig,
                    Laurel.Diamond => R.MiscResources.DiamondLaurelNewBig,
                    Laurel.Champion => R.MiscResources.ChampionLaurelNewBig,
                    _ => null
                };

            }

            if (!kr)
            {
                return l switch
                {
                    Laurel.Bronze => R.MiscResources.BronzeLaurel,
                    Laurel.Silver => R.MiscResources.SilverLaurel,
                    Laurel.Gold => R.MiscResources.GoldLaurel,
                    Laurel.Diamond => R.MiscResources.DiamondLaurel,
                    Laurel.Champion => R.MiscResources.ChampionLaurel,
                    _ => null
                };
            }

            return l switch
            {
                Laurel.Bronze => R.MiscResources.BronzeLaurelNew,
                Laurel.Silver => R.MiscResources.SilverLaurelNew,
                Laurel.Gold => R.MiscResources.GoldLaurelNew,
                Laurel.Diamond => R.MiscResources.DiamondLaurelNew,
                Laurel.Champion => R.MiscResources.ChampionLaurelNew,
                _ => null
            };


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
