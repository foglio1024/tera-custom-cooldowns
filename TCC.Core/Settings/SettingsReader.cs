using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using TCC.Data;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Settings
{
    public class SettingsReader
    {
        private XDocument _settingsDoc;

        private static readonly List<uint> CommonDefault = new List<uint> { 4000, 4001, 4010, 4011, 4020, 4021, 4030, 4031, 4600, 4610, 4611, 4613, 5000003, 4830, 4831, 4833, 4841, 4886, 4861, 4953, 4955, 7777015, 902, 910, 911, 912, 913, 916, 920, 921, 922, 999010000 };
        private static readonly List<uint> PriestDefault = new List<uint> { 201, 202, 805100, 805101, 805102, 98000109, 805600, 805601, 805602, 805603, 805604, 98000110, 800300, 800301, 800302, 800303, 800304, 801500, 801501, 801502, 801503, 98000107 };
        private static readonly List<uint> MysticDefault = new List<uint> { 27120, 700630, 700631, 601, 602, 603, 700330, 700230, 700231, 800132, 700233, 700730, 700731, 700100 };

        //Add My Abnormals Setting by HQ ====================================================
        private static readonly List<uint> MyCommonDefault = new List<uint> { 6001, 6002, 6003, 6004, 6012, 6013, 702004, 805800, 805803, 200700, 200701, 200731, 800300, 800301, 800302, 800303, 800304, 702001 };
        private static readonly List<uint> MyArcherDefault = new List<uint> { 601400, 601450, 601460, 88608101, 88608102, 88608103, 88608104, 88608105, 88608106, 88608107, 88608108, 88608109, 88608110 };
        private static readonly List<uint> MyBerserkerDefault = new List<uint> { 401705, 401707, 401709, 401710, 400500, 400501, 400508, 400710, 400711 };
        private static readonly List<uint> MyBrawlerDefault = new List<uint> { 31020, 10153210 };
        private static readonly List<uint> MyGunnerDefault = new List<uint> { 89105101, 89105102, 89105103, 89105104, 89105105, 89105106, 89105107, 89105108, 89105109, 89105110, 89105111, 89105112, 89105113, 89105114, 89105115, 89105116, 89105117, 89105118, 89105119, 89105120, 10152340, 10152351 };
        private static readonly List<uint> MyLancerDefault = new List<uint> { 200230, 200231, 200232, 201701 };
        private static readonly List<uint> MyMysticDefault = new List<uint> { };
        private static readonly List<uint> MyNinjaDefault = new List<uint> { 89314201, 89314202, 89314203, 89314204, 89314205, 89314206, 89314207, 89314208, 89314209, 89314210, 89314211, 89314212, 89314213, 89314214, 89314215, 89314216, 89314217, 89314218, 89314219, 89314220, 10154480, 10154450 };
        private static readonly List<uint> MyPriestDefault = new List<uint> { };
        private static readonly List<uint> MyReaperDefault = new List<uint> { 10151010, 10151131, 10151192 };
        private static readonly List<uint> MySlayerDefault = new List<uint> { 300800, 300801, 300805 };
        private static readonly List<uint> MySorcererDefault = new List<uint> { 21170, 22120, 23180, 26250, 29011, 25170, 25171, 25201, 25202, 500100, 500150, 501600, 501650 };
        private static readonly List<uint> MyValkyrieDefault = new List<uint> { 10155130, 10155551, 10155510, 10155512, 10155540, 10155541, 10155542 };
        private static readonly List<uint> MyWarriorDefault = new List<uint> { 100800, 100801 };
        //===================================================================================
        
        private List<Tab> ParseTabsSettings(XElement elem)
        {
            var result = new List<Tab>();
            if (elem == null) return result;
            foreach (var t in elem.Descendants().Where(x => x.Name == "Tab"))
            {
                var channels = new List<ChatChannel>();
                var exChannels = new List<ChatChannel>();
                var authors = new List<string>();
                var exAuthors = new List<string>();
                var tabName = t.Attribute("name")?.Value;
                foreach (var chElement in t.Descendants().Where(x => x.Name == "Channel"))
                {
                    try
                    {
                        var value = chElement.Attribute("value");
                        if (value != null) channels.Add((ChatChannel)Enum.Parse(typeof(ChatChannel), value.Value));
                    }
                    catch (Exception) { }
                }
                foreach (var chElement in t.Descendants().Where(x => x.Name == "ExcludedChannel"))
                {
                    try
                    {
                        var value = chElement.Attribute("value");
                        if (value != null) exChannels.Add((ChatChannel)Enum.Parse(typeof(ChatChannel), value.Value));
                    }
                    catch (Exception) { }
                }
                foreach (var authElement in t.Descendants().Where(x => x.Name == "Author"))
                {
                    var value = authElement.Attribute("value");
                    if (value != null) authors.Add(value.Value);
                }
                foreach (var authElement in t.Descendants().Where(x => x.Name == "ExcludedAuthor"))
                {
                    var value = authElement.Attribute("value");
                    if (value != null) exAuthors.Add(value.Value);
                }

                result.Add(new Tab(tabName, channels.ToArray(), exChannels.ToArray(), authors.ToArray(), exAuthors.ToArray()));
            }
            return result;
        }
        private ChatWindowSettings ParseChatWindowSettings(XContainer s)
        {
            var ws = s.Descendants().FirstOrDefault(x => x.Name == "WindowSetting");
            var ts = s.Descendants().FirstOrDefault(x => x.Name == "Tabs");
            var lfg = ws?.Attribute(nameof(ChatWindowSettings.LfgOn));
            var op = ws?.Attribute(nameof(ChatWindowSettings.BackgroundOpacity));

            var sett = ParseWindowSettings(ws);
            var tabs = ParseTabsSettings(ts);

            return new ChatWindowSettings(sett.X, sett.Y, sett.H, sett.W,
                sett.Visible, sett.ClickThruMode,
                sett.Scale, sett.AutoDim, sett.DimOpacity,
                sett.ShowAlways, /*sett.AllowTransparency,*/
                sett.Enabled, sett.AllowOffScreen)
            {
                Tabs = tabs,
                LfgOn = lfg == null || bool.Parse(lfg.Value),
                BackgroundOpacity = op != null ? double.Parse(op.Value, CultureInfo.InvariantCulture) : 0.3
            };
        }
        private ClassPositions ParseWindowPositions(XElement windowSettingXElement)
        {
            ClassPositions positions = null;
            try
            {
                var pss = windowSettingXElement.Descendants().FirstOrDefault(s => s.Name == "Positions");
                if (pss != null) positions = new ClassPositions();
                pss?.Descendants().Where(s => s.Name == "Position").ToList().ForEach(pos =>
                {
                    var clAttr = pos.Attribute("class");
                    var pxAttr = pos.Attribute("X");
                    var pyAttr = pos.Attribute("Y");
                    if (clAttr == null || pxAttr == null || pyAttr == null) return;
                    var cl = (Class)Enum.Parse(typeof(Class), clAttr.Value);
                    var px = double.Parse(pxAttr.Value, CultureInfo.InvariantCulture);
                    var py = double.Parse(pyAttr.Value, CultureInfo.InvariantCulture);
                    positions.SetPosition(cl, new Point(px, py));
                    var bpAttr = pos.Attribute("ButtonsPosition");
                    if (bpAttr != null)
                    {
                        var bp = (ButtonsPosition)Enum.Parse(typeof(ButtonsPosition), bpAttr.Value);
                        positions.SetButtons(cl, bp);
                    }
                });
            }
            catch (Exception)
            {
                positions = null;
            }
            return positions;
        }
        private WindowSettings ParseWindowSettings(XElement ws) //TODO: don't overwrite defaults
        {
            double x = 0, y = 0, w = 0, h = 0, scale = 1, dimOp = .3;
            var ctm = ClickThruMode.Never;
            bool vis = true, enabled = true, autoDim = true, allowOffscreen = false, alwaysVis = false;
            ws.Attributes().ToList().ForEach(a =>
            {
                if (a.Name == nameof(WindowSettings.X)) x = double.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(WindowSettings.Y)) y = double.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(WindowSettings.W)) w = double.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(WindowSettings.H)) h = double.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(WindowSettings.ClickThruMode)) ctm = (ClickThruMode)Enum.Parse(typeof(ClickThruMode), a.Value);
                else if (a.Name == nameof(WindowSettings.Visible)) vis = bool.Parse(a.Value);
                else if (a.Name == nameof(WindowSettings.Scale)) scale = double.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(WindowSettings.AutoDim)) autoDim = bool.Parse(a.Value);
                else if (a.Name == nameof(WindowSettings.DimOpacity)) dimOp = double.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(WindowSettings.ShowAlways)) alwaysVis = bool.Parse(a.Value);
                else if (a.Name == nameof(WindowSettings.AllowOffScreen)) allowOffscreen = bool.Parse(a.Value);
                else if (a.Name == nameof(WindowSettings.Enabled)) enabled = bool.Parse(a.Value);
            });

            if (x > 1) x = x / Settings.ScreenW;
            if (y > 1) y = y / Settings.ScreenH;

            var positions = ParseWindowPositions(ws);

            return new WindowSettings(x, y, h, w, vis, ctm, scale, autoDim, dimOp, alwaysVis, enabled, allowOffscreen, positions, ws.Attribute("Name")?.Value);
        }
        private static void ParseGroupAbnormalSettings(XElement el)
        {
            foreach (var groupAbnormalList in Settings.GroupAbnormals)
            {
                groupAbnormalList.Value.Clear();
            }

            foreach (var abEl in el.Descendants().Where(x => x.Name == "Abnormals"))
            {
                var stringClass = abEl.Attribute("class")?.Value;
                if (stringClass == null) continue;
                var parsedClass = (Class)Enum.Parse(typeof(Class), stringClass);
                var abnormalities = abEl.Value.Split(',');
                var list = abnormalities.Length == 1 && abnormalities[0] == "" ? new List<uint>() : abnormalities.Select(uint.Parse).ToList();
                list.ForEach(abnormality => Settings.GroupAbnormals[parsedClass].Add(abnormality));
            }

            if (Settings.GroupAbnormals.Count != 0) return;
            CommonDefault.ForEach(x => Settings.GroupAbnormals[Class.Common].Add(x));
            PriestDefault.ForEach(x => Settings.GroupAbnormals[Class.Priest].Add(x));
            MysticDefault.ForEach(x => Settings.GroupAbnormals[Class.Mystic].Add(x));
        }
        //Add My Abnormals Setting by HQ ====================================================
        private static void ParseMyAbnormalSettings(XElement el)
        {
            foreach (var myAbnormalList in Settings.MyAbnormals)
            {
                myAbnormalList.Value.Clear();
            }

            foreach (var abEl in el.Descendants().Where(x => x.Name == "Abnormals"))
            {
                var stringClass = abEl.Attribute("class")?.Value;
                if (stringClass == null) continue;
                var parsedClass = (Class)Enum.Parse(typeof(Class), stringClass);
                var abnormalities = abEl.Value.Split(',');
                var list = abnormalities.Length == 1 && abnormalities[0] == "" ? new List<uint>() : abnormalities.Select(uint.Parse).ToList();
                list.ForEach(abnormality => Settings.MyAbnormals[parsedClass].Add(abnormality));
            }
            if (Settings.MyAbnormals.Count == 0)
            {
                MyCommonDefault.ForEach(x => Settings.MyAbnormals[Class.Common].Add(x));
                MyArcherDefault.ForEach(x => Settings.MyAbnormals[Class.Archer].Add(x));
                MyBerserkerDefault.ForEach(x => Settings.MyAbnormals[Class.Berserker].Add(x));
                MyBrawlerDefault.ForEach(x => Settings.MyAbnormals[Class.Brawler].Add(x));
                MyGunnerDefault.ForEach(x => Settings.MyAbnormals[Class.Gunner].Add(x));
                MyLancerDefault.ForEach(x => Settings.MyAbnormals[Class.Lancer].Add(x));
                MyMysticDefault.ForEach(x => Settings.MyAbnormals[Class.Mystic].Add(x));
                MyNinjaDefault.ForEach(x => Settings.MyAbnormals[Class.Ninja].Add(x));
                MyPriestDefault.ForEach(x => Settings.MyAbnormals[Class.Priest].Add(x));
                MyReaperDefault.ForEach(x => Settings.MyAbnormals[Class.Reaper].Add(x));
                MySlayerDefault.ForEach(x => Settings.MyAbnormals[Class.Slayer].Add(x));
                MySorcererDefault.ForEach(x => Settings.MyAbnormals[Class.Sorcerer].Add(x));
                MyValkyrieDefault.ForEach(x => Settings.MyAbnormals[Class.Valkyrie].Add(x));
                MyWarriorDefault.ForEach(x => Settings.MyAbnormals[Class.Warrior].Add(x));
            }
        }
        //===================================================================================
        public void LoadWindowSettings()
        {
            if (!File.Exists(Path.GetDirectoryName(typeof(App).Assembly.Location) + @"/tcc-config.xml")) return;
            try
            {
                _settingsDoc = XDocument.Load(Path.GetDirectoryName(typeof(App).Assembly.Location) + @"/tcc-config.xml");

                foreach (var ws in _settingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    var name = ws.Attribute("Name")?.Value;
                    if (name == null) return;
                    if (name == "BossWindow") Settings.BossWindowSettings = ParseWindowSettings(ws);
                    else if (name == "CharacterWindow") Settings.CharacterWindowSettings = ParseWindowSettings(ws);
                    else if (name == "CooldownWindow") Settings.CooldownWindowSettings = ParseWindowSettings(ws);
                    else if (name == "BuffWindow") Settings.BuffWindowSettings = ParseWindowSettings(ws);
                    else if (name == "GroupWindow") Settings.GroupWindowSettings = ParseWindowSettings(ws);
                    else if (name == "ClassWindow") Settings.ClassWindowSettings = ParseWindowSettings(ws);
                    else if (name == "FlightGaugeWindow") Settings.FlightGaugeWindowSettings = ParseWindowSettings(ws);
                    else if (name == "FloatingButton") Settings.FloatingButtonSettings = ParseWindowSettings(ws);
                    else if (name == "CivilUnrestWindow") Settings.CivilUnrestWindowSettings = ParseWindowSettings(ws);
                    //add window here
                }

                if (_settingsDoc.Descendants().Count(x => x.Name == "ChatWindow") > 0)
                {
                    _settingsDoc.Descendants().Where(x => x.Name == "ChatWindow").ToList().ForEach(s =>
                    {
                        Settings.ChatWindowsSettings.Add(ParseChatWindowSettings(s));
                    });
                }
            }
            catch (XmlException)
            {
                var res = TccMessageBox.Show("TCC",
                    "Cannot load settings file. Do you want TCC to delete it and recreate a default file?",
                    MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes) File.Delete(Path.GetDirectoryName(typeof(App).Assembly.Location) + @"/tcc-config.xml");
                LoadWindowSettings();
            }
        }
        public void LoadSettings()
        {
            try
            {
                if (!File.Exists(Path.GetDirectoryName(typeof(App).Assembly.Location) + @"/tcc-config.xml")) return;
                _settingsDoc = XDocument.Load(Path.GetDirectoryName(typeof(App).Assembly.Location) + @"/tcc-config.xml");

                var b = _settingsDoc.Descendants("OtherSettings").FirstOrDefault();
                if (b == null) return;
                b.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == nameof(Settings.IgnoreMeInGroupWindow)) Settings.IgnoreMeInGroupWindow = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.IgnoreGroupBuffs)) Settings.IgnoreGroupBuffs = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.IgnoreGroupDebuffs)) Settings.IgnoreGroupDebuffs = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.BuffsDirection)) Settings.BuffsDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), attr.Value);
                    else if (attr.Name == nameof(Settings.CooldownBarMode)) Settings.CooldownBarMode = (CooldownBarMode)Enum.Parse(typeof(CooldownBarMode), attr.Value);
                    else if (attr.Name == nameof(Settings.EnrageLabelMode)) Settings.EnrageLabelMode = (EnrageLabelMode)Enum.Parse(typeof(EnrageLabelMode), attr.Value);
                    else if (attr.Name == nameof(Settings.ChatClickThruMode)) Settings.ChatClickThruMode = (ClickThruMode)Enum.Parse(typeof(ClickThruMode), attr.Value);
                    else if (attr.Name == nameof(Settings.WarriorEdgeMode)) Settings.WarriorEdgeMode = (WarriorEdgeMode)Enum.Parse(typeof(WarriorEdgeMode), attr.Value);
                    else if (attr.Name == nameof(Settings.AbnormalityShape)) Settings.AbnormalityShape = (ControlShape)Enum.Parse(typeof(ControlShape), attr.Value);
                    else if (attr.Name == nameof(Settings.SkillShape)) Settings.SkillShape = (ControlShape)Enum.Parse(typeof(ControlShape), attr.Value);
                    else if (attr.Name == nameof(Settings.MaxMessages)) Settings.MaxMessages = int.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.SpamThreshold)) Settings.SpamThreshold = int.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.FontSize)) Settings.FontSize = int.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowChannel)) Settings.ShowChannel = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowTimestamp)) Settings.ShowTimestamp = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowOnlyBosses)) Settings.ShowOnlyBosses = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.DisablePartyHP)) Settings.DisablePartyHP = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.DisablePartyMP)) Settings.DisablePartyMP = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowOnlyAggroStacks)) Settings.ShowOnlyAggroStacks = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.DisablePartyAbnormals)) Settings.DisablePartyAbnormals = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ChatFadeOut)) Settings.ChatFadeOut = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowItemsCooldown)) Settings.ShowItemsCooldown = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowMembersLaurels)) Settings.ShowMembersLaurels = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.AnimateChatMessages)) Settings.AnimateChatMessages = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.StatSent)) Settings.StatSent = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowFlightEnergy)) Settings.ShowFlightEnergy = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.LfgEnabled)) Settings.LfgEnabled = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowGroupWindowDetails)) Settings.ShowGroupWindowDetails = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.UseHotkeys)) Settings.UseHotkeys = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.HideHandles)) Settings.HideHandles = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ChatEnabled)) Settings.ChatEnabled = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowTradeLfg)) Settings.ShowTradeLfg = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowAwakenIcon)) Settings.ShowAwakenIcon = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.EthicalMode)) Settings.EthicalMode = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.AccurateHp)) Settings.AccurateHp = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.WarriorShowEdge)) Settings.WarriorShowEdge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.SorcererReplacesElementsInCharWindow)) Settings.SorcererReplacesElementsInCharWindow = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.FlipFlightGauge)) Settings.FlipFlightGauge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.WarriorShowTraverseCut)) Settings.WarriorShowTraverseCut = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.CharacterWindowCompactMode)) Settings.CharacterWindowCompactMode = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowAllGroupAbnormalities)) Settings.ShowAllGroupAbnormalities = bool.Parse(attr.Value);
                    //Add My Abnormals Setting by HQ ====================================================
                    else if (attr.Name == nameof(Settings.ShowAllMyAbnormalities)) Settings.ShowAllMyAbnormalities = bool.Parse(attr.Value);
                    //===================================================================================
                    else if (attr.Name == nameof(Settings.HighPriority)) Settings.HighPriority = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ForceSoftwareRendering)) Settings.ForceSoftwareRendering = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.Winpcap)) Settings.Winpcap = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.RegionOverride)) Settings.RegionOverride = attr.Value;
                    else if (attr.Name == nameof(Settings.LastRegion)) Settings.LastRegion = attr.Value;
                    else if (attr.Name == nameof(Settings.Webhook)) Settings.Webhook = attr.Value;
                    else if (attr.Name == nameof(Settings.WebhookMessage)) Settings.WebhookMessage = attr.Value;
                    else if (attr.Name == nameof(Settings.ChatWindowOpacity)) Settings.ChatWindowOpacity = double.Parse(attr.Value, CultureInfo.InvariantCulture);
                    else if (attr.Name == nameof(Settings.FlightGaugeRotation)) Settings.FlightGaugeRotation = double.Parse(attr.Value, CultureInfo.InvariantCulture);
                    else if (attr.Name == nameof(Settings.LastRun)) Settings.LastRun = DateTime.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.TwitchName)) Settings.TwitchName = attr.Value;
                    else if (attr.Name == nameof(Settings.TwitchToken)) Settings.TwitchToken = attr.Value;
                    else if (attr.Name == nameof(Settings.TwitchChannelName)) Settings.TwitchChannelName = attr.Value;
                    else if (attr.Name == nameof(Settings.GroupSizeThreshold)) Settings.GroupSizeThreshold = uint.Parse(attr.Value);
                    //add settings here
                });

                try
                {
                    ParseGroupAbnormalSettings(_settingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(Settings.GroupAbnormals)));
                }
                catch
                {
                    CommonDefault.ForEach(x => Settings.GroupAbnormals[Class.Common].Add(x));
                    PriestDefault.ForEach(x => Settings.GroupAbnormals[Class.Priest].Add(x));
                    MysticDefault.ForEach(x => Settings.GroupAbnormals[Class.Mystic].Add(x));
                }
                //Add My Abnormals Setting by HQ ====================================================
                try
                {
                    ParseMyAbnormalSettings(_settingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(Settings.MyAbnormals)));
                }
                catch
                {
                    MyCommonDefault.ForEach(x => Settings.MyAbnormals[Class.Common].Add(x));
                    MyArcherDefault.ForEach(x => Settings.MyAbnormals[Class.Archer].Add(x));
                    MyBerserkerDefault.ForEach(x => Settings.MyAbnormals[Class.Berserker].Add(x));
                    MyBrawlerDefault.ForEach(x => Settings.MyAbnormals[Class.Brawler].Add(x));
                    MyGunnerDefault.ForEach(x => Settings.MyAbnormals[Class.Gunner].Add(x));
                    MyLancerDefault.ForEach(x => Settings.MyAbnormals[Class.Lancer].Add(x));
                    MyMysticDefault.ForEach(x => Settings.MyAbnormals[Class.Mystic].Add(x));
                    MyNinjaDefault.ForEach(x => Settings.MyAbnormals[Class.Ninja].Add(x));
                    MyPriestDefault.ForEach(x => Settings.MyAbnormals[Class.Priest].Add(x));
                    MyReaperDefault.ForEach(x => Settings.MyAbnormals[Class.Reaper].Add(x));
                    MySlayerDefault.ForEach(x => Settings.MyAbnormals[Class.Slayer].Add(x));
                    MySorcererDefault.ForEach(x => Settings.MyAbnormals[Class.Sorcerer].Add(x));
                    MyValkyrieDefault.ForEach(x => Settings.MyAbnormals[Class.Valkyrie].Add(x));
                    MyWarriorDefault.ForEach(x => Settings.MyAbnormals[Class.Warrior].Add(x));
                }
                //===================================================================================
            }
            catch (XmlException)
            {
                var res = TccMessageBox.Show("TCC",
                    "Cannot load settings file. Do you want TCC to delete it and recreate a default file?",
                    MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes) File.Delete(Path.GetDirectoryName(typeof(App).Assembly.Location) + @"/tcc-config.xml");
                LoadSettings();
            }
        }

    }
}