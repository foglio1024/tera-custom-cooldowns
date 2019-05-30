using FoglioUtils;

using System;
using System.Collections.Generic;

using TCC.Data;
using TCC.Data.Chat;
using TCC.ViewModels;

namespace TCC
{
    public static class TccUtils
    {
        public static string ClassEnumToString(Class c)
        {
            switch (c)
            {
                case Class.Warrior:
                    return "Warrior";
                case Class.Lancer:
                    return "Lancer";
                case Class.Slayer:
                    return "Slayer";
                case Class.Berserker:
                    return "Berserker";
                case Class.Sorcerer:
                    return "Sorcerer";
                case Class.Archer:
                    return "Archer";
                case Class.Priest:
                    return "Priest";
                case Class.Mystic:
                    return "Mystic";
                case Class.Reaper:
                    return "Reaper";
                case Class.Gunner:
                    return "Gunner";
                case Class.Brawler:
                    return "Brawler";
                case Class.Ninja:
                    return "Ninja";
                case Class.Valkyrie:
                    return "Valkyrie";
                case Class.Common:
                    return "All classes";
                default:
                    return "";
            }
        }
        public static List<ChatChannelOnOff> GetEnabledChannelsList()
        {
            var ch = EnumUtils.ListFromEnum<ChatChannel>();
            var result = new List<ChatChannelOnOff>();
            foreach (var c in ch)
            {
                result.Add(new ChatChannelOnOff(c));
            }

            return result;
        }

        public static bool IsPhase1Dragon(uint zoneId, uint templateId)
        {
            return zoneId == 950 && templateId >= 1100 && templateId <= 1103;
        }

        public static bool IsGuildTower(uint zoneId, uint templateId)
        {
            return zoneId == 152 && templateId == 5001;
        }

        internal static RegionEnum RegionEnumFromLanguage(string language)
        {
            if (Enum.TryParse<RegionEnum>(language, out var res))
            {
                return res;
            }
            else if (language.StartsWith("EU")) return RegionEnum.EU;
            else if (language.StartsWith("KR")) return RegionEnum.KR;
            else if (language == "THA" || language == "SE") return RegionEnum.THA;
            else return RegionEnum.EU;
        }

        public static C CurrentClassVM<C>() where C : BaseClassLayoutVM
        {
            return WindowManager.ClassWindow.VM.CurrentManager as C;
        }

    }
}
