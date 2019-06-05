using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using TCC.Data;
using TeraDataLite;
using Key = System.Windows.Forms.Keys;

namespace TCC.Settings
{
    public class SettingsContainer
    {
        private string _lastLanguage;
        private bool _chatEnabled;

        #region Misc
        // Misc
        public DateTime LastRun { get; set; }
        public string LastLanguage
        {
            get => LanguageOverride != "" ? LanguageOverride : _lastLanguage;
            set => _lastLanguage = value;
        }
        public Size LastScreenSize { get; set; }
        public DateTime StatSentTime { get; set; }
        public string StatSentVersion { get; set; }
        public string LanguageOverride { get; set; }
        public bool LfgEnabled { get; set; }
        public bool ShowTradeLfg { get; set; }
        public bool UseHotkeys { get; set; }
        public bool EthicalMode { get; set; }
        public bool HideHandles { get; set; }
        public bool HighPriority { get; set; }
        public bool ForceSoftwareRendering { get; set; }
        public bool Npcap { get; set; } //TODO: remove
        public bool CheckOpcodesHash { get; set; }
        public bool CheckGuildBamWithoutOpcode { get; set; } //by HQ 20190324
        public bool ExperimentalNotification { get; set; }
        public bool FpsAtGuardian { get; set; }
        public bool EnableProxy { get; set; }
        public bool DontShowFUBH { get; set; }
        public ControlShape AbnormalityShape { get; set; }
        public ControlShape SkillShape { get; set; }
        public CaptureMode CaptureMode { get; set; }

        #endregion

        #region Hotkeys
        public HotKey LfgHotkey { get; }
        public HotKey InfoWindowHotkey { get; }
        public HotKey SettingsHotkey { get; }
        public HotKey LootSettingsHotkey { get; } 
        #endregion

        #region Webhooks
        public bool WebhookEnabledFieldBoss { get; set; }
        public string WebhookUrlFieldBoss { get; set; }
        public string WebhookMessageFieldBossSpawn { get; set; }
        public string WebhookMessageFieldBossDie { get; set; }
        public bool WebhookEnabledGuildBam { get; set; }
        public string WebhookUrlGuildBam { get; set; }
        public string WebhookMessageGuildBam { get; set; }
        #endregion

        #region Cooldown
        // Cooldown window
        public WindowSettings CooldownWindowSettings { get; set; }
        public bool ShowItemsCooldown { get; set; }
        public CooldownBarMode CooldownBarMode { get; set; }
        #endregion

        #region Character
        // Character window
        public WindowSettings CharacterWindowSettings { get; set; }
        public bool CharacterWindowCompactMode { get; set; }
        #endregion

        #region Npc
        // Boss window
        public WindowSettings BossWindowSettings { get; set; }
        public bool ShowOnlyBosses { get; set; }
        public bool AccurateHp { get; set; }
        public EnrageLabelMode EnrageLabelMode { get; set; }
        #endregion

        #region Buffs
        // Buff window
        public WindowSettings BuffWindowSettings { get; set; }
        public bool ShowAllMyAbnormalities { get; set; } // by HQ
        public FlowDirection BuffsDirection { get; set; }
        public Dictionary<Class, List<uint>> MyAbnormals { get; } // by HQ
        #endregion

        #region Class
        // Class window
        public WindowSettings ClassWindowSettings { get; set; }
        public bool WarriorShowTraverseCut { get; set; }
        public bool WarriorShowEdge { get; set; }
        public WarriorEdgeMode WarriorEdgeMode { get; set; }
        public bool SorcererReplacesElementsInCharWindow { get; set; }
        #endregion

        #region Group
        // Group window
        public WindowSettings GroupWindowSettings { get; set; }
        public GroupWindowLayout GroupWindowLayout { get; set; }
        public bool IgnoreMeInGroupWindow { get; set; }
        public bool IgnoreGroupBuffs { get; set; }
        public bool IgnoreGroupDebuffs { get; set; }
        public bool DisablePartyMP { get; set; }
        public bool DisablePartyHP { get; set; }
        public bool DisablePartyAbnormals { get; set; }
        public bool ShowOnlyAggroStacks { get; set; }
        public bool ShowMembersHpNumbers { get; set; }
        public bool ShowMembersLaurels { get; set; }
        public bool ShowAllGroupAbnormalities { get; set; }
        public bool ShowGroupWindowDetails { get; set; }
        public bool ShowAwakenIcon { get; set; }
        public uint GroupSizeThreshold { get; set; }
        public uint HideBuffsThreshold { get; set; }
        public uint HideDebuffsThreshold { get; set; }
        public uint DisableAbnormalitiesThreshold { get; set; }
        public uint HideHpThreshold { get; set; }
        public uint HideMpThreshold { get; set; }
        public Dictionary<Class, List<uint>> GroupAbnormals { get; }
        #endregion

        #region Flight
        // Flight gauge
        public WindowSettings FlightGaugeWindowSettings { get; set; }
        public bool ShowFlightEnergy { get; set; } // TODO: replace with WindowSettings.Enabled?
        public bool FlipFlightGauge { get; set; }
        public double FlightGaugeRotation { get; set; }
        #endregion

        #region Button
        public WindowSettings FloatingButtonSettings { get; set; }
        public bool ShowNotificationBubble { get; set; }
        #endregion

        #region CU
        public WindowSettings CivilUnrestWindowSettings { get; set; }
        #endregion

        #region Chat
        // Chat window
        public SynchronizedObservableCollection<ChatWindowSettings> ChatWindowsSettings { get; }
        public bool ChatEnabled
        {
            get => ChatWindowsSettings.Count > 0 ? ChatWindowsSettings[0].Enabled : _chatEnabled;
            set
            {
                if (ChatWindowsSettings.Count > 0)
                {
                    if (ChatWindowsSettings[0].Enabled == value) return;
                    ChatWindowsSettings.ToList().ForEach(x => x.Enabled = value);
                }
                else
                {
                    _chatEnabled = value;
                }
            }
        }
        public bool DisableLfgChatMessages { get; set; }
        public bool ShowChannel { get; set; }
        public bool ShowTimestamp { get; set; }
        public bool ChatTimestampSeconds { get; internal set; }
        public bool AnimateChatMessages { get; set; }
        public int MaxMessages { get; set; }
        public int SpamThreshold { get; set; }
        public int FontSize { get; set; }
        public int ChatScrollAmount { get; set; }
        public List<string> UserExcludedSysMsg { get; set; }
        #endregion

        #region Twitch
        [JsonIgnore]
        public string TwitchName { get; set; } //TODO: re-add this
        [JsonIgnore]
        public string TwitchToken { get; set; } //TODO: re-add this
        [JsonIgnore]
        public string TwitchChannelName { get; set; } //TODO: re-add this 
        #endregion

        public SettingsContainer()
        {
            StatSentVersion = App.AppVersion;
            _lastLanguage = "";
            StatSentTime = DateTime.MinValue;
            LastScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            GroupWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(GroupWindowSettings));
            CooldownWindowSettings = new WindowSettings(.4, .7, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(CooldownWindowSettings));
            BossWindowSettings = new WindowSettings(.4, 0, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(BossWindowSettings));
            BuffWindowSettings = new WindowSettings(1, .7, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(BuffWindowSettings));
            CharacterWindowSettings = new WindowSettings(.4, 1, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(CharacterWindowSettings));
            ClassWindowSettings = new WindowSettings(.25, .6, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(ClassWindowSettings));
            FlightGaugeWindowSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Always, 1, false, 1, false, true, false);
            FloatingButtonSettings = new WindowSettings(0, 0, 0, 0, true, ClickThruMode.Never, 1, false, 1, false, true, true);
            CivilUnrestWindowSettings = new WindowSettings(1, .45, 0, 0, true, ClickThruMode.Never, 1, true, .5, false, true, false, null, nameof(CivilUnrestWindowSettings));
            ChatWindowsSettings = new SynchronizedObservableCollection<ChatWindowSettings>(App.BaseDispatcher);
            ShowOnlyAggroStacks = true;
            GroupSizeThreshold = 7;
            HideBuffsThreshold = 7;
            HideDebuffsThreshold = 7;
            DisableAbnormalitiesThreshold = 7;
            HideHpThreshold = 7;
            HideMpThreshold = 7;
            ShowGroupWindowDetails = true;
            ShowAwakenIcon = true;
            GroupAbnormals = new Dictionary<Class, List<uint>>()
            {
                {       0, new List<uint>()},
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
            BuffsDirection = FlowDirection.RightToLeft;
            ShowAllMyAbnormalities = true;
            MyAbnormals = new Dictionary<Class, List<uint>>()
            {
                {       0, new List<uint>{ 100800, 100801 }},
                {(Class)1, new List<uint>{ 200230, 200231, 200232, 201701 }},
                {(Class)2, new List<uint>{ 300800, 300801, 300805 }},
                {(Class)3, new List<uint>{ 401705, 401706, 401710, 400500, 400501, 400508, 400710, 400711 }},
                {(Class)4, new List<uint>{ 21170, 22120, 23180, 26250, 29011, 25170, 25171, 25201, 25202, 500100, 500150, 501600, 501650, 502001, 502051, 502070, 502071, 502072 }},
                {(Class)5, new List<uint>{ 601400, 601450, 601460, 88608101, 88608102, 88608103, 88608104, 88608105, 88608106, 88608107, 88608108, 88608109, 88608110,602101,602102,602103,601611 }},
                {(Class)6, new List<uint>()},
                {(Class)7, new List<uint>()},
                {(Class)8, new List<uint>{ 10151010, 10151131, 10151192 }},
                {(Class)9, new List<uint>{ 89105101, 89105102, 89105103, 89105104, 89105105, 89105106, 89105107, 89105108, 89105109, 89105110, 89105111, 89105112, 89105113, 89105114, 89105115, 89105116, 89105117, 89105118, 89105119, 89105120, 10152340, 10152351 }},
                {(Class)10, new List<uint>{ 31020, 10153210 }},
                {(Class)11, new List<uint>{ 89314201, 89314202, 89314203, 89314204, 89314205, 89314206, 89314207, 89314208, 89314209, 89314210, 89314211, 89314212, 89314213, 89314214, 89314215, 89314216, 89314217, 89314218, 89314219, 89314220, 10154480, 10154450 }},
                {(Class)12, new List<uint>{ 10155130, 10155551, 10155510, 10155512, 10155540, 10155541, 10155542 }},
                {(Class)255, new List<uint>{ 6001, 6002, 6003, 6004, 6012, 6013, 702004, 805800, 805803, 200700, 200701, 200731, 800300, 800301, 800302, 800303, 800304, 702001 }},
            };
            CooldownBarMode = CooldownBarMode.Fixed;
            ShowItemsCooldown = true;
            EnrageLabelMode = EnrageLabelMode.Remaining;
            AccurateHp = true;
            MaxMessages = 500;
            SpamThreshold = 2;
            ShowChannel = true;
            ShowTimestamp = true;
            FontSize = 15;
            ChatScrollAmount = 1;
            CharacterWindowCompactMode = true;
            WarriorShowTraverseCut = true;
            WarriorShowEdge = true;
            WarriorEdgeMode = WarriorEdgeMode.Bar;
            SorcererReplacesElementsInCharWindow = true;
            LastRun = DateTime.MinValue;
            TwitchName = "";
            TwitchToken = "";
            TwitchChannelName = "";
            LfgEnabled = true;
            ShowTradeLfg = true;
            ShowFlightEnergy = true;
            UseHotkeys = true;
            EthicalMode = false;
            LfgHotkey = new HotKey(Key.Y, ModifierKeys.Control);
            InfoWindowHotkey = new HotKey(Key.I, ModifierKeys.Control);
            SettingsHotkey = new HotKey(Key.O, ModifierKeys.Control);
            LootSettingsHotkey = new HotKey(Key.L, ModifierKeys.Control);
            LanguageOverride = "";
            FlightGaugeRotation = 0;
            FlipFlightGauge = false;
            HideHandles = false;
            HighPriority = false;
            ForceSoftwareRendering = true;
            AbnormalityShape = ControlShape.Round;
            SkillShape = ControlShape.Round;
            Npcap = true;
            CheckOpcodesHash = true;
            CheckGuildBamWithoutOpcode = false;
            ShowNotificationBubble = true;
            UserExcludedSysMsg = new List<string>();
            ExperimentalNotification = true;
            FpsAtGuardian = true;
            EnableProxy = true;
            ShowMembersHpNumbers = true;
            DisableLfgChatMessages = true;
            WebhookEnabledFieldBoss = false;
            WebhookUrlFieldBoss = "";
            WebhookMessageFieldBossSpawn = "@here {bossName} spawned in {regionName}!";
            WebhookMessageFieldBossDie = "{bossName} is dead.";
            WebhookEnabledGuildBam = false;
            WebhookUrlGuildBam = "";
            WebhookMessageGuildBam = "@here Guild BAM will spawn soon!";
            GroupWindowLayout = GroupWindowLayout.RoleSeparated;
        }

        public static void Load()
        {
            if (File.Exists(Path.Combine(App.BasePath, SettingsGlobals.JsonFileName)))
                new JsonSettingsReader().LoadSettings();
            else if (File.Exists(Path.Combine(App.BasePath, SettingsGlobals.XmlFileName)))
                new XmlSettingsReader().LoadSettings();
            else App.Settings = new SettingsContainer();
        }
        public void Save()
        {
            new JsonSettingsWriter().Save();
        }

    }
}
