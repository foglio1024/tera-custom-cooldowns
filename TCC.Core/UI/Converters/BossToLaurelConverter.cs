using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using TCC.Data.Npc;

namespace TCC.UI.Converters;

// todo: reintroduce this
public class BossToLaurelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NPC npc) return null;
        var laurel = "";

        switch (npc.ZoneId)
        {
            case 620: //Balder's Refuge guardian
                switch (npc.TemplateId)
                {
                    case 1000:
                        laurel = "silver";
                        break;
                    case 1001:
                    case 1004:
                    case 1005:
                        laurel = "bronze";
                        break;
                }
                break;
            case 621: //Crack the crustacean guardian
                if (npc.TemplateId == 3001) laurel = "silver";
                break;
            case 622: //Dreadreaper guardian
                if (npc.TemplateId == 1000) laurel = "silver";
                break;
            case 713: //Ghillieglade
            case 813: //Ghillieglade
                switch (npc.TemplateId)
                {
                    case 81301:
                        laurel = "gold";
                        break;
                    case 81398:
                    case 81399:
                        laurel = "silver";
                        break;
                }

                break;
            case 628: //Superior guardian
                switch (npc.TemplateId)
                {
                    case 1000:
                        laurel = "gold";
                        break;
                    case 3000:
                    case 3001:
                        laurel = "silver";
                        break;
                }
                break;
            case 434: //Dreadspire
                switch (npc.TemplateId)
                {
                    case 1000:
                    case 2000:
                        laurel = "bronze";
                        break;
                    case 3000:
                    case 4000:
                        laurel = "silver";
                        break;
                    case 5000:
                    case 6000:
                        laurel = "gold";
                        break;
                    case 7000:
                    case 8000:
                    case 9000:
                        laurel = "diamond";
                        break;
                    case 10000:
                        laurel = "champion";
                        break;
                }
                break;
            default:
                switch (npc.TemplateId)
                {
                    case 1000:
                    case 1001:
                        laurel = "bronze";
                        break;
                    case 2000:
                    case 1002:
                        laurel = "silver";
                        break;
                    case 3000:
                    case 1003:
                        laurel = "gold";
                        break;
                    case 4000:
                        laurel = "diamond";
                        break;
                }
                break;
        }


        if (laurel == "") return null;

        return Path.Combine(App.ResourcesPath, "images/Icon_Laurels/" + laurel + "_kr_big.png");

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}