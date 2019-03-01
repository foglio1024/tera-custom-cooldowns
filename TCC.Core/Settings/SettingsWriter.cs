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
                    SettingsHolder.BossWindowSettings.ToXElement("BossWindow"),
                    SettingsHolder.BuffWindowSettings.ToXElement("BuffWindow"),
                    SettingsHolder.CharacterWindowSettings.ToXElement("CharacterWindow"),
                    SettingsHolder.CooldownWindowSettings.ToXElement("CooldownWindow"),
                    SettingsHolder.GroupWindowSettings.ToXElement("GroupWindow"),
                    SettingsHolder.ClassWindowSettings.ToXElement("ClassWindow"),
                    BuildChatWindowSettings(),
                    SettingsHolder.FlightGaugeWindowSettings.ToXElement("FlightGaugeWindow"),
                    SettingsHolder.FloatingButtonSettings.ToXElement("FloatingButton"),
                    SettingsHolder.CivilUnrestWindowSettings.ToXElement("CivilUnrestWindow")
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
                doc.Save(Path.Combine(App.BasePath, "tcc-config.xml"));
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
                new XAttribute(nameof(SettingsHolder.BuffsDirection), SettingsHolder.BuffsDirection),
                new XAttribute(nameof(SettingsHolder.ShowAllMyAbnormalities), SettingsHolder.ShowAllMyAbnormalities), //Add My Abnormals Setting by HQ
                                                                                                                      // Character
                new XAttribute(nameof(SettingsHolder.CharacterWindowCompactMode), SettingsHolder.CharacterWindowCompactMode),
                // Cooldown
                new XAttribute(nameof(SettingsHolder.CooldownBarMode), SettingsHolder.CooldownBarMode),
                new XAttribute(nameof(SettingsHolder.ShowItemsCooldown), SettingsHolder.ShowItemsCooldown),
                new XAttribute(nameof(SettingsHolder.SkillShape), SettingsHolder.SkillShape),
                // Boss
                new XAttribute(nameof(SettingsHolder.ShowOnlyBosses), SettingsHolder.ShowOnlyBosses),
                new XAttribute(nameof(SettingsHolder.EnrageLabelMode), SettingsHolder.EnrageLabelMode),
                new XAttribute(nameof(SettingsHolder.AccurateHp), SettingsHolder.AccurateHp),
                // Class
                new XAttribute(nameof(SettingsHolder.WarriorShowTraverseCut), SettingsHolder.WarriorShowTraverseCut),
                new XAttribute(nameof(SettingsHolder.WarriorShowEdge), SettingsHolder.WarriorShowEdge),
                new XAttribute(nameof(SettingsHolder.WarriorEdgeMode), SettingsHolder.WarriorEdgeMode),
                new XAttribute(nameof(SettingsHolder.SorcererReplacesElementsInCharWindow), SettingsHolder.SorcererReplacesElementsInCharWindow),
                // Chat
                new XAttribute(nameof(SettingsHolder.MaxMessages), SettingsHolder.MaxMessages),
                new XAttribute(nameof(SettingsHolder.SpamThreshold), SettingsHolder.SpamThreshold),
                new XAttribute(nameof(SettingsHolder.FontSize), SettingsHolder.FontSize),
                new XAttribute(nameof(SettingsHolder.ShowChannel), SettingsHolder.ShowChannel),
                new XAttribute(nameof(SettingsHolder.ShowTimestamp), SettingsHolder.ShowTimestamp),
                new XAttribute(nameof(SettingsHolder.ChatTimestampSeconds), SettingsHolder.ChatTimestampSeconds),
                new XAttribute(nameof(SettingsHolder.AnimateChatMessages), SettingsHolder.AnimateChatMessages),
                new XAttribute(nameof(SettingsHolder.ChatEnabled), SettingsHolder.ChatEnabled),
                new XAttribute(nameof(SettingsHolder.ChatClickThruMode), SettingsHolder.ChatClickThruMode),
                new XAttribute(nameof(SettingsHolder.ChatScrollAmount), SettingsHolder.ChatScrollAmount),
                new XAttribute(nameof(SettingsHolder.UserExcludedSysMsg), SettingsHolder.UserExcludedSysMsg.ToCSV()),
                // Group
                new XAttribute(nameof(SettingsHolder.IgnoreMeInGroupWindow), SettingsHolder.IgnoreMeInGroupWindow),
                new XAttribute(nameof(SettingsHolder.IgnoreGroupBuffs), SettingsHolder.IgnoreGroupBuffs),
                new XAttribute(nameof(SettingsHolder.IgnoreGroupDebuffs), SettingsHolder.IgnoreGroupDebuffs),
                new XAttribute(nameof(SettingsHolder.DisablePartyMP), SettingsHolder.DisablePartyMP),
                new XAttribute(nameof(SettingsHolder.DisablePartyHP), SettingsHolder.DisablePartyHP),
                new XAttribute(nameof(SettingsHolder.DisablePartyAbnormals), SettingsHolder.DisablePartyAbnormals),
                new XAttribute(nameof(SettingsHolder.ShowOnlyAggroStacks), SettingsHolder.ShowOnlyAggroStacks),
                new XAttribute(nameof(SettingsHolder.GroupSizeThreshold), SettingsHolder.GroupSizeThreshold),
                new XAttribute(nameof(SettingsHolder.HideHpThreshold), SettingsHolder.HideHpThreshold),
                new XAttribute(nameof(SettingsHolder.HideMpThreshold), SettingsHolder.HideMpThreshold),
                new XAttribute(nameof(SettingsHolder.DisableAbnormalitiesThreshold), SettingsHolder.DisableAbnormalitiesThreshold),
                new XAttribute(nameof(SettingsHolder.HideBuffsThreshold), SettingsHolder.HideBuffsThreshold),
                new XAttribute(nameof(SettingsHolder.HideDebuffsThreshold), SettingsHolder.HideDebuffsThreshold),
                new XAttribute(nameof(SettingsHolder.ShowMembersLaurels), SettingsHolder.ShowMembersLaurels),
                new XAttribute(nameof(SettingsHolder.ShowGroupWindowDetails), SettingsHolder.ShowGroupWindowDetails),
                new XAttribute(nameof(SettingsHolder.ShowAwakenIcon), SettingsHolder.ShowAwakenIcon),
                new XAttribute(nameof(SettingsHolder.ShowAllGroupAbnormalities), SettingsHolder.ShowAllGroupAbnormalities),
                // Misc
                new XAttribute(nameof(SettingsHolder.ForceSoftwareRendering), SettingsHolder.ForceSoftwareRendering),
                new XAttribute(nameof(SettingsHolder.HighPriority), SettingsHolder.HighPriority),
                new XAttribute(nameof(SettingsHolder.LastRun), DateTime.Now),
                new XAttribute(nameof(SettingsHolder.LastLanguage), SettingsHolder.LastLanguage),
                new XAttribute(nameof(SettingsHolder.DiscordWebhookEnabled), SettingsHolder.DiscordWebhookEnabled),
                new XAttribute(nameof(SettingsHolder.Webhook), SettingsHolder.Webhook),
                new XAttribute(nameof(SettingsHolder.WebhookMessage), SettingsHolder.WebhookMessage),
                new XAttribute(nameof(SettingsHolder.TwitchName), SettingsHolder.TwitchName),
                new XAttribute(nameof(SettingsHolder.TwitchToken), SettingsHolder.TwitchToken),
                new XAttribute(nameof(SettingsHolder.TwitchChannelName), SettingsHolder.TwitchChannelName),
                new XAttribute(nameof(SettingsHolder.StatSent), SettingsHolder.StatSent),
                new XAttribute(nameof(SettingsHolder.ShowFlightEnergy), SettingsHolder.ShowFlightEnergy),
                new XAttribute(nameof(SettingsHolder.LfgEnabled), SettingsHolder.LfgEnabled),
                new XAttribute(nameof(SettingsHolder.UseHotkeys), SettingsHolder.UseHotkeys),
                new XAttribute(nameof(SettingsHolder.HideHandles), SettingsHolder.HideHandles),
                new XAttribute(nameof(SettingsHolder.ShowTradeLfg), SettingsHolder.ShowTradeLfg),
                new XAttribute(nameof(SettingsHolder.LanguageOverride), SettingsHolder.LanguageOverride),
                new XAttribute(nameof(SettingsHolder.FlightGaugeRotation), SettingsHolder.FlightGaugeRotation),
                new XAttribute(nameof(SettingsHolder.FlipFlightGauge), SettingsHolder.FlipFlightGauge),
                new XAttribute(nameof(SettingsHolder.AbnormalityShape), SettingsHolder.AbnormalityShape),
                new XAttribute(nameof(SettingsHolder.Npcap), SettingsHolder.Npcap),
                new XAttribute(nameof(SettingsHolder.EthicalMode), SettingsHolder.EthicalMode),
                new XAttribute(nameof(SettingsHolder.CheckOpcodesHash), SettingsHolder.CheckOpcodesHash),
                new XAttribute(nameof(SettingsHolder.ShowNotificationBubble), SettingsHolder.ShowNotificationBubble),
                new XAttribute(nameof(SettingsHolder.FpsAtGuardian), SettingsHolder.FpsAtGuardian),
                new XAttribute(nameof(SettingsHolder.ExperimentalNotification), SettingsHolder.ExperimentalNotification)
            );
        }



        private static XElement BuildGroupAbnormalsXElement()
        {
            var result = new XElement(nameof(SettingsHolder.GroupAbnormals));
            foreach (var pair in SettingsHolder.GroupAbnormals)
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
            var result = new XElement(nameof(SettingsHolder.MyAbnormals));
            foreach (var pair in SettingsHolder.MyAbnormals)
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
                //cw.Dispatcher.Invoke(() =>
                //{
                //    cw.UpdateSettings();
                //});
                result.Add(new XElement("ChatWindow", cw.WindowSettings.ToXElement("ChatWindow")));
            });
            return result;
        }
    }
}