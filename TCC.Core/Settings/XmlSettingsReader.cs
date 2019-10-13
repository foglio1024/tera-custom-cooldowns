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

        private List<TabInfo> ParseTabsSettings(XElement elem)
        {
            var result = new List<TabInfo>();
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

                var tabData = new TabInfo(tabName);
                channels.ForEach(tabData.ShowedChannels.Add);
                exChannels.ForEach(tabData.HiddenChannels.Add);
                authors.ForEach(tabData.ShowedAuthors.Add);
                exAuthors.ForEach(tabData.HiddenAuthors.Add);

                //result.Add(new Tab(tabName, channels.ToArray(), exChannels.ToArray(), authors.ToArray(), exAuthors.ToArray()));
                result.Add(tabData);
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
            foreach (var groupAbnormalList in App.Settings.GroupWindowSettings.GroupAbnormals)
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
                list.ForEach(abnormality => App.Settings.GroupWindowSettings.GroupAbnormals[parsedClass].Add(abnormality));
            }

            if (App.Settings.GroupWindowSettings.GroupAbnormals.Count != 0) return;
            CommonDefault.ForEach(x => App.Settings.GroupWindowSettings.GroupAbnormals[Class.Common].Add(x));
            PriestDefault.ForEach(x => App.Settings.GroupWindowSettings.GroupAbnormals[Class.Priest].Add(x));
            MysticDefault.ForEach(x => App.Settings.GroupWindowSettings.GroupAbnormals[Class.Mystic].Add(x));
        }
        //Add My Abnormals Setting by HQ ====================================================
        private static void ParseMyAbnormalSettings(XElement el)
        {
            foreach (var myAbnormalList in App.Settings.BuffWindowSettings.MyAbnormals)
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
                list.ForEach(abnormality => App.Settings.BuffWindowSettings.MyAbnormals[parsedClass].Add(abnormality));
            }
            if (App.Settings.BuffWindowSettings.MyAbnormals.Count == 0)
            {
                MyCommonDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Common].Add(x));
                MyArcherDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Archer].Add(x));
                MyBerserkerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Berserker].Add(x));
                MyBrawlerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Brawler].Add(x));
                MyGunnerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Gunner].Add(x));
                MyLancerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Lancer].Add(x));
                MyMysticDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Mystic].Add(x));
                MyNinjaDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Ninja].Add(x));
                MyPriestDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Priest].Add(x));
                MyReaperDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Reaper].Add(x));
                MySlayerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Slayer].Add(x));
                MySorcererDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Sorcerer].Add(x));
                MyValkyrieDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Valkyrie].Add(x));
                MyWarriorDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Warrior].Add(x));
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
                    switch (name)
                    {
                        case null:
                            return;
                        case "BossWindow":
                            App.Settings.NpcWindowSettings = ParseWindowSettings(ws) as NpcWindowSettings;
                            break;
                        case "CharacterWindow":
                            App.Settings.CharacterWindowSettings = ParseWindowSettings(ws) as CharacterWindowSettings;
                            break;
                        case "CooldownWindow":
                            App.Settings.CooldownWindowSettings = ParseWindowSettings(ws) as CooldownWindowSettings;
                            break;
                        case "BuffWindow":
                            App.Settings.BuffWindowSettings = ParseWindowSettings(ws) as BuffWindowSettings;
                            break;
                        case "GroupWindow":
                            App.Settings.GroupWindowSettings = ParseWindowSettings(ws) as GroupWindowSettings;
                            break;
                        case "ClassWindow":
                            App.Settings.ClassWindowSettings = ParseWindowSettings(ws) as ClassWindowSettings;
                            break;
                        case "FlightGaugeWindow":
                            App.Settings.FlightGaugeWindowSettings = ParseWindowSettings(ws) as FlightWindowSettings;
                            break;
                        case "FloatingButton":
                            App.Settings.FloatingButtonSettings = ParseWindowSettings(ws) as FloatingButtonWindowSettings;
                            break;
                        case "CivilUnrestWindow":
                            App.Settings.CivilUnrestWindowSettings = ParseWindowSettings(ws) as CivilUnrestWindowSettings;
                            break;
                    }

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
                    if (attr.Name == "IgnoreMeInGroupWindow") App.Settings.GroupWindowSettings.IgnoreMe = bool.Parse(attr.Value);
                    else if (attr.Name == "EnrageLabelMode") App.Settings.NpcWindowSettings.EnrageLabelMode = (EnrageLabelMode)Enum.Parse(typeof(EnrageLabelMode), attr.Value);
                    else if (attr.Name == "ShowOnlyBosses") App.Settings.NpcWindowSettings.HideAdds = bool.Parse(attr.Value);
                    else if (attr.Name == "AccurateHp") App.Settings.NpcWindowSettings.AccurateHp = bool.Parse(attr.Value);
                    else if (attr.Name == "BuffsDirection") App.Settings.BuffWindowSettings.Direction = (FlowDirection)Enum.Parse(typeof(FlowDirection), attr.Value);
                    else if (attr.Name == "ShowAllMyAbnormalities") App.Settings.BuffWindowSettings.ShowAll = bool.Parse(attr.Value); // MyAbnormals Setting by HQ 
                    else if (attr.Name == "CooldownBarMode") App.Settings.CooldownWindowSettings.Mode = (CooldownBarMode)Enum.Parse(typeof(CooldownBarMode), attr.Value);
                    else if (attr.Name == "ShowItemsCooldown") App.Settings.CooldownWindowSettings.ShowItems = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(ClassWindowSettings.WarriorEdgeMode)) App.Settings.ClassWindowSettings.WarriorEdgeMode = (WarriorEdgeMode)Enum.Parse(typeof(WarriorEdgeMode), attr.Value);
                    else if (attr.Name == nameof(ClassWindowSettings.WarriorShowEdge)) App.Settings.ClassWindowSettings.WarriorShowEdge = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(ClassWindowSettings.SorcererReplacesElementsInCharWindow)) App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(ClassWindowSettings.WarriorShowTraverseCut)) App.Settings.ClassWindowSettings.WarriorShowTraverseCut = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.AbnormalityShape)) App.Settings.AbnormalityShape = (ControlShape)Enum.Parse(typeof(ControlShape), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.SkillShape)) App.Settings.SkillShape = (ControlShape)Enum.Parse(typeof(ControlShape), attr.Value);
                    else if (attr.Name == "GroupWindowLayout") App.Settings.GroupWindowSettings.Layout = (GroupWindowLayout)Enum.Parse(typeof(GroupWindowLayout), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.CaptureMode)) App.Settings.CaptureMode = (CaptureMode)Enum.Parse(typeof(CaptureMode), attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.MaxMessages)) App.Settings.MaxMessages = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.SpamThreshold)) App.Settings.SpamThreshold = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.FontSize)) App.Settings.FontSize = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ChatScrollAmount)) App.Settings.ChatScrollAmount = int.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowChannel)) App.Settings.ShowChannel = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ChatTimestampSeconds)) App.Settings.ChatTimestampSeconds = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowTimestamp)) App.Settings.ShowTimestamp = bool.Parse(attr.Value);
                    else if (attr.Name == "ShowOnlyAggroStacks") App.Settings.GroupWindowSettings.ShowOnlyAggroStacks = bool.Parse(attr.Value);
                    else if (attr.Name == "ShowMembersLaurels") App.Settings.GroupWindowSettings.ShowLaurels = bool.Parse(attr.Value);
                    else if (attr.Name == "ShowGroupWindowDetails") App.Settings.GroupWindowSettings.ShowDetails = bool.Parse(attr.Value);
                    else if (attr.Name == "ShowAwakenIcon") App.Settings.GroupWindowSettings.ShowAwakenIcon = bool.Parse(attr.Value);
                    else if (attr.Name == "ShowAllGroupAbnormalities") App.Settings.GroupWindowSettings.ShowAllAbnormalities = bool.Parse(attr.Value);
                    else if (attr.Name == "GroupSizeThreshold") App.Settings.GroupWindowSettings.GroupSizeThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == "ShowMembersHpNumbers") App.Settings.GroupWindowSettings.ShowHpLabels = bool.Parse(attr.Value);
                    else if (attr.Name == "HideBuffsThreshold") App.Settings.GroupWindowSettings.HideBuffsThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == "HideDebuffsThreshold") App.Settings.GroupWindowSettings.HideDebuffsThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == "DisableAbnormalitiesThreshold") App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == "HideHpThreshold") App.Settings.GroupWindowSettings.HideHpThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == "HideMpThreshold") App.Settings.GroupWindowSettings.HideMpThreshold = uint.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.AnimateChatMessages)) App.Settings.AnimateChatMessages = bool.Parse(attr.Value);
                    //else if (attr.Name == nameof(SettingsContainer.LfgEnabled)) App.Settings.LfgWindowSettings.Enabled = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.UseHotkeys)) App.Settings.UseHotkeys = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.HideHandles)) App.Settings.HideHandles = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ChatEnabled)) App.Settings.ChatEnabled = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ShowTradeLfg)) App.Settings.ShowTradeLfg = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.EthicalMode)) App.Settings.EthicalMode = bool.Parse(attr.Value);
                    //TODO: else if (attr.Name == nameof(SettingsContainer.ShowFlightEnergy)) App.Settings.ShowFlightEnergy = bool.Parse(attr.Value);
                    else if (attr.Name == "FlipFlightGauge") App.Settings.FlightGaugeWindowSettings.Flip = bool.Parse(attr.Value);
                    else if (attr.Name == "FlightGaugeRotation") App.Settings.FlightGaugeWindowSettings.Rotation = double.Parse(attr.Value, CultureInfo.InvariantCulture);
                    else if (attr.Name == "CharacterWindowCompactMode") App.Settings.CharacterWindowSettings.CompactMode = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.HighPriority)) App.Settings.HighPriority = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ForceSoftwareRendering)) App.Settings.ForceSoftwareRendering = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.Npcap)) App.Settings.Npcap = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.ExperimentalNotification)) App.Settings.ExperimentalNotification = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.LanguageOverride)) App.Settings.LanguageOverride = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.LastLanguage)) App.Settings.LastLanguage = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.LastRun)) App.Settings.LastRun = DateTime.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.TwitchName)) App.Settings.TwitchName = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.TwitchToken)) App.Settings.TwitchToken = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.StatSentVersion)) App.Settings.StatSentVersion = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.TwitchChannelName)) App.Settings.TwitchChannelName = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.DisableLfgChatMessages)) App.Settings.DisableLfgChatMessages = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.CheckGuildBamWithoutOpcode)) App.Settings.CheckGuildBamWithoutOpcode = bool.Parse(attr.Value);


                    else if (attr.Name == nameof(SettingsContainer.WebhookEnabledGuildBam)) App.Settings.WebhookEnabledGuildBam = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WebhookEnabledFieldBoss)) App.Settings.WebhookEnabledFieldBoss = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(SettingsContainer.WebhookUrlGuildBam)) App.Settings.WebhookUrlGuildBam = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookUrlFieldBoss)) App.Settings.WebhookUrlFieldBoss = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookMessageGuildBam)) App.Settings.WebhookMessageGuildBam = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookMessageFieldBossSpawn)) App.Settings.WebhookMessageFieldBossSpawn = attr.Value;
                    else if (attr.Name == nameof(SettingsContainer.WebhookMessageFieldBossDie)) App.Settings.WebhookMessageFieldBossDie = attr.Value;

                    else if (attr.Name == nameof(SettingsContainer.CheckOpcodesHash)) App.Settings.CheckOpcodesHash = bool.Parse(attr.Value);
                    else if (attr.Name == nameof(FloatingButtonWindowSettings.ShowNotificationBubble)) App.Settings.FloatingButtonSettings.ShowNotificationBubble = bool.Parse(attr.Value);
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
                    ParseGroupAbnormalSettings(_settingsDoc.Descendants().FirstOrDefault(x => x.Name == "GroupAbnormals"));
                }
                catch
                {
                    CommonDefault.ForEach(x => App.Settings.GroupWindowSettings.GroupAbnormals[Class.Common].Add(x));
                    PriestDefault.ForEach(x => App.Settings.GroupWindowSettings.GroupAbnormals[Class.Priest].Add(x));
                    MysticDefault.ForEach(x => App.Settings.GroupWindowSettings.GroupAbnormals[Class.Mystic].Add(x));
                }
                //Add My Abnormals Setting by HQ ====================================================
                try
                {
                    ParseMyAbnormalSettings(_settingsDoc.Descendants().FirstOrDefault(x => x.Name == nameof(BuffWindowSettings.MyAbnormals)));
                }
                catch
                {
                    MyCommonDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Common].Add(x));
                    MyArcherDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Archer].Add(x));
                    MyBerserkerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Berserker].Add(x));
                    MyBrawlerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Brawler].Add(x));
                    MyGunnerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Gunner].Add(x));
                    MyLancerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Lancer].Add(x));
                    MyMysticDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Mystic].Add(x));
                    MyNinjaDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Ninja].Add(x));
                    MyPriestDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Priest].Add(x));
                    MyReaperDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Reaper].Add(x));
                    MySlayerDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Slayer].Add(x));
                    MySorcererDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Sorcerer].Add(x));
                    MyValkyrieDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Valkyrie].Add(x));
                    MyWarriorDefault.ForEach(x => App.Settings.BuffWindowSettings.MyAbnormals[Class.Warrior].Add(x));
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