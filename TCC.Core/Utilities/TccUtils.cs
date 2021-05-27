using Nostrum;
using Nostrum.Extensions;
using Nostrum.WinAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Interop;
using TCC.R;
using TCC.UI;
using TCC.Utils;
using TCC.ViewModels;
using TCC.ViewModels.ClassManagers;
using TeraDataLite;
using Brushes = TCC.R.Brushes;
using Colors = TCC.R.Colors;

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
                    ? Game.NearbyPlayers[pSource].Item1
                    : "unknown";
        }

        public static Class GetEntityClass(ulong pSource)
        {
            return Game.NearbyPlayers.ContainsKey(pSource)
                    ? Game.NearbyPlayers[pSource].Item2
                    : Class.None;
        }

        public static bool IsEntitySpawned(ulong pSource)
        {
            return Game.NearbyNPC.ContainsKey(pSource) || Game.NearbyPlayers.ContainsKey(pSource);
        }

        public static bool IsEntitySpawned(uint zoneId, uint templateId)
        {
            var name = Game.DB!.MonsterDatabase.GetMonsterName(templateId, zoneId);
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

        public static TClassLayoutVM? CurrentClassVM<TClassLayoutVM>() where TClassLayoutVM : BaseClassLayoutVM
        {
            return WindowManager.ViewModels.ClassVM.CurrentManager is TClassLayoutVM ret ? ret : null;
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
        /// <returns>TCC version as "TCC vX.Y.Z-b"</returns>
        public static string GetTccVersion()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            if (v == null) throw new InvalidOperationException("Unable to retrieve TCC version.");
            return $"TCC v{v.Major}.{v.Minor}.{v.Build}{(App.Beta ? "-b" : "")}";
        }

        public static string GradeToColorString(RareGrade g)
        {
            return g switch
            {
                RareGrade.Common => Colors.ItemCommonColor.ToHex(),
                RareGrade.Uncommon => Colors.ItemUncommonColor.ToHex(),
                RareGrade.Rare => Colors.ItemRareColor.ToHex(),
                RareGrade.Superior => Colors.ItemSuperiorColor.ToHex(),
                RareGrade.Heroic => Colors.ItemHeroicColor.ToHex(),
                _ => ""
            };
        }

        public static bool CheckMention(string text)
        {
            try
            {
                return App.Settings.MentionMode switch
                {
                    MentionMode.Current => (text.IndexOf(Game.Me.Name, StringComparison.InvariantCultureIgnoreCase) != -1),
                    MentionMode.All => Game.Account.Characters.Where(c => !c.Hidden).Any(ch => text.IndexOf(ch.Name, StringComparison.InvariantCultureIgnoreCase) != -1),
                    _ => false
                };
            }
            catch { return false; }
        }

        public static void CheckDiscordNotify(string message, string discordUsername)
        {
            if (FocusManager.IsForeground) return;
            if (!App.Settings.WebhookEnabledMentions) return;
            //var txt = GetPlainText(message).UnescapeHtml();
            //var chStr = new ChatChannelToName().Convert(ch, null, null, null);

            Discord.FireWebhook(App.Settings.WebhookUrlMentions, message, discordUsername, App.Settings.LastAccountNameHash); //string.IsNullOrEmpty(discordTextOverride) ? $"**{author}** `{chStr}`\n{txt}" : discordTextOverride);
        }

        public static void CheckWindowNotify(string message, string title)
        {
            if (FocusManager.IsForeground) return;
            if (!App.Settings.BackgroundNotifications) return;
            //var txt = GetPlainText(message).UnescapeHtml();
            //var chStr = new ChatChannelToName().Convert(ch, null, null, null);

            Log.N(title, message /*string.IsNullOrEmpty(titleOverride) ? $"{chStr} - {author}" : titleOverride, $"{txt}"*/, NotificationType.None, 6000);
        }

        // TODO: move to nostrum
        private const uint StdOutputHandle = 0xFFFFFFF5;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(uint nStdHandle, IntPtr handle);

        public static void CreateConsole()
        {
            Kernel32.AllocConsole();

            // stdout's handle seems to always be equal to 7
            var defaultStdout = new IntPtr(7);
            var currentStdout = GetStdHandle(StdOutputHandle);

            if (currentStdout != defaultStdout)
                // reset stdout
                SetStdHandle(StdOutputHandle, defaultStdout);

            // reopen stdout
            TextWriter writer = new StreamWriter(Console.OpenStandardOutput())
            { AutoFlush = true };
            Console.SetOut(writer);
        }

        public static SolidColorBrush ChatChannelToBrush(ChatChannel ch)
        {
            return ch switch
            {
                ChatChannel.Say => Brushes.ChatSayBrush,
                ChatChannel.Party => Brushes.ChatPartyBrush,
                ChatChannel.Guild => Brushes.ChatGuildBrush,
                ChatChannel.Area => Brushes.ChatAreaBrush,
                ChatChannel.Trade => Brushes.ChatTradeBrush,
                ChatChannel.Greet => Brushes.ChatGreetBrush,
                ChatChannel.Angler => Brushes.ChatGreetBrush,
                ChatChannel.PartyNotice => Brushes.ChatPartyNoticeBrush,
                ChatChannel.RaidNotice => Brushes.ChatRaidNoticeBrush,
                ChatChannel.Emote => Brushes.ChatEmoteBrush,
                ChatChannel.Global => Brushes.ChatGlobalBrush,
                ChatChannel.Raid => Brushes.ChatRaidBrush,
                ChatChannel.GuildAdvertising => Brushes.ChatGuildAdBrush,
                ChatChannel.Private1 => Brushes.ChatPrivateBrush,
                ChatChannel.Private2 => Brushes.ChatPrivateBrush,
                ChatChannel.Private3 => Brushes.ChatPrivateBrush,
                ChatChannel.Private4 => Brushes.ChatPrivateBrush,
                ChatChannel.Private5 => Brushes.ChatPrivateBrush,
                ChatChannel.Private6 => Brushes.ChatPrivateBrush,
                ChatChannel.Private7 => Brushes.ChatProxyBrush,
                ChatChannel.Private8 => Brushes.ChatProxyBrush,
                ChatChannel.SentWhisper => Brushes.ChatWhisperBrush,
                ChatChannel.ReceivedWhisper => Brushes.ChatWhisperBrush,
                ChatChannel.System => Brushes.ChatSystemGenericBrush,
                ChatChannel.Notify => Brushes.ChatSystemNotifyBrush,
                ChatChannel.Event => Brushes.ChatSystemEventBrush,
                ChatChannel.Error => Brushes.ChatSystemErrorBrush,
                ChatChannel.Group => Brushes.ChatSystemGroupBrush,
                ChatChannel.GuildNotice => Brushes.ChatGuildBrush,
                ChatChannel.Deathmatch => Brushes.ChatSystemDeathmatchBrush,
                ChatChannel.ContractAlert => Brushes.ChatSystemContractAlertBrush,
                ChatChannel.GroupAlerts => Brushes.ChatSystemGroupAlertBrush,
                ChatChannel.Loot => Brushes.ChatSystemLootBrush,
                ChatChannel.Exp => Brushes.ChatSystemExpBrush,
                ChatChannel.Money => Brushes.ChatSystemMoneyBrush,
                ChatChannel.TradeRedirect => Brushes.ChatTradeBrush,
                ChatChannel.Enchant => Brushes.EnchantLowBrush,
                ChatChannel.Laurel => Brushes.EnchantHighBrush,
                ChatChannel.RaidLeader => Brushes.ChatRaidNoticeBrush,
                ChatChannel.TCC => Brushes.MainBrush,
                ChatChannel.Bargain => Brushes.ChatSystemBargainBrush,
                ChatChannel.Apply => Brushes.ChatMegaphoneBrush,
                ChatChannel.LFG => Brushes.ChatMegaphoneBrush,
                ChatChannel.Megaphone => Brushes.ChatMegaphoneBrush,
                ChatChannel.Death => Brushes.HpBrush,
                ChatChannel.Damage => Brushes.HpBrush,
                ChatChannel.Ress => Brushes.GreenBrush,
                ChatChannel.Quest => Brushes.ChatSystemQuestBrush,
                ChatChannel.Friend => Brushes.ChatSystemFriendBrush,
                ChatChannel.Twitch => Brushes.TwitchBrush,
                ChatChannel.WorldBoss => Brushes.ChatSystemWorldBossBrush,
                ChatChannel.Guardian => Brushes.GuardianBrush,
                _ => Brushes.ChatSystemGenericBrush
            };
        }

        public static string ChatChannelToName(ChatChannel ch)
        {
            return ch switch
            {
                ChatChannel.PartyNotice => "Notice",
                ChatChannel.RaidNotice => "Notice",
                ChatChannel.GuildAdvertising => "G. Ad",
                ChatChannel.Megaphone => "Megaphone",
                ChatChannel.Private1 => (ChatManager.Instance.PrivateChannels[0].Name),
                ChatChannel.Private2 => (ChatManager.Instance.PrivateChannels[1].Name),
                ChatChannel.Private3 => (ChatManager.Instance.PrivateChannels[2].Name),
                ChatChannel.Private4 => (ChatManager.Instance.PrivateChannels[3].Name),
                ChatChannel.Private5 => (ChatManager.Instance.PrivateChannels[4].Name),
                ChatChannel.Private6 => (ChatManager.Instance.PrivateChannels[5].Name),
                ChatChannel.Private7 => (ChatManager.Instance.PrivateChannels[6].Name),
                ChatChannel.Private8 => (ChatManager.Instance.PrivateChannels[7].Name),
                ChatChannel.Notify => "Info",
                ChatChannel.Error => "Alert",
                ChatChannel.GuildNotice => "Guild",
                ChatChannel.GroupAlerts => "Group",
                ChatChannel.TradeRedirect => "Global",
                ChatChannel.Enchant => "Gear",
                ChatChannel.RaidLeader => "Leader",
                ChatChannel.Bargain => "Offer",
                ChatChannel.WorldBoss => "W.B.",
                ChatChannel.SystemDefault => "System",
                ChatChannel.Damage => "Dmg",
                ChatChannel.Guardian => "G.L.",
                ChatChannel.ReceivedWhisper => "Whisper",
                _ => ch.ToString()
            };
        }

        public static void SetAlignment()
        {
            var ifLeft = SystemParameters.MenuDropAlignment;
            if (!ifLeft) return;
            var t = typeof(SystemParameters);
            var field = t.GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null) field.SetValue(null, false);
        }
    }
}