using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using TCC.Data;
using TCC.ViewModels;

namespace TCC
{
    public static class SettingsManager
    {
        static Rectangle _screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        public static XDocument SettingsDoc;

        public static WindowSettings GroupWindowSettings;
        public static WindowSettings CooldownWindowSettings;
        public static WindowSettings BossWindowSettings;
        public static WindowSettings BuffWindowSettings;
        public static WindowSettings CharacterWindowSettings;
        public static WindowSettings ClassWindowSettings;
        public static WindowSettings ChatWindowSettings;

        public static bool IgnoreMeInGroupWindow { get; set; }
        public static bool IgnoreGroupBuffs { get; set; }
        public static bool IgnoreGroupDebuffs { get; set; }
        public static bool IgnoreRaidAbnormalitiesInGroupWindow { get; set; }
        public static FlowDirection BuffsDirection { get; set; } = FlowDirection.RightToLeft;
        //public static bool ClassWindowOn { get; set; } = false;
        public static CooldownBarMode CooldownBarMode { get; set; } = CooldownBarMode.Fixed;
        public static bool ClickThruWhenDim { get; set; } = true;
        public static bool ClickThruInCombat { get; set; } = false;
        public static int MaxMessages { get; set; } = 500;
        public static int SpamThreshold { get; set; } = 2;
        public static bool ShowChannel { get; set; } = true;
        public static bool ShowTimestamp { get; set; } = true;
        public static bool ShowOnlyBosses { get; set; } = false;
        public static bool DisablePartyMP { get; set; } = false;
        public static bool DisablePartyHP { get; set; } = false;
        public static bool ShowOnlyAggroStacks { get; set; } = true;
        public static bool DisablePartyAbnormals { get; set; } = false;
        public static bool LfgOn { get; set; } = true;
        public static double ChatWindowOpacity { get; set; } = 0.4;
        public static DateTime LastRun { get; set; } = DateTime.MinValue;
        public static string LastRegion { get; set; } = "";
        public static string Webhook { get; set; } = "";
        public static List<ChatChannelOnOff> EnabledChatChannels { get; set; } = Utils.GetEnabledChannelsList();
        public static string WebhookMessage { get; set; } = "@here Guild BAM will spawn soon!";
        public static int FontSize { get; set; } = 15;
        public static string TwitchName { get; set; } = "";
        public static string TwitchToken { get; set; } = "";
        public static string TwitchChannelName { get; set; } = "";
        public static bool ChatFadeOut { get; set; } = true;

        public static Dictionary<Class, List<uint>> GroupAbnormals = new Dictionary<Class, List<uint>>();
        public static uint GroupSizeThreshold = 7;
        public static EnrageLabelMode EnrageLabelMode { get; set; } = EnrageLabelMode.Remaining;
        public static void LoadWindowSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");

                foreach (var ws in SettingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    if (ws.Attribute("Name").Value == "BossWindow")
                    {
                        BossWindowSettings = ParseWindowSettings(ws);
                    }
                    else if (ws.Attribute("Name").Value == "CharacterWindow")
                    {
                        CharacterWindowSettings = ParseWindowSettings(ws);
                    }
                    else if (ws.Attribute("Name").Value == "CooldownWindow")
                    {
                        CooldownWindowSettings = ParseWindowSettings(ws);
                    }
                    else if (ws.Attribute("Name").Value == "BuffWindow")
                    {
                        BuffWindowSettings = ParseWindowSettings(ws);
                    }
                    else if (ws.Attribute("Name").Value == "GroupWindow")
                    {
                        GroupWindowSettings = ParseWindowSettings(ws);
                    }
                    else if (ws.Attribute("Name").Value == "ClassWindow")
                    {
                        ClassWindowSettings = ParseWindowSettings(ws);
                    }
                    else if (ws.Attribute("Name").Value == "ChatWindow")
                    {
                        ChatWindowSettings = ParseWindowSettings(ws);
                    }
                    //add window here
                }
            }
            else
            {
                GroupWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Never, 1, true, .2, false, true, true);
                CooldownWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.WhenDim, 1, true, .2, false, true, true);
                BossWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Never, 1, true, .2, false, true, true);
                BuffWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.WhenDim, 1, true, .2, false, true, true);
                CharacterWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Always, 1, true, .2, false, true, true);
                ClassWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Always, 1, true, .2, false, true, true);
                ChatWindowSettings = new WindowSettings(0, 0, 200, 600, true, ClickThruMode.Never, 1, false, 1, false, true, true);
            }
        }
        public static void LoadSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");

                var b = SettingsDoc.Descendants("OtherSettings").FirstOrDefault();
                if (b == null) return;
                try
                {
                    IgnoreMeInGroupWindow = Boolean.Parse(b.Attribute("IgnoreMeInGroupWindow").Value);
                }
                catch { }
                try
                {
                    IgnoreGroupBuffs = Boolean.Parse(b.Attribute(nameof(IgnoreGroupBuffs)).Value);
                }
                catch { }
                try
                {
                    IgnoreGroupDebuffs = Boolean.Parse(b.Attribute(nameof(IgnoreGroupDebuffs)).Value);
                }
                catch { }
                try
                {
                    IgnoreRaidAbnormalitiesInGroupWindow = Boolean.Parse(b.Attribute("IgnoreRaidAbnormalitiesInGroupWindow").Value);
                }
                catch { }
                try
                {
                    BuffsDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), b.Attribute("BuffsDirection").Value);
                }
                catch { }
                try
                {
                    //ClassWindowOn = Boolean.Parse(b.Attribute("ClassWindowOn").Value);
                    CooldownBarMode = (TCC.CooldownBarMode) Enum.Parse(typeof(CooldownBarMode), b.Attribute(nameof(TCC.CooldownBarMode)).Value);
                }
                catch { }
                try
                {
                    ClickThruWhenDim = Boolean.Parse(b.Attribute("ClickThruWhenDim").Value);
                }
                catch { }
                try
                {
                    ClickThruInCombat = Boolean.Parse(b.Attribute(nameof(ClickThruInCombat)).Value);
                }
                catch { }
                try
                {
                    MaxMessages = Int32.Parse(b.Attribute(nameof(MaxMessages)).Value);
                }
                catch { }
                try
                {
                    SpamThreshold = Int32.Parse(b.Attribute(nameof(SpamThreshold)).Value);
                }
                catch { }
                try
                {
                    FontSize = Int32.Parse(b.Attribute(nameof(FontSize)).Value);
                }
                catch { }
                try
                {
                    ShowChannel = Boolean.Parse(b.Attribute(nameof(ShowChannel)).Value);
                }
                catch { }
                try
                {
                    ShowTimestamp = Boolean.Parse(b.Attribute(nameof(ShowTimestamp)).Value);
                }
                catch { }
                try
                {
                    ShowOnlyBosses = Boolean.Parse(b.Attribute(nameof(ShowOnlyBosses)).Value);
                }
                catch { }
                try
                {
                    DisablePartyMP = Boolean.Parse(b.Attribute(nameof(DisablePartyMP)).Value);
                }
                catch { }
                try
                {
                    DisablePartyHP = Boolean.Parse(b.Attribute(nameof(DisablePartyHP)).Value);
                }
                catch { }
                try
                {
                    ShowOnlyAggroStacks = Boolean.Parse(b.Attribute(nameof(ShowOnlyAggroStacks)).Value);
                }
                catch { }
                try
                {
                    DisablePartyAbnormals = Boolean.Parse(b.Attribute(nameof(DisablePartyAbnormals)).Value);
                }
                catch (Exception) { }
                try
                {
                    LfgOn = Boolean.Parse(b.Attribute(nameof(LfgOn)).Value);
                }
                catch (Exception) { }
                try
                {
                    ChatFadeOut = Boolean.Parse(b.Attribute(nameof(ChatFadeOut)).Value);
                }
                catch (Exception) { }
                try
                {
                    ChatWindowOpacity = Double.Parse(b.Attribute(nameof(ChatWindowOpacity)).Value, CultureInfo.InvariantCulture);
                }
                catch (Exception) { }
                try
                {
                    LastRun = DateTime.Parse(b.Attribute(nameof(LastRun)).Value);
                }
                catch (Exception) { }
                try
                {
                    LastRegion = b.Attribute(nameof(LastRegion)).Value;
                }
                catch (Exception) { }
                try
                {
                    Webhook = b.Attribute(nameof(Webhook)).Value;
                }
                catch (Exception) { }
                try
                {
                    WebhookMessage = b.Attribute(nameof(WebhookMessage)).Value;
                }
                catch (Exception) { }
                try
                {
                    TwitchName = b.Attribute(nameof(TwitchName)).Value;
                }
                catch (Exception) { }
                try
                {
                    TwitchToken = b.Attribute(nameof(TwitchToken)).Value;
                }
                catch (Exception) { }
                try
                {
                    TwitchChannelName = b.Attribute(nameof(TwitchChannelName)).Value;
                }
                catch (Exception) { }
                try
                {
                    GroupSizeThreshold = UInt32.Parse(b.Attribute(nameof(GroupSizeThreshold)).Value);
                }
                catch { }
                try
                {
                    EnrageLabelMode = (EnrageLabelMode)Enum.Parse(typeof(EnrageLabelMode), b.Attribute(nameof(EnrageLabelMode)).Value);
                }
                catch { }
                //add settings here

                try
                {
                    ParseChannelsSettings(SettingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(EnabledChatChannels)));
                }
                catch (Exception) { }

                try
                {
                    ParseGroupAbnormalSettings(SettingsDoc.Descendants()
                        .FirstOrDefault(x => x.Name == nameof(GroupAbnormals)));
                }
                catch
                {
                    GroupAbnormals = new Dictionary<Class, List<uint>>
                    {
                        { Class.Common, new List<uint>{ 4000,4001,4010,4011,4020,4021,4030,4031,4600,4610,4611,4613,5000003, 4830, 4831, 4833, 4841, 4886, 4861, 4953, 4955, 7777015,902,910,911,912,913,916,920,921,922, 999010000 } },
                        { Class.Priest, new List<uint>{ 201,202,805100,805101,805102,98000109,805600,805601,805602,805603,805604,98000110,800300,800301,800302,800303,800304,801500,801501,801502,801503,98000107} },
                        { Class.Elementalist, new List<uint>{ 27120,700630,700631, 601,602,603, 700330, 700230,700231,800132,700233,700730,700731, 700100 } }
                    };
                }
            }
        }


        public static List<Tab> ParseTabsSettings()
        {
            var result = new List<Tab>();
            if (SettingsDoc != null)
            {
                var el = SettingsDoc.Descendants().Where(x => x.Name == "ChatTabsSettings");
                foreach (var t in el.Descendants().Where(x => x.Name == "Tab"))
                {
                    var tabName = t.Attribute("name").Value;
                    var channels = new List<ChatChannel>();
                    var exChannels = new List<ChatChannel>();
                    var authors = new List<string>();
                    var exAuthors = new List<string>();
                    foreach (var chElement in t.Descendants().Where(x => x.Name == "Channel"))
                    {
                        channels.Add((ChatChannel)Enum.Parse(typeof(ChatChannel), chElement.Attribute("value").Value));
                    }
                    foreach (var chElement in t.Descendants().Where(x => x.Name == "ExcludedChannel"))
                    {
                        exChannels.Add((ChatChannel)Enum.Parse(typeof(ChatChannel), chElement.Attribute("value").Value));
                    }
                    foreach (var authElement in t.Descendants().Where(x => x.Name == "Author"))
                    {
                        authors.Add(authElement.Attribute("value").Value);
                    }
                    foreach (var authElement in t.Descendants().Where(x => x.Name == "ExcludedAuthor"))
                    {
                        exAuthors.Add(authElement.Attribute("value").Value);
                    }

                    result.Add(new Tab(tabName, channels.ToArray(), exChannels.ToArray(), authors.ToArray(), exAuthors.ToArray()));
                }
            }
            return result;
        }

        public static void SaveSettings()
        {
            var xSettings = new XElement("Settings",
                new XElement("WindowSettings",
                    BossWindowSettings.ToXElement("BossWindow"),
                    BuffWindowSettings.ToXElement("BuffWindow"),
                    CharacterWindowSettings.ToXElement("CharacterWindow"),
                    CooldownWindowSettings.ToXElement("CooldownWindow"),
                    GroupWindowSettings.ToXElement("GroupWindow"),
                    ClassWindowSettings.ToXElement("ClassWindow"),
                    ChatWindowSettings.ToXElement("ChatWindow")
                    //add window here
                    ),
                new XElement("OtherSettings",
                new XAttribute(nameof(IgnoreMeInGroupWindow), IgnoreMeInGroupWindow),
                new XAttribute(nameof(IgnoreGroupBuffs), IgnoreGroupBuffs),
                new XAttribute(nameof(IgnoreGroupDebuffs), IgnoreGroupDebuffs),
                new XAttribute(nameof(IgnoreRaidAbnormalitiesInGroupWindow), IgnoreRaidAbnormalitiesInGroupWindow),
                new XAttribute(nameof(BuffsDirection), BuffsDirection),
                new XAttribute(nameof(CooldownBarMode), CooldownBarMode),
                new XAttribute(nameof(ClickThruWhenDim), ClickThruWhenDim),
                new XAttribute(nameof(ClickThruInCombat), ClickThruInCombat),
                new XAttribute(nameof(MaxMessages), MaxMessages),
                new XAttribute(nameof(SpamThreshold), SpamThreshold),
                new XAttribute(nameof(FontSize), FontSize),
                new XAttribute(nameof(ShowChannel), ShowChannel),
                new XAttribute(nameof(ShowTimestamp), ShowTimestamp),
                new XAttribute(nameof(ShowOnlyBosses), ShowOnlyBosses),
                new XAttribute(nameof(DisablePartyMP), DisablePartyMP),
                new XAttribute(nameof(DisablePartyHP), DisablePartyHP),
                new XAttribute(nameof(DisablePartyAbnormals), DisablePartyAbnormals),
                new XAttribute(nameof(ShowOnlyAggroStacks), ShowOnlyAggroStacks),
                new XAttribute(nameof(LfgOn), LfgOn),
                new XAttribute(nameof(ChatFadeOut), ChatFadeOut),
                new XAttribute(nameof(ChatWindowOpacity), ChatWindowOpacity),
                new XAttribute(nameof(LastRun), DateTime.Now),
                new XAttribute(nameof(LastRegion), LastRegion),
                new XAttribute(nameof(Webhook), Webhook),
                new XAttribute(nameof(WebhookMessage), WebhookMessage),
                new XAttribute(nameof(TwitchName), TwitchName),
                new XAttribute(nameof(TwitchToken), TwitchToken),
                new XAttribute(nameof(TwitchChannelName), TwitchChannelName),
                new XAttribute(nameof(GroupSizeThreshold), GroupSizeThreshold),
                new XAttribute(nameof(EnrageLabelMode), EnrageLabelMode)
                //add setting here
                ),
                BuildChannelsXElement(),
                BuildChatTabsXElement(),
                BuildGroupAbnormalsXElement()
            );
            xSettings.Save(Environment.CurrentDirectory + @"/tcc-config.xml");
        }

        private static WindowSettings ParseWindowSettings(XElement ws)
        {
            double x = 0, y = 0, w = 0, h = 0, scale = 1, dimOp = .3;
            ClickThruMode ctm = ClickThruMode.Never;
            bool vis = true, enabled = true, autoDim = true, allowTrans = true, alwaysVis = false;

            try
            {
                x = Double.Parse(ws.Attribute("X").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                y = Double.Parse(ws.Attribute("Y").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w = Double.Parse(ws.Attribute("W").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                h = Double.Parse(ws.Attribute("H").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                ctm = (ClickThruMode)Enum.Parse(typeof(ClickThruMode), ws.Attribute(nameof(ClickThruMode)).Value);
            }
            catch (Exception) { }
            try
            {
                //w.Visibility = (Visibility)Enum.Parse(typeof(Visibility), ws.Attribute("Visibility").Value);
                vis = Boolean.Parse(ws.Attribute("Visible").Value);
            }
            catch (Exception) { }

            try
            {
                scale = Double.Parse(ws.Attribute("Scale").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                autoDim = Boolean.Parse(ws.Attribute("AutoDim").Value);
            }
            catch (Exception) { }
            try
            {
                dimOp = Double.Parse(ws.Attribute("DimOpacity").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                alwaysVis = Boolean.Parse(ws.Attribute("ShowAlways").Value);
            }
            catch (Exception) { }
            try
            {
                allowTrans = Boolean.Parse(ws.Attribute("AllowTransparency").Value);
            }
            catch (Exception) { }
            try
            {
                enabled = Boolean.Parse(ws.Attribute("Enabled").Value);
            }
            catch (Exception) { }
            return new WindowSettings(x, y, h, w, vis, ctm, scale, autoDim, dimOp, alwaysVis, allowTrans, enabled);
        }
        private static void ParseChannelsSettings(XElement xElement)
        {
            foreach (var e in xElement.Descendants().Where(x => x.Name == "Channel"))
            {
                EnabledChatChannels.FirstOrDefault(x => x.Channel == (ChatChannel)Enum.Parse(typeof(ChatChannel), e.Attribute("name").Value)).Enabled = Boolean.Parse(e.Attribute("enabled").Value);
            }
        }
        private static void ParseGroupAbnormalSettings(XElement el)
        {
            GroupAbnormals = new Dictionary<Class, List<uint>>();
            foreach (var abEl in el.Descendants().Where(x => x.Name == "Abnormals"))
            {
                var c = abEl.Attribute("class").Value;
                var cl = (Class)Enum.Parse(typeof(Class), c);
                var abs = abEl.Value.Split(',');
                var l = abs.Select(uint.Parse).ToList();
                GroupAbnormals.Add(cl, l);
            }
            if (GroupAbnormals.Count == 0)
            {
                GroupAbnormals = new Dictionary<Class, List<uint>>
                {
                    { Class.Common, new List<uint>{ 4000,4001,4010,4011,4020,4021,4030,4031,4600,4610,4611,4613,5000003, 4830, 4831, 4833, 4841, 4886, 4861, 4953, 4955, 7777015,902,910,911,912,913,916,920,921,922, 999010000 } },
                    { Class.Priest, new List<uint>{ 201,202,805100,805101,805102,98000109,805600,805601,805602,805603,805604,98000110,800300,800301,800302,800303,800304,801500,801501,801502,801503,98000107} },
                    { Class.Elementalist, new List<uint>{ 27120,700630,700631, 601,602,603, 700330, 700230,700231,800132,700233,700730,700731, 700100 } }
                };
            }
        }

        private static XElement BuildChatTabsXElement()
        {
            XElement result = new XElement("ChatTabsSettings");
            foreach (var tab in ChatWindowViewModel.Instance.Tabs)
            {
                XAttribute tabName = new XAttribute("name", tab.TabName);
                XElement tabElement = new XElement("Tab", tabName);
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
        private static XElement BuildChannelsXElement()
        {
            XElement result = new XElement(nameof(EnabledChatChannels));
            foreach (var c in EnabledChatChannels)
            {
                XAttribute name = new XAttribute("name", c.Channel.ToString());
                XAttribute val = new XAttribute("enabled", c.Enabled.ToString());
                XElement chElement = new XElement("Channel", name, val);
                result.Add(chElement);
            }
            return result;
        }
        private static XElement BuildGroupAbnormalsXElement()
        {
            var result = new XElement(nameof(GroupAbnormals));
            foreach (KeyValuePair<Class, List<uint>> pair in GroupAbnormals)
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

    }
}
