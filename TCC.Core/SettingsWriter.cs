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

namespace TCC
{
    public static class SettingsWriter
    {
        public static void Save()
        {
            var xSettings = new XElement("Settings",
                new XElement("WindowSettings",
                    Settings.BossWindowSettings.ToXElement("BossWindow"),
                    Settings.BuffWindowSettings.ToXElement("BuffWindow"),
                    Settings.CharacterWindowSettings.ToXElement("CharacterWindow"),
                    Settings.CooldownWindowSettings.ToXElement("CooldownWindow"),
                    Settings.GroupWindowSettings.ToXElement("GroupWindow"),
                    Settings.ClassWindowSettings.ToXElement("ClassWindow"),
                    BuildChatWindowSettings("ChatWindows"),
                    Settings.FlightGaugeWindowSettings.ToXElement("FlightGaugeWindow"),
                    Settings.FloatingButtonSettings.ToXElement("FloatingButton")
                    //add window here
                ),
                BuildOtherSettingsXElement(),
                BuildGroupAbnormalsXElement()
            );
            WriteSettings(xSettings);
        }
        private static void WriteSettings(XElement doc)
        {
            if (!doc.HasElements) return;
            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml")) File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml", AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml.bak", true);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");
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
                new XAttribute(nameof(Settings.BuffsDirection), Settings.BuffsDirection),
                // Character
                new XAttribute(nameof(Settings.CharacterWindowCompactMode), Settings.CharacterWindowCompactMode),
                // Cooldown
                new XAttribute(nameof(Settings.CooldownBarMode), Settings.CooldownBarMode),
                new XAttribute(nameof(Settings.ShowItemsCooldown), Settings.ShowItemsCooldown),
                // Boss
                new XAttribute(nameof(Settings.ShowOnlyBosses), Settings.ShowOnlyBosses),
                new XAttribute(nameof(Settings.EnrageLabelMode), Settings.EnrageLabelMode),
                new XAttribute(nameof(Settings.AccurateHp), Settings.AccurateHp),
                // Class
                new XAttribute(nameof(Settings.WarriorShowTraverseCut), Settings.WarriorShowTraverseCut),
                new XAttribute(nameof(Settings.WarriorShowEdge), Settings.WarriorShowEdge),
                new XAttribute(nameof(Settings.WarriorEdgeMode), Settings.WarriorEdgeMode),
                // Chat
                new XAttribute(nameof(Settings.MaxMessages), Settings.MaxMessages),
                new XAttribute(nameof(Settings.SpamThreshold), Settings.SpamThreshold),
                new XAttribute(nameof(Settings.FontSize), Settings.FontSize),
                new XAttribute(nameof(Settings.ShowChannel), Settings.ShowChannel),
                new XAttribute(nameof(Settings.ShowTimestamp), Settings.ShowTimestamp),
                new XAttribute(nameof(Settings.ChatFadeOut), Settings.ChatFadeOut),
                new XAttribute(nameof(Settings.AnimateChatMessages), Settings.AnimateChatMessages),
                new XAttribute(nameof(Settings.ChatEnabled), Settings.ChatEnabled),
                new XAttribute(nameof(Settings.ChatClickThruMode), Settings.ChatClickThruMode),
                // Group
                new XAttribute(nameof(Settings.IgnoreMeInGroupWindow), Settings.IgnoreMeInGroupWindow),
                new XAttribute(nameof(Settings.IgnoreGroupBuffs), Settings.IgnoreGroupBuffs),
                new XAttribute(nameof(Settings.IgnoreGroupDebuffs), Settings.IgnoreGroupDebuffs),
                new XAttribute(nameof(Settings.DisablePartyMP), Settings.DisablePartyMP),
                new XAttribute(nameof(Settings.DisablePartyHP), Settings.DisablePartyHP),
                new XAttribute(nameof(Settings.DisablePartyAbnormals), Settings.DisablePartyAbnormals),
                new XAttribute(nameof(Settings.ShowOnlyAggroStacks), Settings.ShowOnlyAggroStacks),
                new XAttribute(nameof(Settings.ChatWindowOpacity), Settings.ChatWindowOpacity),
                new XAttribute(nameof(Settings.GroupSizeThreshold), Settings.GroupSizeThreshold),
                new XAttribute(nameof(Settings.ShowMembersLaurels), Settings.ShowMembersLaurels),
                new XAttribute(nameof(Settings.ShowGroupWindowDetails), Settings.ShowGroupWindowDetails),
                new XAttribute(nameof(Settings.ShowAwakenIcon), Settings.ShowAwakenIcon),
                new XAttribute(nameof(Settings.ShowAllGroupAbnormalities), Settings.ShowAllGroupAbnormalities),
                // Misc
                new XAttribute(nameof(Settings.ForceSoftwareRendering), Settings.ForceSoftwareRendering),
                new XAttribute(nameof(Settings.HighPriority), Settings.HighPriority),
                new XAttribute(nameof(Settings.LastRun), DateTime.Now),
                new XAttribute(nameof(Settings.LastRegion), Settings.LastRegion),
                new XAttribute(nameof(Settings.Webhook), Settings.Webhook),
                new XAttribute(nameof(Settings.WebhookMessage), Settings.WebhookMessage),
                new XAttribute(nameof(Settings.TwitchName), Settings.TwitchName),
                new XAttribute(nameof(Settings.TwitchToken), Settings.TwitchToken),
                new XAttribute(nameof(Settings.TwitchChannelName), Settings.TwitchChannelName),
                new XAttribute(nameof(Settings.StatSent), Settings.StatSent),
                new XAttribute(nameof(Settings.ShowFlightEnergy), Settings.ShowFlightEnergy),
                new XAttribute(nameof(Settings.LfgEnabled), Settings.LfgEnabled),
                new XAttribute(nameof(Settings.UseHotkeys), Settings.UseHotkeys),
                new XAttribute(nameof(Settings.HideHandles), Settings.HideHandles),
                new XAttribute(nameof(Settings.ShowTradeLfg), Settings.ShowTradeLfg),
                new XAttribute(nameof(Settings.RegionOverride), Settings.RegionOverride),
                new XAttribute(nameof(Settings.FlightGaugeRotation), Settings.FlightGaugeRotation),
                new XAttribute(nameof(Settings.FlipFlightGauge), Settings.FlipFlightGauge)
            );
        }
        private static XElement BuildGroupAbnormalsXElement()
        {
            var result = new XElement(nameof(Settings.GroupAbnormals));
            foreach (var pair in Settings.GroupAbnormals)
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
        private static XElement BuildChatWindowSettings(string v)
        {
            var result = new XElement("ChatWindows");
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