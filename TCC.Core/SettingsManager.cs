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
    public class WindowSettings
    {
        public double X;
        public double Y;
        public double W;
        public double H;
        public Visibility Visibility;
        public bool ClickThru;
        public double Scale;
        public bool AutoDim;
        public double DimOpacity;
        public bool ShowAlways;
        public bool AllowTransparency;
        public bool Enabled;

        public XElement ToXElement(string name)
        {
            var xe = new XElement("WindowSetting");
            xe.Add(new XAttribute("Name", name));
            xe.Add(new XAttribute(nameof(X), X));
            xe.Add(new XAttribute(nameof(Y), Y));
            xe.Add(new XAttribute(nameof(W), W));
            xe.Add(new XAttribute(nameof(H), H));
            xe.Add(new XAttribute(nameof(Visibility), Visibility));
            xe.Add(new XAttribute(nameof(ClickThru), ClickThru));
            xe.Add(new XAttribute(nameof(Scale), Scale));
            xe.Add(new XAttribute(nameof(AutoDim), AutoDim));
            xe.Add(new XAttribute(nameof(DimOpacity), DimOpacity));
            xe.Add(new XAttribute(nameof(ShowAlways), ShowAlways));
            xe.Add(new XAttribute(nameof(AllowTransparency), AllowTransparency));
            xe.Add(new XAttribute(nameof(Enabled), Enabled));
            return xe;
        }

    }

    public static class SettingsManager
    {
        static Rectangle _screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        public static XDocument SettingsDoc;

        public static WindowSettings GroupWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true
        };
        public static WindowSettings CooldownWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings BossWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings BuffWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings CharacterWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings ClassWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = true,
            DimOpacity = .2,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };
        public static WindowSettings ChatWindowSettings = new WindowSettings()
        {
            X = 0,
            Y = 0,
            W = 600,
            H = 200,
            Visibility = Visibility.Visible,
            ClickThru = false,
            Scale = 1,
            AutoDim = false,
            DimOpacity = 1,
            ShowAlways = false,
            AllowTransparency = true,
            Enabled = true

        };

        public static bool IgnoreMeInGroupWindow { get; set; }
        public static bool IgnoreGroupBuffs { get; set; }
        public static bool IgnoreGroupDebuffs { get; set; }
        public static bool IgnoreRaidAbnormalitiesInGroupWindow { get; set; }
        public static FlowDirection BuffsDirection { get; set; } = FlowDirection.RightToLeft;
        public static bool ClassWindowOn { get; set; } = false;
        public static bool ClickThruWhenDim { get; set; } = true;
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
        public static Dictionary<Class, List<uint>> GroupAbnormals = new Dictionary<Class, List<uint>>();
        public static uint GroupSizeThreshold = 7;
        public static void LoadWindowSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(Environment.CurrentDirectory + @"/tcc-config.xml");

                foreach (var ws in SettingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    if (ws.Attribute("Name").Value == "BossWindow")
                    {
                        ParseWindowSettings(BossWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "CharacterWindow")
                    {
                        ParseWindowSettings(CharacterWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "CooldownWindow")
                    {
                        ParseWindowSettings(CooldownWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "BuffWindow")
                    {
                        ParseWindowSettings(BuffWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "GroupWindow")
                    {
                        ParseWindowSettings(GroupWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "ClassWindow")
                    {
                        ParseWindowSettings(ClassWindowSettings, ws);
                    }
                    else if (ws.Attribute("Name").Value == "ChatWindow")
                    {
                        ParseWindowSettings(ChatWindowSettings, ws);
                    }
                    //add window here
                }
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
                    ClassWindowOn = Boolean.Parse(b.Attribute("ClassWindowOn").Value);
                }
                catch { }
                try
                {
                    ClickThruWhenDim = Boolean.Parse(b.Attribute("ClickThruWhenDim").Value);
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
                    GroupSizeThreshold = UInt32.Parse(b.Attribute(nameof(GroupSizeThreshold)).Value);
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
                new XAttribute(nameof(ClassWindowOn), ClassWindowOn),
                new XAttribute(nameof(ClickThruWhenDim), ClickThruWhenDim),
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
                new XAttribute(nameof(ChatWindowOpacity), ChatWindowOpacity),
                new XAttribute(nameof(LastRun), DateTime.Now),
                new XAttribute(nameof(LastRegion), LastRegion),
                new XAttribute(nameof(Webhook), Webhook),
                new XAttribute(nameof(WebhookMessage), WebhookMessage),
                new XAttribute(nameof(GroupSizeThreshold), GroupSizeThreshold)
                //add setting here
                ),
                BuildChannelsXElement(),
                BuildChatTabsXElement(),
                BuildGroupAbnormalsXElement()
            );
            xSettings.Save(Environment.CurrentDirectory + @"/tcc-config.xml");
        }

        private static void ParseWindowSettings(WindowSettings w, XElement ws)
        {
            try
            {
                w.X = Double.Parse(ws.Attribute("X").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.Y = Double.Parse(ws.Attribute("Y").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.W = Double.Parse(ws.Attribute("W").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.H = Double.Parse(ws.Attribute("H").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }

            try
            {
                w.ClickThru = Boolean.Parse(ws.Attribute("ClickThru").Value);
            }
            catch (Exception) { }

            try
            {
                w.Visibility = (Visibility)Enum.Parse(typeof(Visibility), ws.Attribute("Visibility").Value);
            }
            catch (Exception) { }

            try
            {
                w.Scale = Double.Parse(ws.Attribute("Scale").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.AutoDim = Boolean.Parse(ws.Attribute("AutoDim").Value);
            }
            catch (Exception) { }
            try
            {
                w.DimOpacity = Double.Parse(ws.Attribute("DimOpacity").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                w.ShowAlways = Boolean.Parse(ws.Attribute("ShowAlways").Value);
            }
            catch (Exception) { }
            try
            {
                w.AllowTransparency = Boolean.Parse(ws.Attribute("AllowTransparency").Value);
            }
            catch (Exception) { }
            try
            {
                w.Enabled = Boolean.Parse(ws.Attribute("Enabled").Value);
            }
            catch (Exception) { }
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
