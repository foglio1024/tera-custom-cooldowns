using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using TCC.Data;
using TCC.ViewModels;
using TCC.Windows;
using TeraDataLite;
using MessageBoxImage = TCC.Data.MessageBoxImage;

// ReSharper disable CollectionNeverUpdated.Local

namespace TCC.Settings
{
    public class XmlSettingsReader : SettingsReaderBase
    {
        private XDocument _settingsDoc;

        public XmlSettingsReader()
        {
            FileName = SettingsGlobals.XmlFileName;
            App.Settings = new SettingsContainer();
        }

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
            var lfgOn = false;
            var fadeOut = true;
            var backgroundOpacity = .3;
            var frameOpacity = 1f;
            var hideTimeout = 10;
            ws?.Attributes().ToList().ForEach(a =>
            {
                if (a.Name == nameof(ChatWindowSettings.LfgOn)) lfgOn = bool.Parse(a.Value);
                else if (a.Name == nameof(ChatWindowSettings.FadeOut)) fadeOut = bool.Parse(a.Value);
                else if (a.Name == nameof(ChatWindowSettings.BackgroundOpacity)) backgroundOpacity = float.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(ChatWindowSettings.FrameOpacity)) frameOpacity = float.Parse(a.Value, CultureInfo.InvariantCulture);
                else if (a.Name == nameof(ChatWindowSettings.HideTimeout)) hideTimeout = int.Parse(a.Value);
            });

            var sett = ParseWindowSettings(ws);
            var tabs = ParseTabsSettings(ts);

            return new ChatWindowSettings(sett)
            {
                Tabs = tabs,
                LfgOn = lfgOn,
                BackgroundOpacity = backgroundOpacity,
                FrameOpacity = frameOpacity,
                FadeOut = fadeOut,
                HideTimeout = hideTimeout
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

            if (x > 1) x /= WindowManager.ScreenSize.Width;
            if (y > 1) y /= WindowManager.ScreenSize.Height;

            var positions = ParseWindowPositions(ws);

            return new WindowSettings(x, y, h, w, vis, ctm, scale, autoDim, dimOp, alwaysVis, enabled, allowOffscreen, positions, ws.Attribute("Name")?.Value);
        }
        private static void ParseGroupAbnormalSettings(XElement el)
        {
            foreach (var groupAbnormalList in App.Settings.GroupAbnormals)
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
                list.ForEach(abnormality => App.Settings.GroupAbnormals[parsedClass].Add(abnormality));
            }

            if (App.Settings.GroupAbnormals.Count != 0) return;
            CommonDefault.ForEach(x => App.Settings.GroupAbnormals[Class.Common].Add(x));
            PriestDefault.ForEach(x => App.Settings.GroupAbnormals[Class.Priest].Add(x));
            MysticDefault.ForEach(x => App.Settings.GroupAbnormals[Class.Mystic].Add(x));
        }
        //Add My Abnormals Setting by HQ ====================================================
        private static void ParseMyAbnormalSettings(XElement el)
        {
            foreach (var myAbnormalList in App.Settings.MyAbnormals)
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
                list.ForEach(abnormality => App.Settings.MyAbnormals[parsedClass].Add(abnormality));
            }
            if (App.Settings.MyAbnormals.Count == 0)
            {
                MyCommonDefault.ForEach(x => App.Settings.MyAbnormals[Class.Common].Add(x));
                MyArcherDefault.ForEach(x => App.Settings.MyAbnormals[Class.Archer].Add(x));
                MyBerserkerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Berserker].Add(x));
                MyBrawlerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Brawler].Add(x));
                MyGunnerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Gunner].Add(x));
                MyLancerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Lancer].Add(x));
                MyMysticDefault.ForEach(x => App.Settings.MyAbnormals[Class.Mystic].Add(x));
                MyNinjaDefault.ForEach(x => App.Settings.MyAbnormals[Class.Ninja].Add(x));
                MyPriestDefault.ForEach(x => App.Settings.MyAbnormals[Class.Priest].Add(x));
                MyReaperDefault.ForEach(x => App.Settings.MyAbnormals[Class.Reaper].Add(x));
                MySlayerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Slayer].Add(x));
                MySorcererDefault.ForEach(x => App.Settings.MyAbnormals[Class.Sorcerer].Add(x));
                MyValkyrieDefault.ForEach(x => App.Settings.MyAbnormals[Class.Valkyrie].Add(x));
                MyWarriorDefault.ForEach(x => App.Settings.MyAbnormals[Class.Warrior].Add(x));
            }
        }
        //===================================================================================
        public void LoadWindowSettings(string pathOverride = null)
        {

            var path = Path.Combine(App.BasePath, FileName);
            if (pathOverride != null) path = pathOverride;
            if (!File.Exists(path)) return;

            try
            {
                _settingsDoc = XDocument.Load(Path.Combine(App.BasePath, FileName));

                foreach (var ws in _settingsDoc.Descendants().Where(x => x.Name == "WindowSetting"))
                {
                    var name = ws.Attribute("Name")?.Value;
                    if (name == null) return;
                    if (name == "BossWindow") App.Settings.BossWindowSettings = ParseWindowSettings(ws);
                    else if (name == "CharacterWindow") App.Settings.CharacterWindowSettings = ParseWindowSettings(ws);
                    else if (name == "CooldownWindow") App.Settings.CooldownWindowSettings = ParseWindowSettings(ws);
                    else if (name == "BuffWindow") App.Settings.BuffWindowSettings = ParseWindowSettings(ws);
                    else if (name == "GroupWindow") App.Settings.GroupWindowSettings = ParseWindowSettings(ws);
                    else if (name == "ClassWindow") App.Settings.ClassWindowSettings = ParseWindowSettings(ws);
                    else if (name == "FlightGaugeWindow") App.Settings.FlightGaugeWindowSettings = ParseWindowSettings(ws);
                    else if (name == "FloatingButton") App.Settings.FloatingButtonSettings = ParseWindowSettings(ws);
                    else if (name == "CivilUnrestWindow") App.Settings.CivilUnrestWindowSettings = ParseWindowSettings(ws);
                    //add window here
                }

                if (_settingsDoc.Descendants().Count(x => x.Name == "ChatWindow") > 0)
                {
                    _settingsDoc.Descendants().Where(x => x.Name == "ChatWindow").ToList().ForEach(s =>
                    {
                        App.Settings.ChatWindowsSettings.Add(ParseChatWindowSettings(s));
                    });
                }
            }
            catch (XmlException)
            {
                var res = TccMessageBox.Show("TCC",
                    "Cannot load settings file. Do you want TCC to delete it and recreate a default file?",
                    MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (res == MessageBoxResult.Yes) File.Delete(Path.Combine(App.BasePath, FileName));
                LoadWindowSettings(pathOverride);
            }
        }
        public void LoadSettings(string pathOverride = null)
        {

            try
            {
                var path = Path.Combine(App.BasePath, FileName);
                if (pathOverride != null) path = pathOverride;

                if (!File.Exists(path))
                {
                    var res = TccMessageBox.Show("Settings file not found. Do you want to import an existing one?",
                        MessageBoxType.ConfirmationWithYesNo);
                    if(res == MessageBoxResult.No) return;
                    var diag = new OpenFileDialog
                    {
                        Title = $"Import TCC settings file ({FileName})",
                        Filter = $"{FileName} (*.xml)|*.xml"
                    };
                    if (diag.ShowDialog() == true)
                    {
                        path = diag.FileName;
                    }
                    else return;
                }
                _settingsDoc = XDocument.Load(path);

                var b = _settingsDoc.Descendants(SettingsGlobals.OtherSettingsKey).FirstOrDefault();
                if (b == null) return;
                b.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == nameof(SettingsContainer.IgnoreMeInGroupWindow)) App.Settings.IgnoreMeInGroupWindow = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.IgnoreGroupBuffs)) App.Settings.IgnoreGroupBuffs = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.IgnoreGroupDebuffs)) App.Settings.IgnoreGroupDebuffs = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.BuffsDirection)) App.Settings.BuffsDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.CooldownBarMode)) App.Settings.CooldownBarMode = (CooldownBarMode)Enum.Parse(typeof(CooldownBarMode), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.EnrageLabelMode)) App.Settings.EnrageLabelMode = (EnrageLabelMode)Enum.Parse(typeof(EnrageLabelMode), attr.Value);
                    //else if (attr.Name == nameof(SettingsContainer.ChatClickThruMode)) App.Settings.ChatClickThruMode = (ClickThruMode)Enum.Parse(typeof(ClickThruMode), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WarriorEdgeMode)) App.Settings.WarriorEdgeMode = (WarriorEdgeMode)Enum.Parse(typeof(WarriorEdgeMode), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.AbnormalityShape)) App.Settings.AbnormalityShape = (ControlShape)Enum.Parse(typeof(ControlShape), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.SkillShape)) App.Settings.SkillShape = (ControlShape)Enum.Parse(typeof(ControlShape), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.GroupWindowLayout)) App.Settings.GroupWindowLayout = (GroupWindowLayout)Enum.Parse(typeof(GroupWindowLayout), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.CaptureMode)) App.Settings.CaptureMode = (CaptureMode)Enum.Parse(typeof(CaptureMode), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.MaxMessages)) App.Settings.MaxMessages = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.SpamThreshold)) App.Settings.SpamThreshold = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.FontSize)) App.Settings.FontSize = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ChatScrollAmount)) App.Settings.ChatScrollAmount = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowChannel)) App.Settings.ShowChannel = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ChatTimestampSeconds)) App.Settings.ChatTimestampSeconds = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowTimestamp)) App.Settings.ShowTimestamp = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowOnlyBosses)) App.Settings.ShowOnlyBosses = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.DisablePartyHP)) App.Settings.DisablePartyHP = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.DisablePartyMP)) App.Settings.DisablePartyMP = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowOnlyAggroStacks)) App.Settings.ShowOnlyAggroStacks = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.DisablePartyAbnormals)) App.Settings.DisablePartyAbnormals = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowItemsCooldown)) App.Settings.ShowItemsCooldown = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowMembersLaurels)) App.Settings.ShowMembersLaurels = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.AnimateChatMessages)) App.Settings.AnimateChatMessages = bool.Parse(attr.Value);
                    //else if (attr.Name == nameof(SettingsContainer.StatSent)) App.Settings.StatSent = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowFlightEnergy)) App.Settings.ShowFlightEnergy = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.LfgEnabled)) App.Settings.LfgEnabled = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowGroupWindowDetails)) App.Settings.ShowGroupWindowDetails = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.UseHotkeys)) App.Settings.UseHotkeys = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.HideHandles)) App.Settings.HideHandles = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ChatEnabled)) App.Settings.ChatEnabled = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowTradeLfg)) App.Settings.ShowTradeLfg = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowAwakenIcon)) App.Settings.ShowAwakenIcon = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.EthicalMode)) App.Settings.EthicalMode = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.AccurateHp)) App.Settings.AccurateHp = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WarriorShowEdge)) App.Settings.WarriorShowEdge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.SorcererReplacesElementsInCharWindow)) App.Settings.SorcererReplacesElementsInCharWindow = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.FlipFlightGauge)) App.Settings.FlipFlightGauge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WarriorShowTraverseCut)) App.Settings.WarriorShowTraverseCut = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.CharacterWindowCompactMode)) App.Settings.CharacterWindowCompactMode = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowAllGroupAbnormalities)) App.Settings.ShowAllGroupAbnormalities = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowAllMyAbnormalities)) App.Settings.ShowAllMyAbnormalities = bool.Parse(attr.Value); // MyAbnormals Setting by HQ 
                    else if (attr.Name == nameof(SettingsContainer.HighPriority)) App.Settings.HighPriority = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ForceSoftwareRendering)) App.Settings.ForceSoftwareRendering = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.Npcap)) App.Settings.Npcap = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ExperimentalNotification)) App.Settings.ExperimentalNotification = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.LanguageOverride)) App.Settings.LanguageOverride = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.LastLanguage)) App.Settings.LastLanguage = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.FlightGaugeRotation)) App.Settings.FlightGaugeRotation = double.Parse(attr.Value, CultureInfo.InvariantCulture);
                    else if (attr.Name == nameof(SettingsContainer.LastRun)) App.Settings.LastRun = DateTime.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.TwitchName)) App.Settings.TwitchName = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.TwitchToken)) App.Settings.TwitchToken = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.StatSentVersion)) App.Settings.StatSentVersion = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.TwitchChannelName)) App.Settings.TwitchChannelName = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.GroupSizeThreshold)) App.Settings.GroupSizeThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowMembersHpNumbers)) App.Settings.ShowMembersHpNumbers = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.DisableLfgChatMessages)) App.Settings.DisableLfgChatMessages = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.CheckGuildBamWithoutOpcode)) App.Settings.CheckGuildBamWithoutOpcode = bool.Parse(attr.Value);

                    else if (attr.Name == nameof(SettingsContainer.HideBuffsThreshold)) App.Settings.HideBuffsThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.HideDebuffsThreshold)) App.Settings.HideDebuffsThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.DisableAbnormalitiesThreshold)) App.Settings.DisableAbnormalitiesThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.HideHpThreshold)) App.Settings.HideHpThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.HideMpThreshold)) App.Settings.HideMpThreshold = uint.Parse(attr.Value);

                    else if (attr.Name == "Webhook") App.Settings.WebhookUrlGuildBam = attr.Value; // for retrocompat
                    else if (attr.Name == "WebhookMessage") App.Settings.WebhookMessageGuildBam = attr.Value; // for retrocompat

                    else if (attr.Name == nameof(SettingsContainer.WebhookEnabledGuildBam)) App.Settings.WebhookEnabledGuildBam = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WebhookEnabledFieldBoss)) App.Settings.WebhookEnabledFieldBoss = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WebhookUrlGuildBam)) App.Settings.WebhookUrlGuildBam = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookUrlFieldBoss)) App.Settings.WebhookUrlFieldBoss = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookMessageGuildBam)) App.Settings.WebhookMessageGuildBam = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookMessageFieldBossSpawn)) App.Settings.WebhookMessageFieldBossSpawn = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookMessageFieldBossDie)) App.Settings.WebhookMessageFieldBossDie = attr.Value;

                    else if (attr.Name == nameof(SettingsContainer.CheckOpcodesHash)) App.Settings.CheckOpcodesHash = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowNotificationBubble)) App.Settings.ShowNotificationBubble = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.UserExcludedSysMsg)) App.Settings.UserExcludedSysMsg = ParseUserExcludedSysMsg(attr.Value);

                    else if (attr.Name == nameof(SettingsContainer.FpsAtGuardian)) App.Settings.FpsAtGuardian = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.EnableProxy)) App.Settings.EnableProxy = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.DontShowFUBH)) App.Settings.DontShowFUBH = bool.Parse(attr.Value);

                    else if (attr.Name == nameof(SettingsContainer.LastScreenSize))
                    {
                        var val = attr.Value.Split(',');
                        App.Settings.LastScreenSize = new Size(float.Parse(val[0], CultureInfo.InvariantCulture), float.Parse(val[1], CultureInfo.InvariantCulture));
                    }
                    //add settings here
                });

                try
                {
                    ParseGroupAbnormalSettings(_settingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(SettingsContainer.GroupAbnormals)));
                }
                catch
                {
                    CommonDefault.ForEach(x => App.Settings.GroupAbnormals[Class.Common].Add(x));
                    PriestDefault.ForEach(x => App.Settings.GroupAbnormals[Class.Priest].Add(x));
                    MysticDefault.ForEach(x => App.Settings.GroupAbnormals[Class.Mystic].Add(x));
                }
                //Add My Abnormals Setting by HQ ====================================================
                try
                {
                    ParseMyAbnormalSettings(_settingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(SettingsContainer.MyAbnormals)));
                }
                catch
                {
                    MyCommonDefault.ForEach(x => App.Settings.MyAbnormals[Class.Common].Add(x));
                    MyArcherDefault.ForEach(x => App.Settings.MyAbnormals[Class.Archer].Add(x));
                    MyBerserkerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Berserker].Add(x));
                    MyBrawlerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Brawler].Add(x));
                    MyGunnerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Gunner].Add(x));
                    MyLancerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Lancer].Add(x));
                    MyMysticDefault.ForEach(x => App.Settings.MyAbnormals[Class.Mystic].Add(x));
                    MyNinjaDefault.ForEach(x => App.Settings.MyAbnormals[Class.Ninja].Add(x));
                    MyPriestDefault.ForEach(x => App.Settings.MyAbnormals[Class.Priest].Add(x));
                    MyReaperDefault.ForEach(x => App.Settings.MyAbnormals[Class.Reaper].Add(x));
                    MySlayerDefault.ForEach(x => App.Settings.MyAbnormals[Class.Slayer].Add(x));
                    MySorcererDefault.ForEach(x => App.Settings.MyAbnormals[Class.Sorcerer].Add(x));
                    MyValkyrieDefault.ForEach(x => App.Settings.MyAbnormals[Class.Valkyrie].Add(x));
                    MyWarriorDefault.ForEach(x => App.Settings.MyAbnormals[Class.Warrior].Add(x));
                }
                //===================================================================================
            }
            catch (XmlException)
            {
                var res = TccMessageBox.Show("TCC",
                    "Cannot load settings file. Do you want TCC to delete it and recreate a default file?",
                    MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (res == MessageBoxResult.Yes) File.Delete(Path.Combine(App.BasePath, FileName));
                LoadSettings(pathOverride);
            }
        }

        private List<string> ParseUserExcludedSysMsg(string value)
        {
            var opcodes = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return opcodes.Where(item => !string.IsNullOrEmpty(item)).ToList();
        }
    }
}