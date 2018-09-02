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

namespace TCC
{
    public class SettingsReader
    {
        private XDocument _settingsDoc;

        private static readonly List<uint> CommonDefault = new List<uint> { 4000, 4001, 4010, 4011, 4020, 4021, 4030, 4031, 4600, 4610, 4611, 4613, 5000003, 4830, 4831, 4833, 4841, 4886, 4861, 4953, 4955, 7777015, 902, 910, 911, 912, 913, 916, 920, 921, 922, 999010000 };
        private static readonly List<uint> PriestDefault = new List<uint> { 201, 202, 805100, 805101, 805102, 98000109, 805600, 805601, 805602, 805603, 805604, 98000110, 800300, 800301, 800302, 800303, 800304, 801500, 801501, 801502, 801503, 98000107 };
        private static readonly List<uint> MysticDefault = new List<uint> { 27120, 700630, 700631, 601, 602, 603, 700330, 700230, 700231, 800132, 700233, 700730, 700731, 700100 };

        private List<Tab> ParseTabsSettings(XElement elem)
        {
            var result = new List<Tab>();
            if (elem == null) return result;
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
            return result;
        }
        private ChatWindowSettings ParseChatWindowSettings(XContainer s)
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
                sett.Enabled, sett.AllowOffScreen)
            {
                Tabs = tabs,
                LfgOn = lfg == null || bool.Parse(lfg.Value),
                BackgroundOpacity = op != null ? double.Parse(op.Value, CultureInfo.InvariantCulture) : 0.3
            };
        }
        private Dictionary<Class, Point> ParseWindowPositions(XElement windowSettingXElement)
        {
            Dictionary<Class, Point> positions = null;
            try
            {
                var pss = windowSettingXElement.Descendants().FirstOrDefault(s => s.Name == "Positions");
                if (pss != null) positions = new Dictionary<Class, Point>();
                pss?.Descendants().Where(s => s.Name == "Position").ToList().ForEach(pos =>
                {
                    var cl = (Class)Enum.Parse(typeof(Class), pos.Attribute("class").Value);
                    var px = double.Parse(pos.Attribute("X")?.Value, CultureInfo.InvariantCulture);
                    var py = double.Parse(pos.Attribute("Y")?.Value, CultureInfo.InvariantCulture);
                    positions.Add(cl, new Point(px, py));
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

            return new WindowSettings(x, y, h, w, vis, ctm, scale, autoDim, dimOp, alwaysVis, enabled, allowOffscreen, positions);
        }
        private static void ParseGroupAbnormalSettings(XElement el)
        {
            foreach (var groupAbnormalList in Settings.GroupAbnormals)
            {
                groupAbnormalList.Value.Clear();
            }

            foreach (var abEl in el.Descendants().Where(x => x.Name == "Abnormals"))
            {
                var stringClass = abEl.Attribute("class").Value;
                var parsedClass = (Class)Enum.Parse(typeof(Class), stringClass);
                var abnormalities = abEl.Value.Split(',');
                var list = abnormalities.Length == 1 && abnormalities[0] == "" ? new List<uint>() : abnormalities.Select(uint.Parse).ToList();
                list.ForEach(abnormality => Settings.GroupAbnormals[parsedClass].Add(abnormality));
                //GroupAbnormals.Add(cl, l);
            }

            if (Settings.GroupAbnormals.Count != 0) return;
            CommonDefault.ForEach(x => Settings.GroupAbnormals[Class.Common].Add(x));
            PriestDefault.ForEach(x => Settings.GroupAbnormals[Class.Priest].Add(x));
            MysticDefault.ForEach(x => Settings.GroupAbnormals[Class.Mystic].Add(x));
        }

        public void LoadWindowSettings()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml")) return;
            try
            {
                _settingsDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");

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
                if (res == MessageBoxResult.Yes) File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");
                LoadWindowSettings();
            }
        }
        public void LoadSettings()
        {
            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml")) return;
                _settingsDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"/tcc-config.xml");

                //TODO: iterate thru attributes and just check names (like parsewindowsettings)
                var b = _settingsDoc.Descendants("OtherSettings").FirstOrDefault();
                if (b == null) return;
                b.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == nameof(Settings.IgnoreMeInGroupWindow)) Settings.IgnoreMeInGroupWindow = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.IgnoreGroupBuffs)) Settings.IgnoreGroupBuffs = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.IgnoreGroupDebuffs)) Settings.IgnoreGroupDebuffs = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.BuffsDirection)) Settings.BuffsDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), attr.Value);
                    else if (attr.Name == nameof(Settings.CooldownBarMode)) Settings.CooldownBarMode = (CooldownBarMode)Enum.Parse(typeof(CooldownBarMode), attr.Value);
                    else if (attr.Name == nameof(Settings.CooldownBarMode)) Settings.CooldownBarMode = (CooldownBarMode)Enum.Parse(typeof(CooldownBarMode), attr.Value);
                    else if (attr.Name == nameof(Settings.EnrageLabelMode)) Settings.EnrageLabelMode = (EnrageLabelMode)Enum.Parse(typeof(EnrageLabelMode), attr.Value);
                    else if (attr.Name == nameof(Settings.ChatClickThruMode)) Settings.ChatClickThruMode = (ClickThruMode)Enum.Parse(typeof(ClickThruMode), attr.Value);
                    else if (attr.Name == nameof(Settings.WarriorEdgeMode)) Settings.WarriorEdgeMode = (WarriorEdgeMode)Enum.Parse(typeof(WarriorEdgeMode), attr.Value);
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
                    else if (attr.Name == nameof(Settings.AccurateHp)) Settings.AccurateHp = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.WarriorShowEdge)) Settings.WarriorShowEdge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.FlipFlightGauge)) Settings.FlipFlightGauge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.WarriorShowTraverseCut)) Settings.WarriorShowTraverseCut = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.CharacterWindowCompactMode)) Settings.CharacterWindowCompactMode = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(Settings.ShowAllGroupAbnormalities)) Settings.ShowAllGroupAbnormalities = bool.Parse(attr.Value);
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

    }
}