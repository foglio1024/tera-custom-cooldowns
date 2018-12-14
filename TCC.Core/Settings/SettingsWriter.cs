using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.Settings
{
    public static class SettingsWriter
    {
        public static void Save()
        {
            var xSettings = new XElement("Settings",
                new XElement("WindowSettings",
                    SettingsStorage.BossWindowSettings.ToXElement("BossWindow"),
                    SettingsStorage.BuffWindowSettings.ToXElement("BuffWindow"),
                    SettingsStorage.CharacterWindowSettings.ToXElement("CharacterWindow"),
                    SettingsStorage.CooldownWindowSettings.ToXElement("CooldownWindow"),
                    SettingsStorage.GroupWindowSettings.ToXElement("GroupWindow"),
                    SettingsStorage.ClassWindowSettings.ToXElement("ClassWindow"),
                    BuildChatWindowSettings(),
                    SettingsStorage.FlightGaugeWindowSettings.ToXElement("FlightGaugeWindow"),
                    SettingsStorage.FloatingButtonSettings.ToXElement("FloatingButton"),
                    SettingsStorage.CivilUnrestWindowSettings.ToXElement("CivilUnrestWindow")
                    //add window here
                ),
                BuildOtherSettingsXElement(),
                BuildGroupAbnormalsXElement(),  //Add , by HQ
                BuildMyAbnormalsXElement()      //Add My Abnormals Setting by HQ
            );
            WriteSettings(xSettings);
        }
        private static void WriteSettings(XElement doc)
        {
            if (!doc.HasElements) return;
            try
            {
                if (File.Exists(Path.Combine(App.BasePath, "tcc-config.xml")))
                    File.Copy(Path.Combine(App.BasePath, "tcc-config.xml"), Path.Combine(App.BasePath, "tcc-config.xml.bak"), true);
                doc.Save(Path.Combine(App.BasePath,"tcc-config.xml"));
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", "Could not write settings data to tcc-config.xml. File is being used by another process. Try again?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes) WriteSettings(doc);
            }
        }
        public static XElement BuildChatTabsXElement(List<Tab> tabList)
        {
            var result = new XElement("Tabs");
            foreach (var tab in tabList)
            {
                var tabName = new XAttribute("name", tab.TabName);
                var tabElement = new XElement("Tab", tabName);
                foreach (var ch in tab.Channels)
                {
                    tabElement.Add(new XElement("Channel", new XAttribute("value", ch)));
                }
                foreach (var ch in tab.Authors)
                {
                    tabElement.Add(new XElement("Author", new XAttribute("value", ch)));
                }
                foreach (var ch in tab.ExcludedChannels)
                {
                    tabElement.Add(new XElement("ExcludedChannel", new XAttribute("value", ch)));
                }
                foreach (var ch in tab.ExcludedAuthors)
                {
                    tabElement.Add(new XElement("ExcludedAuthor", new XAttribute("value", ch)));
                }
                result.Add(tabElement);

            }
            return result;
        }
        private static XElement BuildOtherSettingsXElement()
        {
            /* add new settings down here */
            return new XElement("OtherSettings",
                // Buff
                new XAttribute(nameof(SettingsStorage.BuffsDirection), SettingsStorage.BuffsDirection),
                new XAttribute(nameof(SettingsStorage.ShowAllMyAbnormalities), SettingsStorage.ShowAllMyAbnormalities), //Add My Abnormals Setting by HQ
                // Character
                new XAttribute(nameof(SettingsStorage.CharacterWindowCompactMode), SettingsStorage.CharacterWindowCompactMode),
                // Cooldown
                new XAttribute(nameof(SettingsStorage.CooldownBarMode), SettingsStorage.CooldownBarMode),
                new XAttribute(nameof(SettingsStorage.ShowItemsCooldown), SettingsStorage.ShowItemsCooldown),
                new XAttribute(nameof(SettingsStorage.SkillShape), SettingsStorage.SkillShape),
                // Boss
                new XAttribute(nameof(SettingsStorage.ShowOnlyBosses), SettingsStorage.ShowOnlyBosses),
                new XAttribute(nameof(SettingsStorage.EnrageLabelMode), SettingsStorage.EnrageLabelMode),
                new XAttribute(nameof(SettingsStorage.AccurateHp), SettingsStorage.AccurateHp),
                // Class
                new XAttribute(nameof(SettingsStorage.WarriorShowTraverseCut), SettingsStorage.WarriorShowTraverseCut),
                new XAttribute(nameof(SettingsStorage.WarriorShowEdge), SettingsStorage.WarriorShowEdge),
                new XAttribute(nameof(SettingsStorage.WarriorEdgeMode), SettingsStorage.WarriorEdgeMode),
                new XAttribute(nameof(SettingsStorage.SorcererReplacesElementsInCharWindow), SettingsStorage.SorcererReplacesElementsInCharWindow),
                // Chat
                new XAttribute(nameof(SettingsStorage.MaxMessages), SettingsStorage.MaxMessages),
                new XAttribute(nameof(SettingsStorage.SpamThreshold), SettingsStorage.SpamThreshold),
                new XAttribute(nameof(SettingsStorage.FontSize), SettingsStorage.FontSize),
                new XAttribute(nameof(SettingsStorage.ShowChannel), SettingsStorage.ShowChannel),
                new XAttribute(nameof(SettingsStorage.ShowTimestamp), SettingsStorage.ShowTimestamp),
                //new XAttribute(nameof(Settings.ChatFadeOut), Settings.ChatFadeOut),
                new XAttribute(nameof(SettingsStorage.AnimateChatMessages), SettingsStorage.AnimateChatMessages),
                new XAttribute(nameof(SettingsStorage.ChatEnabled), SettingsStorage.ChatEnabled),
                new XAttribute(nameof(SettingsStorage.ChatClickThruMode), SettingsStorage.ChatClickThruMode),
                // Group
                new XAttribute(nameof(SettingsStorage.IgnoreMeInGroupWindow), SettingsStorage.IgnoreMeInGroupWindow),
                new XAttribute(nameof(SettingsStorage.IgnoreGroupBuffs), SettingsStorage.IgnoreGroupBuffs),
                new XAttribute(nameof(SettingsStorage.IgnoreGroupDebuffs), SettingsStorage.IgnoreGroupDebuffs),
                new XAttribute(nameof(SettingsStorage.DisablePartyMP), SettingsStorage.DisablePartyMP),
                new XAttribute(nameof(SettingsStorage.DisablePartyHP), SettingsStorage.DisablePartyHP),
                new XAttribute(nameof(SettingsStorage.DisablePartyAbnormals), SettingsStorage.DisablePartyAbnormals),
                new XAttribute(nameof(SettingsStorage.ShowOnlyAggroStacks), SettingsStorage.ShowOnlyAggroStacks),
                //new XAttribute(nameof(Settings.ChatWindowOpacity), Settings.ChatWindowOpacity),
                new XAttribute(nameof(SettingsStorage.GroupSizeThreshold), SettingsStorage.GroupSizeThreshold),
                new XAttribute(nameof(SettingsStorage.ShowMembersLaurels), SettingsStorage.ShowMembersLaurels),
                new XAttribute(nameof(SettingsStorage.ShowGroupWindowDetails), SettingsStorage.ShowGroupWindowDetails),
                new XAttribute(nameof(SettingsStorage.ShowAwakenIcon), SettingsStorage.ShowAwakenIcon),
                new XAttribute(nameof(SettingsStorage.ShowAllGroupAbnormalities), SettingsStorage.ShowAllGroupAbnormalities),
                // Misc
                new XAttribute(nameof(SettingsStorage.ForceSoftwareRendering), SettingsStorage.ForceSoftwareRendering),
                new XAttribute(nameof(SettingsStorage.HighPriority), SettingsStorage.HighPriority),
                new XAttribute(nameof(SettingsStorage.LastRun), DateTime.Now),
                new XAttribute(nameof(SettingsStorage.LastRegion), SettingsStorage.LastRegion),
                new XAttribute(nameof(SettingsStorage.DiscordWebhookEnabled), SettingsStorage.DiscordWebhookEnabled),
                new XAttribute(nameof(SettingsStorage.Webhook), SettingsStorage.Webhook),
                new XAttribute(nameof(SettingsStorage.WebhookMessage), SettingsStorage.WebhookMessage),
                new XAttribute(nameof(SettingsStorage.TwitchName), SettingsStorage.TwitchName),
                new XAttribute(nameof(SettingsStorage.TwitchToken), SettingsStorage.TwitchToken),
                new XAttribute(nameof(SettingsStorage.TwitchChannelName), SettingsStorage.TwitchChannelName),
                new XAttribute(nameof(SettingsStorage.StatSent), SettingsStorage.StatSent),
                new XAttribute(nameof(SettingsStorage.ShowFlightEnergy), SettingsStorage.ShowFlightEnergy),
                new XAttribute(nameof(SettingsStorage.LfgEnabled), SettingsStorage.LfgEnabled),
                new XAttribute(nameof(SettingsStorage.UseHotkeys), SettingsStorage.UseHotkeys),
                new XAttribute(nameof(SettingsStorage.HideHandles), SettingsStorage.HideHandles),
                new XAttribute(nameof(SettingsStorage.ShowTradeLfg), SettingsStorage.ShowTradeLfg),
                new XAttribute(nameof(SettingsStorage.RegionOverride), SettingsStorage.RegionOverride),
                new XAttribute(nameof(SettingsStorage.FlightGaugeRotation), SettingsStorage.FlightGaugeRotation),
                new XAttribute(nameof(SettingsStorage.FlipFlightGauge), SettingsStorage.FlipFlightGauge),
                new XAttribute(nameof(SettingsStorage.AbnormalityShape), SettingsStorage.AbnormalityShape),
                new XAttribute(nameof(SettingsStorage.Npcap), SettingsStorage.Npcap),
                new XAttribute(nameof(SettingsStorage.EthicalMode), SettingsStorage.EthicalMode),
                new XAttribute(nameof(SettingsStorage.CheckOpcodesHash), SettingsStorage.CheckOpcodesHash),
                new XAttribute(nameof(SettingsStorage.ShowNotificationBubble), SettingsStorage.ShowNotificationBubble)
            );
        }
        private static XElement BuildGroupAbnormalsXElement()
        {
            var result = new XElement(nameof(SettingsStorage.GroupAbnormals));
            foreach (var pair in SettingsStorage.GroupAbnormals)
            {
                var c = pair.Key;
                var sb = new StringBuilder();
                foreach (var u in pair.Value)
                {
                    sb.Append(u);
                    if (pair.Value.Count != pair.Value.IndexOf(u) + 1) sb.Append(',');
                }
                var cl = new XAttribute("class", c);
                var xel = new XElement("Abnormals", cl, sb.ToString());
                result.Add(xel);
            }
            return result;
        }
        //Add My Abnormals Setting by HQ ===========================================================
        private static XElement BuildMyAbnormalsXElement()
        {
            var result = new XElement(nameof(SettingsStorage.MyAbnormals));
            foreach (var pair in SettingsStorage.MyAbnormals)
            {
                var c = pair.Key;
                var sb = new StringBuilder();
                foreach (var u in pair.Value)
                {
                    sb.Append(u);
                    if (pair.Value.Count != pair.Value.IndexOf(u) + 1) sb.Append(',');
                }
                var cl = new XAttribute("class", c);
                var xel = new XElement("Abnormals", cl, sb.ToString());
                result.Add(xel);
            }
            return result;
        }
        //==========================================================================================
        private static XElement BuildChatWindowSettings()
        {
            var result = new XElement("ChatWindows");
            if (ChatWindowManager.Instance.ChatWindows.Count == 0) return result;
            ChatWindowManager.Instance.ChatWindows.ToList().ForEach(cw =>
            {
                if (cw.VM.Tabs.Count == 0) return;
                cw.UpdateSettings();
                result.Add(new XElement("ChatWindow", cw.WindowSettings.ToXElement("ChatWindow")));
            });
            return result;
        }
    }
}