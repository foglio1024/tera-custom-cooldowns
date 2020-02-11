using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Nostrum;
using TCC.Data;
using TCC.Data.Chat;
using TCC.R;
using TCC.ViewModels;
using TeraDataLite;

namespace TCC.Utilities
{
    public static class TccUtils
    {
        public static Geometry SvgClass(Class c)
        {
            return c switch
            {
                Class.Warrior => SVG.SvgClassWarrior,
                Class.Lancer => SVG.SvgClassLancer,
                Class.Slayer => SVG.SvgClassSlayer,
                Class.Berserker => SVG.SvgClassBerserker,
                Class.Sorcerer => SVG.SvgClassSorcerer,
                Class.Archer => SVG.SvgClassArcher,
                Class.Priest => SVG.SvgClassPriest,
                Class.Mystic => SVG.SvgClassMystic,
                Class.Reaper => SVG.SvgClassReaper,
                Class.Gunner => SVG.SvgClassGunner,
                Class.Brawler => SVG.SvgClassBrawler,
                Class.Ninja => SVG.SvgClassNinja,
                Class.Valkyrie => SVG.SvgClassValkyrie,
                _ => SVG.SvgClassCommon
            };
        }
        public static string ClassEnumToString(Class c)
        {
            return c switch
            {
                Class.Warrior => "Warrior",
                Class.Lancer => "Lancer",
                Class.Slayer => "Slayer",
                Class.Berserker => "Berserker",
                Class.Sorcerer => "Sorcerer",
                Class.Archer => "Archer",
                Class.Priest => "Priest",
                Class.Mystic => "Mystic",
                Class.Reaper => "Reaper",
                Class.Gunner => "Gunner",
                Class.Brawler => "Brawler",
                Class.Ninja => "Ninja",
                Class.Valkyrie => "Valkyrie",
                Class.Common => "All classes",
                _ => ""
            };
        }
        public static List<ChatChannelOnOff> GetEnabledChannelsList()
        {
            return EnumUtils.ListFromEnum<ChatChannel>().Select(c => new ChatChannelOnOff(c)).ToList();
        }


        public static bool IsPhase1Dragon(uint zoneId, uint templateId)
        {
            return zoneId == 950 && templateId >= 1100 && templateId <= 1103;
        }
        public static bool IsGuildTower(uint zoneId, uint templateId)
        {
            return zoneId == 152 && templateId == 5001;
        }
        public static bool IsFieldBoss(uint zone, uint template)
        {
            return (zone == 39 && template == 501) ||
                   (zone == 26 && template == 5001) ||
                   (zone == 51 && template == 4001);
        }
        public static bool IsWorldBoss(ushort zoneId, uint templateId)
        {
            return zoneId == 10 && templateId == 99 ||
                   zoneId == 4 && templateId == 5011 ||
                   zoneId == 51 && templateId == 7011 ||
                   zoneId == 52 && templateId == 9050 ||
                   zoneId == 57 && templateId == 33 ||
                   zoneId == 38 && templateId == 35;
        }

        public static string GetEntityName(ulong pSource)
        {
            return Game.NearbyNPC.ContainsKey(pSource)
                 ? Game.NearbyNPC[pSource]
                 : Game.NearbyPlayers.ContainsKey(pSource)
                    ? Game.NearbyPlayers[pSource]
                    : "unknown";
        }

        public static bool IsEntitySpawned(ulong pSource)
        {
            return Game.NearbyNPC.ContainsKey(pSource) || Game.NearbyPlayers.ContainsKey(pSource);
        }

        public static bool IsEntitySpawned(uint zoneId, uint templateId)
        {
            var name = Game.DB.MonsterDatabase.GetName(templateId, zoneId);
            return name != "Unknown" && Game.NearbyNPC.ContainsValue(name);
        }

        internal static RegionEnum RegionEnumFromLanguage(string language)
        {
            if (Enum.TryParse<RegionEnum>(language, out var res)) return res;
            if (language.StartsWith("EU")) return RegionEnum.EU;
            if (language.StartsWith("KR")) return RegionEnum.KR;
            if (language == "THA" || language == "SE") return RegionEnum.THA;
            return RegionEnum.EU;
        }

        public static TClassLayoutVM CurrentClassVM<TClassLayoutVM>() where TClassLayoutVM : BaseClassLayoutVM
        {
            return WindowManager.ViewModels.ClassVM.CurrentManager as TClassLayoutVM;
        }

        public static Race RaceFromTemplateId(int templateId)
        {
            return (Race)((templateId - 10000) / 100);
        }

        public static Class ClassFromModel(uint model)
        {
            var c = model % 100 - 1;
            return (Class)c;
        }

        /// <summary>
        /// Retrieves TCC version from the executing assembly.
        /// </summary>
        /// <returns>TCC version as "TCC vX.Y.Z-e"</returns>
        public static string GetTccVersion()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"TCC v{v.Major}.{v.Minor}.{v.Build}{(App.Beta ? "-e" : "")}";

        }
    }
}
