using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using FoglioUtils.Extensions;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.Settings
{
    public class XmlSettingsWriter : SettingsWriterBase
    {
        public XmlSettingsWriter()
        {
            FileName = SettingsGlobals.XmlFileName;
        }
        public override void Save()
        {
            var xSettings = new XElement("Settings",
                new XElement("WindowSettings",
                     App.Settings.NpcWindowSettings.ToXElement("BossWindow"),
                     App.Settings.BuffWindowSettings.ToXElement("BuffWindow"),
                     App.Settings.CharacterWindowSettings.ToXElement("CharacterWindow"),
                     App.Settings.CooldownWindowSettings.ToXElement("CooldownWindow"),
                     App.Settings.GroupWindowSettings.ToXElement("GroupWindow"),
                     App.Settings.ClassWindowSettings.ToXElement("ClassWindow"),
                    BuildChatWindowSettings(),
                     App.Settings.FlightGaugeWindowSettings.ToXElement("FlightGaugeWindow"),
                     App.Settings.FloatingButtonSettings.ToXElement("FloatingButton"),
                     App.Settings.CivilUnrestWindowSettings.ToXElement("CivilUnrestWindow")
                //add window here
                ),
                BuildOtherSettingsXElement(),
                BuildGroupAbnormalsXElement(),  //Add , by HQ
                BuildMyAbnormalsXElement()      //Add My Abnormals Setting by HQ
            );
            WriteSettings(xSettings);
        }
        private void WriteSettings(XElement doc)
        {
            if (!doc.HasElements) return;
            try
            {
                if (File.Exists(Path.Combine(App.BasePath, FileName)))
                    File.Copy(Path.Combine(App.BasePath, FileName), Path.Combine(App.BasePath, FileName + ".bak"), true);
                doc.Save(Path.Combine(App.BasePath, FileName));
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", "Could not write settings data to tcc-config.xml. File is being used by another process. Try again?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes) WriteSettings(doc);
            }
        }
        public static XElement BuildChatTabsXElement(List<TabInfo> tabList)
        {
            var result = new XElement("Tabs");
            foreach (var tab in tabList)
            {
                var tabName = new XAttribute("name", tab.Name);
                var tabElement = new XElement("Tab", tabName);
                foreach (var ch in tab.ShowedChannels)
                {
                    tabElement.Add(new XElement("Channel", new XAttribute("value", ch)));
                }
                foreach (var ch in tab.ShowedAuthors)
                {
                    tabElement.Add(new XElement("Author", new XAttribute("value", ch)));
                }
                foreach (var ch in tab.HiddenChannels)
                {
                    tabElement.Add(new XElement("ExcludedChannel", new XAttribute("value", ch)));
                }
                foreach (var ch in tab.HiddenAuthors)
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
                new XAttribute(nameof(BuffWindowSettings.Direction), App.Settings.BuffWindowSettings.Direction),
                new XAttribute(nameof(BuffWindowSettings.ShowAll), App.Settings.BuffWindowSettings.ShowAll), //Add My Abnormals Setting by HQ
                                                                                                                       // Character
                new XAttribute("CharacterWindowCompactMode", App.Settings.CharacterWindowSettings.CompactMode),
                // Cooldown
                new XAttribute(nameof(CooldownWindowSettings.Mode), App.Settings.CooldownWindowSettings.Mode),
                new XAttribute(nameof(CooldownWindowSettings.ShowItems), App.Settings.CooldownWindowSettings.ShowItems),
                new XAttribute(nameof(SettingsContainer.SkillShape), App.Settings.SkillShape),
                // Boss
                new XAttribute(nameof(NpcWindowSettings.HideAdds), App.Settings.NpcWindowSettings.HideAdds),
                new XAttribute(nameof(NpcWindowSettings.EnrageLabelMode), App.Settings.NpcWindowSettings.EnrageLabelMode),
                new XAttribute(nameof(NpcWindowSettings.AccurateHp), App.Settings.NpcWindowSettings.AccurateHp),
                // Class
                new XAttribute(nameof(ClassWindowSettings.WarriorShowTraverseCut), App.Settings.ClassWindowSettings.WarriorShowTraverseCut),
                new XAttribute(nameof(ClassWindowSettings.WarriorShowEdge), App.Settings.ClassWindowSettings.WarriorShowEdge),
                new XAttribute(nameof(ClassWindowSettings.WarriorEdgeMode), App.Settings.ClassWindowSettings.WarriorEdgeMode),
                new XAttribute(nameof(ClassWindowSettings.SorcererReplacesElementsInCharWindow), App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow),
                // Chat
                new XAttribute(nameof(SettingsContainer.MaxMessages), App.Settings.MaxMessages),
                new XAttribute(nameof(SettingsContainer.SpamThreshold), App.Settings.SpamThreshold),
                new XAttribute(nameof(SettingsContainer.FontSize), App.Settings.FontSize),
                new XAttribute(nameof(SettingsContainer.ShowChannel), App.Settings.ShowChannel),
                new XAttribute(nameof(SettingsContainer.ShowTimestamp), App.Settings.ShowTimestamp),
                new XAttribute(nameof(SettingsContainer.ChatTimestampSeconds), App.Settings.ChatTimestampSeconds),
                new XAttribute(nameof(SettingsContainer.AnimateChatMessages), App.Settings.AnimateChatMessages),
                new XAttribute(nameof(SettingsContainer.ChatEnabled), App.Settings.ChatEnabled),
                //new XAttribute(nameof(SettingsContainer.ChatClickThruMode), App.Settings.ChatClickThruMode),
                new XAttribute(nameof(SettingsContainer.ChatScrollAmount), App.Settings.ChatScrollAmount),
                new XAttribute(nameof(SettingsContainer.UserExcludedSysMsg), App.Settings.UserExcludedSysMsg.ToCSV()),
                // Group
                new XAttribute("IgnoreMeInGroupWindow", App.Settings.GroupWindowSettings.IgnoreMe),
                new XAttribute("ShowOnlyAggroStacks", App.Settings.GroupWindowSettings.ShowOnlyAggroStacks),
                new XAttribute("GroupSizeThreshold", App.Settings.GroupWindowSettings.GroupSizeThreshold),
                new XAttribute("HideHpThreshold", App.Settings.GroupWindowSettings.HideHpThreshold),
                new XAttribute("HideMpThreshold", App.Settings.GroupWindowSettings.HideMpThreshold),
                new XAttribute("DisableAbnormalitiesThreshold", App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold),
                new XAttribute("HideBuffsThreshold", App.Settings.GroupWindowSettings.HideBuffsThreshold),
                new XAttribute("HideDebuffsThreshold", App.Settings.GroupWindowSettings.HideDebuffsThreshold),
                new XAttribute("ShowMembersLaurels", App.Settings.GroupWindowSettings.ShowLaurels),
                new XAttribute("ShowGroupWindowDetails", App.Settings.GroupWindowSettings.ShowDetails),
                new XAttribute("ShowAwakenIcon", App.Settings.GroupWindowSettings.ShowAwakenIcon),
                new XAttribute("ShowAllGroupAbnormalities", App.Settings.GroupWindowSettings.ShowAllAbnormalities),
                new XAttribute("ShowMembersHpNumbers", App.Settings.GroupWindowSettings.ShowHpLabels),
                new XAttribute("GroupWindowLayout", App.Settings.GroupWindowSettings.Layout),
                // Misc
                new XAttribute(nameof(SettingsContainer.ForceSoftwareRendering), App.Settings.ForceSoftwareRendering),
                new XAttribute(nameof(SettingsContainer.HighPriority), App.Settings.HighPriority),
                new XAttribute(nameof(SettingsContainer.LastRun), DateTime.Now),
                new XAttribute(nameof(SettingsContainer.StatSentTime), App.Settings.StatSentTime),
                new XAttribute(nameof(SettingsContainer.StatSentVersion), App.Settings.StatSentVersion),
                new XAttribute(nameof(SettingsContainer.LastLanguage), App.Settings.LastLanguage),
                new XAttribute(nameof(SettingsContainer.WebhookEnabledGuildBam), App.Settings.WebhookEnabledGuildBam),
                new XAttribute(nameof(SettingsContainer.WebhookUrlGuildBam), App.Settings.WebhookUrlGuildBam),
                new XAttribute(nameof(SettingsContainer.WebhookMessageGuildBam), App.Settings.WebhookMessageGuildBam),
                new XAttribute(nameof(SettingsContainer.WebhookEnabledFieldBoss), App.Settings.WebhookEnabledFieldBoss),
                new XAttribute(nameof(SettingsContainer.WebhookUrlFieldBoss), App.Settings.WebhookUrlFieldBoss),
                new XAttribute(nameof(SettingsContainer.WebhookMessageFieldBossSpawn), App.Settings.WebhookMessageFieldBossSpawn),
                new XAttribute(nameof(SettingsContainer.WebhookMessageFieldBossDie), App.Settings.WebhookMessageFieldBossDie),
                new XAttribute(nameof(SettingsContainer.TwitchName), App.Settings.TwitchName),
                new XAttribute(nameof(SettingsContainer.TwitchToken), App.Settings.TwitchToken),
                new XAttribute(nameof(SettingsContainer.TwitchChannelName), App.Settings.TwitchChannelName),
                //new XAttribute(nameof(SettingsContainer.LfgEnabled), App.Settings.LfgWindowSettings.Enabled),
                new XAttribute(nameof(SettingsContainer.UseHotkeys), App.Settings.UseHotkeys),
                new XAttribute(nameof(SettingsContainer.HideHandles), App.Settings.HideHandles),
                new XAttribute(nameof(SettingsContainer.ShowTradeLfg), App.Settings.ShowTradeLfg),
                new XAttribute(nameof(SettingsContainer.LanguageOverride), App.Settings.LanguageOverride),
                //new XAttribute("ShowFlightEnergy", App.Settings.FlightGaugeWindowSettings.ShowFlightEnergy),
                new XAttribute("FlightGaugeRotation", App.Settings.FlightGaugeWindowSettings.Rotation),
                new XAttribute("FlipFlightGauge", App.Settings.FlightGaugeWindowSettings.Flip),
                new XAttribute(nameof(SettingsContainer.AbnormalityShape), App.Settings.AbnormalityShape),
                new XAttribute(nameof(SettingsContainer.Npcap), App.Settings.Npcap),
                new XAttribute(nameof(SettingsContainer.EthicalMode), App.Settings.EthicalMode),
                new XAttribute(nameof(SettingsContainer.CheckOpcodesHash), App.Settings.CheckOpcodesHash),
                new XAttribute(nameof(FloatingButtonWindowSettings.ShowNotificationBubble), App.Settings.FloatingButtonSettings.ShowNotificationBubble),
                new XAttribute(nameof(SettingsContainer.FpsAtGuardian), App.Settings.FpsAtGuardian),
                new XAttribute(nameof(SettingsContainer.BetaNotification), App.Settings.BetaNotification),
                new XAttribute(nameof(SettingsContainer.EnableProxy), App.Settings.EnableProxy),
                new XAttribute(nameof(SettingsContainer.DisableLfgChatMessages), App.Settings.DisableLfgChatMessages),
                new XAttribute(nameof(SettingsContainer.CheckGuildBamWithoutOpcode), App.Settings.CheckGuildBamWithoutOpcode),
                new XAttribute(nameof(SettingsContainer.DontShowFUBH), App.Settings.DontShowFUBH),
                new XAttribute(nameof(SettingsContainer.CaptureMode), App.Settings.CaptureMode),
                new XAttribute(nameof(SettingsContainer.LastScreenSize), $"{App.Settings.LastScreenSize.Width},{App.Settings.LastScreenSize.Height}")
            );
        }
        private static XElement BuildGroupAbnormalsXElement()
        {
            var result = new XElement("GroupAbnormals");
            foreach (var pair in App.Settings.GroupWindowSettings.GroupAbnormals)
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
            var result = new XElement(nameof(SettingsContainer.BuffWindowSettings.MyAbnormals));
            foreach (var pair in App.Settings.BuffWindowSettings.MyAbnormals)
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