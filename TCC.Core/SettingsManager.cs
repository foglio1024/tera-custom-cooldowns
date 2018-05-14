using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using TCC.Data;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;
using ModifierKeys = TCC.Tera.Data.HotkeysData.ModifierKeys;
using Key = System.Windows.Forms.Keys;
using Size = System.Drawing.Size;

namespace TCC
{
    public struct HotKey
    {
        public HotKey(Key k, ModifierKeys m) : this()
        {
            Key = k;
            Modifier = m;
        }

        public Key Key { get; set; }
        public ModifierKeys Modifier { get; set; }
    }
    public static class SettingsManager
    {
        public static double ScreenW => SystemParameters.VirtualScreenWidth;
        public static double ScreenH => SystemParameters.VirtualScreenHeight;
        public static XDocument SettingsDoc;

        public static WindowSettings GroupWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true);
        public static WindowSettings CooldownWindowSettings = new WindowSettings(.4, .7, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true);
        public static WindowSettings BossWindowSettings = new WindowSettings(.4, 0, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true);
        public static WindowSettings BuffWindowSettings = new WindowSettings(1, .7, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true);
        public static WindowSettings CharacterWindowSettings = new WindowSettings(.4, 1, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true);
        public static WindowSettings ClassWindowSettings = new WindowSettings(.25, .6, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true);
        public static WindowSettings FlightGaugeWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Always, 1, true, 1, false, true);

        public static SynchronizedObservableCollection<ChatWindowSettings> ChatWindowsSettings = new SynchronizedObservableCollection<ChatWindowSettings>();

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

        public static string LastRegion
        {
            get
            {
                if (RegionOverride != "") return RegionOverride;
                return _lastRegion;
            }
            set => _lastRegion = value;
        }

        public static string Webhook { get; set; } = "";
        public static List<ChatChannelOnOff> EnabledChatChannels { get; set; } = Utils.GetEnabledChannelsList();
        public static string WebhookMessage { get; set; } = "@here Guild BAM will spawn soon!";
        public static int FontSize { get; set; } = 15;
        public static string TwitchName { get; set; } = "";
        public static string TwitchToken { get; set; } = "";
        public static string TwitchChannelName { get; set; } = "";
        public static bool ChatFadeOut { get; set; } = true;

        public static Dictionary<Class, List<uint>> GroupAbnormals = new Dictionary<Class, List<uint>>()
        {
            {(Class)0, new List<uint>()},
            {(Class)1, new List<uint>()},
            {(Class)2, new List<uint>()},
            {(Class)3, new List<uint>()},
            {(Class)4, new List<uint>()},
            {(Class)5, new List<uint>()},
            {(Class)6, new List<uint>()},
            {(Class)7, new List<uint>()},
            {(Class)8, new List<uint>()},
            {(Class)9, new List<uint>()},
            {(Class)10, new List<uint>()},
            {(Class)11, new List<uint>()},
            {(Class)12, new List<uint>()},
            {(Class)255, new List<uint>()},
        };
        public static uint GroupSizeThreshold = 7;
        public static EnrageLabelMode EnrageLabelMode { get; set; } = EnrageLabelMode.Remaining;
        public static bool ShowItemsCooldown { get; set; } = true;
        public static bool ShowMembersLaurels { get; set; } = false;
        public static bool AnimateChatMessages { get; set; } = false;
        public static bool ChatEnabled { get; set; } = true;
        public static bool StatSent { get; set; } = false;
        public static bool ShowFlightEnergy { get; set; } = true;
        public static bool LfgEnabled { get; set; } = true;
        public static bool ShowGroupWindowDetails { get; set; } = true;
        public static bool UseHotkeys { get; set; } = true;
        public static HotKey LfgHotkey { get; set; } = new HotKey(Key.Y, ModifierKeys.Control);
        public static HotKey InfoWindowHotkey { get; set; } = new HotKey(Key.I, ModifierKeys.Control);
        public static HotKey SettingsHotkey { get; set; } = new HotKey(Key.O, ModifierKeys.Control);
        public static HotKey ShowAllHotkey { get; set; } = new HotKey(Key.NumPad5, ModifierKeys.Control);
        public static string RegionOverride { get; set; } = "";

        public static void LoadWindowSettings()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml"))
            {
                SettingsDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");

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
                    else if (ws.Attribute("Name").Value == "FlightGaugeWindow")
                    {
                        FlightGaugeWindowSettings = ParseWindowSettings(ws);
                    }
                    //add window here
                }

                if (SettingsDoc.Descendants().Count(x => x.Name == "ChatWindow") > 0)
                {
                    SettingsDoc.Descendants().Where(x => x.Name == "ChatWindow").ToList().ForEach(s =>
                    {
                        ChatWindowsSettings.Add(ParseChatWindowSettings(s));
                    });
                }
            }
        }

        private static ChatWindowSettings ParseChatWindowSettings(XElement s)
        {
            var ws = s.Descendants().FirstOrDefault(x => x.Name == "WindowSetting");
            var ts = s.Descendants().FirstOrDefault(x => x.Name == "Tabs");
            var lfg = ws.Attribute(nameof(ChatWindowSettings.LfgOn));
            var op = ws.Attribute(nameof(ChatWindowSettings.BackgroundOpacity));

            var sett = ParseWindowSettings(ws);
            var tabs = ParseTabsSettings(ts);

            return new ChatWindowSettings(sett.X, sett.Y, sett.H, sett.W,
                                          sett.Visible, sett.ClickThruMode,
                                          sett.Scale, sett.AutoDim, sett.DimOpacity,
                                          sett.ShowAlways, /*sett.AllowTransparency,*/
                                          sett.Enabled)
            {
                Tabs = tabs,
                LfgOn = lfg != null ? bool.Parse(lfg.Value) : true,
                BackgroundOpacity = op != null ? double.Parse(op.Value, CultureInfo.InvariantCulture) : 0.3
            };
        }

        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml"))
                {
                    SettingsDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");

                    var b = SettingsDoc.Descendants("OtherSettings").FirstOrDefault();
                    if (b == null) return;
                    try
                    {
                        IgnoreMeInGroupWindow = bool.Parse(b.Attribute("IgnoreMeInGroupWindow").Value);
                    }
                    catch { }
                    try
                    {
                        IgnoreGroupBuffs = bool.Parse(b.Attribute(nameof(IgnoreGroupBuffs)).Value);
                    }
                    catch { }
                    try
                    {
                        IgnoreGroupDebuffs = bool.Parse(b.Attribute(nameof(IgnoreGroupDebuffs)).Value);
                    }
                    catch { }
                    try
                    {
                        IgnoreRaidAbnormalitiesInGroupWindow = bool.Parse(b.Attribute("IgnoreRaidAbnormalitiesInGroupWindow").Value);
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
                        CooldownBarMode = (CooldownBarMode)Enum.Parse(typeof(CooldownBarMode), b.Attribute(nameof(Data.CooldownBarMode)).Value);
                    }
                    catch { }
                    try
                    {
                        ClickThruWhenDim = bool.Parse(b.Attribute("ClickThruWhenDim").Value);
                    }
                    catch { }
                    try
                    {
                        ClickThruInCombat = bool.Parse(b.Attribute(nameof(ClickThruInCombat)).Value);
                    }
                    catch { }
                    try
                    {
                        MaxMessages = int.Parse(b.Attribute(nameof(MaxMessages)).Value);
                    }
                    catch { }
                    try
                    {
                        SpamThreshold = int.Parse(b.Attribute(nameof(SpamThreshold)).Value);
                    }
                    catch { }
                    try
                    {
                        FontSize = int.Parse(b.Attribute(nameof(FontSize)).Value);
                    }
                    catch { }
                    try
                    {
                        ShowChannel = bool.Parse(b.Attribute(nameof(ShowChannel)).Value);
                    }
                    catch { }
                    try
                    {
                        ShowTimestamp = bool.Parse(b.Attribute(nameof(ShowTimestamp)).Value);
                    }
                    catch { }
                    try
                    {
                        ShowOnlyBosses = bool.Parse(b.Attribute(nameof(ShowOnlyBosses)).Value);
                    }
                    catch { }
                    try
                    {
                        DisablePartyMP = bool.Parse(b.Attribute(nameof(DisablePartyMP)).Value);
                    }
                    catch { }
                    try
                    {
                        DisablePartyHP = bool.Parse(b.Attribute(nameof(DisablePartyHP)).Value);
                    }
                    catch { }
                    try
                    {
                        ShowOnlyAggroStacks = bool.Parse(b.Attribute(nameof(ShowOnlyAggroStacks)).Value);
                    }
                    catch { }
                    try
                    {
                        DisablePartyAbnormals = bool.Parse(b.Attribute(nameof(DisablePartyAbnormals)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        LfgOn = bool.Parse(b.Attribute(nameof(LfgOn)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        ChatFadeOut = bool.Parse(b.Attribute(nameof(ChatFadeOut)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        ChatWindowOpacity = double.Parse(b.Attribute(nameof(ChatWindowOpacity)).Value, CultureInfo.InvariantCulture);
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
                        GroupSizeThreshold = uint.Parse(b.Attribute(nameof(GroupSizeThreshold)).Value);
                    }
                    catch { }
                    try
                    {
                        EnrageLabelMode = (EnrageLabelMode)Enum.Parse(typeof(EnrageLabelMode), b.Attribute(nameof(EnrageLabelMode)).Value);
                    }
                    catch { }
                    try
                    {
                        ShowItemsCooldown = bool.Parse(b.Attribute(nameof(ShowItemsCooldown)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        ShowMembersLaurels = bool.Parse(b.Attribute(nameof(ShowMembersLaurels)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        AnimateChatMessages = bool.Parse(b.Attribute(nameof(AnimateChatMessages)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        StatSent = bool.Parse(b.Attribute(nameof(StatSent)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        StatSent = bool.Parse(b.Attribute(nameof(StatSent)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        ShowFlightEnergy = bool.Parse(b.Attribute(nameof(ShowFlightEnergy)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        LfgEnabled = bool.Parse(b.Attribute(nameof(LfgEnabled)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        ShowGroupWindowDetails = bool.Parse(b.Attribute(nameof(ShowGroupWindowDetails)).Value);
                    }
                    catch (Exception) { }
                    try
                    {
                        RegionOverride = b.Attribute(nameof(RegionOverride)).Value;
                    }
                    catch (Exception) { }
                    try { UseHotkeys = bool.Parse(b.Attribute(nameof(UseHotkeys)).Value); }
                    catch (Exception) { }
                    //add settings here

                    try
                    {
                        ParseChannelsSettings(SettingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(EnabledChatChannels)));
                    }
                    catch (Exception) { }

                    try
                    {
                        ParseGroupAbnormalSettings(SettingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(GroupAbnormals)));
                    }
                    catch
                    {
                        CommonDefault.ForEach(x => GroupAbnormals[Class.Common].Add(x));
                        PriestDefault.ForEach(x => GroupAbnormals[Class.Priest].Add(x));
                        MysticDefault.ForEach(x => GroupAbnormals[Class.Mystic].Add(x));
                    }
                }
            }
            catch (XmlException)
            {
                var res = TccMessageBox.Show("TCC",
                    "Cannot load settings file. Do you want TCC to delete it and recreate a default file?",
                    MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes) File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");
                LoadSettings();
            }
        }

        public static List<Tab> ParseTabsSettings(XElement elem)
        {
            var result = new List<Tab>();
            if (elem != null)
            {
                foreach (var t in elem.Descendants().Where(x => x.Name == "Tab"))
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
                    BuildChatWindowSettings("ChatWindows"),
                    FlightGaugeWindowSettings.ToXElement("FlightGaugeWindow")
                    //ChatWindowSettings.ToXElement("ChatWindow")
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
                new XAttribute(nameof(EnrageLabelMode), EnrageLabelMode),
                new XAttribute(nameof(ShowMembersLaurels), ShowMembersLaurels),
                new XAttribute(nameof(AnimateChatMessages), AnimateChatMessages),
                new XAttribute(nameof(ShowItemsCooldown), ShowItemsCooldown),
                new XAttribute(nameof(StatSent), StatSent),
                new XAttribute(nameof(ShowFlightEnergy), ShowFlightEnergy),
                new XAttribute(nameof(LfgEnabled), LfgEnabled),
                new XAttribute(nameof(ShowGroupWindowDetails), ShowGroupWindowDetails),
                new XAttribute(nameof(UseHotkeys), UseHotkeys),
                new XAttribute(nameof(RegionOverride), RegionOverride)
                //add setting here
                ),
                BuildChannelsXElement(),
                //BuildChatTabsXElement(),
                BuildGroupAbnormalsXElement()
            );
            SaveSettingsDoc(xSettings);
        }


        private static void SaveSettingsDoc(XElement doc)
        {
            try
            {
                doc.Save(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");
            }
            catch (Exception)
            {
                var res = TccMessageBox.Show("TCC", "Could not write settings data to tcc-config.xml. File is being used by another process. Try again?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes) SaveSettingsDoc(doc);
            }

        }
        private static WindowSettings ParseWindowSettings(XElement ws)
        {
            double x = 0, y = 0, w = 0, h = 0, scale = 1, dimOp = .3;
            var ctm = ClickThruMode.Never;
            bool vis = true, enabled = true, autoDim = true, allowTrans = true, alwaysVis = false;

            try
            {
                x = double.Parse(ws.Attribute("X").Value, CultureInfo.InvariantCulture);
                if (x > 1) x = x / ScreenW;
            }
            catch (Exception) { }
            try
            {
                y = double.Parse(ws.Attribute("Y").Value, CultureInfo.InvariantCulture);
                if (y > 1) y = y / ScreenH;
            }
            catch (Exception) { }
            try
            {
                w = double.Parse(ws.Attribute("W").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                h = double.Parse(ws.Attribute("H").Value, CultureInfo.InvariantCulture);
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
                vis = bool.Parse(ws.Attribute("Visible").Value);
            }
            catch (Exception) { }

            try
            {
                scale = double.Parse(ws.Attribute("Scale").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                autoDim = bool.Parse(ws.Attribute("AutoDim").Value);
            }
            catch (Exception) { }
            try
            {
                dimOp = double.Parse(ws.Attribute("DimOpacity").Value, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            try
            {
                alwaysVis = bool.Parse(ws.Attribute("ShowAlways").Value);
            }
            catch (Exception) { }
            //try
            //{
            //    allowTrans = Boolean.Parse(ws.Attribute("AllowTransparency").Value);
            //}
            //catch (Exception) { }
            try
            {
                enabled = bool.Parse(ws.Attribute("Enabled").Value);
            }
            catch (Exception) { }
            return new WindowSettings(x, y, h, w, vis, ctm, scale, autoDim, dimOp, alwaysVis, enabled);
        }
        private static void ParseChannelsSettings(XElement xElement)
        {
            foreach (var e in xElement.Descendants().Where(x => x.Name == "Channel"))
            {
                EnabledChatChannels.FirstOrDefault(x => x.Channel == (ChatChannel)Enum.Parse(typeof(ChatChannel), e.Attribute("name").Value)).Enabled = bool.Parse(e.Attribute("enabled").Value);
            }
        }
        private static void ParseGroupAbnormalSettings(XElement el)
        {
            //GroupAbnormals = new Dictionary<Class, List<uint>>();
            foreach (var groupAbnormalList in GroupAbnormals)
            {
                groupAbnormalList.Value.Clear();
            }

            foreach (var abEl in el.Descendants().Where(x => x.Name == "Abnormals"))
            {
                var c = abEl.Attribute("class").Value;
                var cl = (Class)Enum.Parse(typeof(Class), c);
                var abs = abEl.Value.Split(',');
                var l = abs.Length == 1 && abs[0] == "" ? new List<uint>() : abs.Select(uint.Parse).ToList();
                l.ForEach(ab => GroupAbnormals[cl].Add(ab));
                //GroupAbnormals.Add(cl, l);
            }
            if (GroupAbnormals.Count == 0)
            {
                CommonDefault.ForEach(x => GroupAbnormals[Class.Common].Add(x));
                PriestDefault.ForEach(x => GroupAbnormals[Class.Priest].Add(x));
                MysticDefault.ForEach(x => GroupAbnormals[Class.Mystic].Add(x));
            }
        }
        static List<uint> CommonDefault = new List<uint> { 4000, 4001, 4010, 4011, 4020, 4021, 4030, 4031, 4600, 4610, 4611, 4613, 5000003, 4830, 4831, 4833, 4841, 4886, 4861, 4953, 4955, 7777015, 902, 910, 911, 912, 913, 916, 920, 921, 922, 999010000 };
        static List<uint> PriestDefault = new List<uint> { 201, 202, 805100, 805101, 805102, 98000109, 805600, 805601, 805602, 805603, 805604, 98000110, 800300, 800301, 800302, 800303, 800304, 801500, 801501, 801502, 801503, 98000107 };
        static List<uint> MysticDefault = new List<uint> { 27120, 700630, 700631, 601, 602, 603, 700330, 700230, 700231, 800132, 700233, 700730, 700731, 700100 };
        private static string _lastRegion = "";

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
        private static XElement BuildChannelsXElement()
        {
            var result = new XElement(nameof(EnabledChatChannels));
            foreach (var c in EnabledChatChannels)
            {
                var name = new XAttribute("name", c.Channel.ToString());
                var val = new XAttribute("enabled", c.Enabled.ToString());
                var chElement = new XElement("Channel", name, val);
                result.Add(chElement);
            }
            return result;
        }
        private static XElement BuildGroupAbnormalsXElement()
        {
            var result = new XElement(nameof(GroupAbnormals));
            foreach (var pair in GroupAbnormals)
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
                result.Add(new XElement("ChatWindow", cw.WindowSettings.ToXElement("Settings")));
            });
            return result;
        }

    }
}
