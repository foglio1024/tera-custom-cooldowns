using Newtonsoft.Json;
using Nostrum.Extensions;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TCC.Data;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser;
using Key = System.Windows.Forms.Keys;

namespace TCC.Settings;

public class SettingsContainer
{
    private string _lastLanguage;
    private bool _chatEnabled;
    private bool _enableProxy;
    private LootDistributionWindowSettings _lootDistributionWindowSettings;

    #region Misc

    public DateTime LastRun { get; set; }

    public string LastLanguage
    {
        get => LanguageOverride != LanguageOverride.None ? LanguageOverride.GetDescription() : _lastLanguage;
        set => _lastLanguage = value;
    }

    public Size LastScreenSize { get; set; }
    public DateTime StatSentTime { get; set; }
    public string StatSentVersion { get; set; }
    public LanguageOverride LanguageOverride { get; set; }
    public bool ShowTradeLfg { get; set; }
    public bool UseHotkeys { get; set; }
    public bool EthicalMode { get; set; }
    public bool HideHandles { get; set; }
    public bool HighPriority { get; set; }
    public bool ForceSoftwareRendering { get; set; }
    public bool Npcap { get; set; } //TODO: remove
    public bool ShowIngameChat { get; set; }
    public bool CheckOpcodesHash { get; set; }
    public bool CheckGuildBamWithoutOpcode { get; set; } //by HQ 20190324
    public bool BetaNotification { get; set; }
    public bool FpsAtGuardian { get; set; }
    public bool IntegratedGpuSleepWorkaround { get; set; }

    public bool EnableProxy
    {
        get => _enableProxy || App.ToolboxMode;
        set => _enableProxy = value;
    }

    public bool DontShowFUBH { get; set; }
    public ControlShape AbnormalityShape { get; set; }
    public ControlShape SkillShape { get; set; }
    public CaptureMode CaptureMode { get; set; }
    public MentionMode MentionMode { get; set; }

    #endregion Misc

    #region Hotkeys

    public HotKey LfgHotkey { get; set; }
    public HotKey DashboardHotkey { get; set; }
    public HotKey SettingsHotkey { get; set; }
    public HotKey SkillSettingsHotkey { get; set; }
    public HotKey ForceClickableChatHotkey { get; set; }
    public HotKey ReturnToLobbyHotkey { get; set; }
    public HotKey ToggleBoundariesHotkey { get; set; }
    public HotKey ToggleHideAllHotkey { get; set; }
    public HotKey AbnormalSettingsHotkey { get; set; }

    #endregion Hotkeys

    #region Webhooks

    public bool WebhookEnabledFieldBoss { get; set; }
    public string WebhookUrlFieldBoss { get; set; }
    public string WebhookMessageFieldBossSpawn { get; set; }
    public string WebhookMessageFieldBossDie { get; set; }
    public bool WebhookEnabledGuildBam { get; set; }
    public string WebhookUrlGuildBam { get; set; }
    public string WebhookMessageGuildBam { get; set; }
    public bool WebhookEnabledMentions { get; set; }
    public string WebhookUrlMentions { get; set; }

    #endregion Webhooks

    public CooldownWindowSettings CooldownWindowSettings { get; set; }
    public CharacterWindowSettings CharacterWindowSettings { get; set; }
    public NpcWindowSettings NpcWindowSettings { get; set; }
    public BuffWindowSettings BuffWindowSettings { get; set; }
    public ClassWindowSettings ClassWindowSettings { get; set; }
    public GroupWindowSettings GroupWindowSettings { get; set; }
    public FlightWindowSettings FlightGaugeWindowSettings { get; set; }
    public FloatingButtonWindowSettings FloatingButtonSettings { get; set; }
    public CivilUnrestWindowSettings CivilUnrestWindowSettings { get; set; }
    public LfgWindowSettings LfgWindowSettings { get; set; }
    public NotificationAreaSettings NotificationAreaSettings { get; set; }
    public PerfMonitorSettings PerfMonitorSettings { get; set; }

    public LootDistributionWindowSettings LootDistributionWindowSettings
    {
        get => _lootDistributionWindowSettings;
        set
        {
            if (value == null) return;
            _lootDistributionWindowSettings = value;
        }
    }

    #region Chat

    public ThreadSafeObservableCollection<ChatWindowSettings> ChatWindowsSettings { get; }
    public WindowSettingsBase ChatSettings { get; private set; } // added to have the EnabledChanged event

    public bool ChatEnabled
    {
        get => ChatWindowsSettings.Count > 0 ? ChatWindowsSettings[0].Enabled : _chatEnabled;
        set
        {
            ChatSettings.Enabled = value;
            if (ChatWindowsSettings.Count > 0)
            {
                if (ChatWindowsSettings[0].Enabled == value) return;
                foreach (var x in ChatWindowsSettings) x.Enabled = value;
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
    public bool ChatTimestampSeconds { get; set; }
    public bool AnimateChatMessages { get; set; }
    public int MaxMessages { get; set; }
    public int SpamThreshold { get; set; }
    public int FontSize { get; set; }
    public int ChatScrollAmount { get; set; }
    public List<string> UserExcludedSysMsg { get; set; }
    public TranslationMode TranslationMode { get; set; }

    #endregion Chat

    #region Twitch

    [JsonIgnore]
    public string TwitchName { get; set; } //TODO: re-add this

    [JsonIgnore]
    public string TwitchToken { get; set; } //TODO: re-add this

    [JsonIgnore]
    public string TwitchChannelName { get; set; } //TODO: re-add this

    #endregion Twitch

    public static string SettingsOverride { get; set; } = "";
    public string LastAccountNameHash { get; set; } = "";
    public bool BackgroundNotifications { get; set; }
    public bool EnablePlayerMenu { get; set; }
    public CooldownDecimalMode CooldownsDecimalMode { get; set; }

    public AbnormalitySettingsContainer AbnormalitySettings { get; set; }

    public SettingsContainer()
    {
        StatSentVersion = App.AppVersion;
        _lastLanguage = "";
        StatSentTime = DateTime.MinValue;
        LastScreenSize = FocusManager.TeraScreen.Bounds.Size;

        AbnormalitySettings = new AbnormalitySettingsContainer();
        AbnormalitySettings.Self.Whitelist[Class.Warrior] = [100800, 100801];
        AbnormalitySettings.Self.Whitelist[Class.Lancer] = [200230, 200231, 200232, 201701];
        AbnormalitySettings.Self.Whitelist[Class.Slayer] = [300800, 300801, 300805];
        AbnormalitySettings.Self.Whitelist[Class.Berserker] = [401705, 401706, 401710, 400500, 400501, 400508, 400710, 400711];
        AbnormalitySettings.Self.Whitelist[Class.Sorcerer] = [21170, 22120, 23180, 26250, 29011, 25170, 25171, 25201, 25202, 500100, 500150, 501600, 501650, 502001, 502051, 502070, 502071, 502072];
        AbnormalitySettings.Self.Whitelist[Class.Archer] = [601400, 601450, 601460, 88608101, 88608102, 88608103, 88608104, 88608105, 88608106, 88608107, 88608108, 88608109, 88608110, 602101, 602102, 602103, 601611];
        AbnormalitySettings.Self.Whitelist[Class.Reaper] = [10151010, 10151131, 10151192];
        AbnormalitySettings.Self.Whitelist[Class.Gunner] = [89105101, 89105102, 89105103, 89105104, 89105105, 89105106, 89105107, 89105108, 89105109, 89105110, 89105111, 89105112, 89105113, 89105114, 89105115, 89105116, 89105117, 89105118, 89105119, 89105120, 10152340, 10152351];
        AbnormalitySettings.Self.Whitelist[Class.Brawler] = [31020, 10153210];
        AbnormalitySettings.Self.Whitelist[Class.Ninja] = [89314201, 89314202, 89314203, 89314204, 89314205, 89314206, 89314207, 89314208, 89314209, 89314210, 89314211, 89314212, 89314213, 89314214, 89314215, 89314216, 89314217, 89314218, 89314219, 89314220, 10154480, 10154450];
        AbnormalitySettings.Self.Whitelist[Class.Valkyrie] = [10155130, 10155551, 10155510, 10155512, 10155540, 10155541, 10155542];
        AbnormalitySettings.Self.Whitelist[Class.Common] = [6001, 6002, 6003, 6004, 6012, 6013, 702004, 805800, 805803, 200700, 200701, 200731, 800300, 800301, 800302, 800303, 800304, 702001];

        CooldownWindowSettings = new CooldownWindowSettings();
        CharacterWindowSettings = new CharacterWindowSettings();
        NpcWindowSettings = new NpcWindowSettings();
        BuffWindowSettings = new BuffWindowSettings();
        ClassWindowSettings = new ClassWindowSettings();
        GroupWindowSettings = new GroupWindowSettings();
        FlightGaugeWindowSettings = new FlightWindowSettings();
        FloatingButtonSettings = new FloatingButtonWindowSettings();
        CivilUnrestWindowSettings = new CivilUnrestWindowSettings();
        ChatWindowsSettings = new ThreadSafeObservableCollection<ChatWindowSettings>(App.BaseDispatcher);
        PerfMonitorSettings = new PerfMonitorSettings();
        ChatSettings = new WindowSettingsBase();
        LfgWindowSettings = new LfgWindowSettings();
        NotificationAreaSettings = new NotificationAreaSettings();
        _lootDistributionWindowSettings = new LootDistributionWindowSettings();

        MaxMessages = 500;
        SpamThreshold = 2;
        ShowChannel = true;
        ShowTimestamp = true;
        FontSize = 15;
        ChatScrollAmount = 1;
        LastRun = DateTime.MinValue;
        TwitchName = "";
        TwitchToken = "";
        TwitchChannelName = "";
        ShowTradeLfg = true;
        UseHotkeys = true;
        EthicalMode = false;
        LfgHotkey = new HotKey(Key.Y, ModifierKeys.Control);
        DashboardHotkey = new HotKey(Key.I, ModifierKeys.Control);
        SettingsHotkey = new HotKey(Key.O, ModifierKeys.Control);
        SkillSettingsHotkey = new HotKey(Key.K, ModifierKeys.Control);
        AbnormalSettingsHotkey = new HotKey(Key.B, ModifierKeys.Control);
        ReturnToLobbyHotkey = new HotKey(Key.R, ModifierKeys.Control | ModifierKeys.Alt);
        ForceClickableChatHotkey = new HotKey(Key.C, ModifierKeys.Control | ModifierKeys.Alt);
        ToggleBoundariesHotkey = new HotKey(Key.H, ModifierKeys.Control | ModifierKeys.Alt);
        ToggleHideAllHotkey = new HotKey(Key.Z, ModifierKeys.Control | ModifierKeys.Alt);
        LanguageOverride = LanguageOverride.None;
        HideHandles = false;
        HighPriority = false;
        ForceSoftwareRendering = true;
        AbnormalityShape = ControlShape.Round;
        SkillShape = ControlShape.Round;
        MentionMode = MentionMode.All;
        Npcap = true;
        CheckOpcodesHash = true;
        CheckGuildBamWithoutOpcode = false;
        UserExcludedSysMsg = new List<string>();
        BetaNotification = true;
        FpsAtGuardian = true;
        EnableProxy = true;
        DisableLfgChatMessages = true;
        WebhookEnabledFieldBoss = false;
        WebhookUrlFieldBoss = "";
        WebhookMessageFieldBossSpawn = "@here {bossName} spawned in {regionName}!";
        WebhookMessageFieldBossDie = "{bossName} is dead.";
        WebhookEnabledGuildBam = false;
        WebhookUrlGuildBam = "";
        WebhookMessageGuildBam = "@here Guild BAM will spawn soon!";
        WebhookEnabledMentions = false;
        WebhookUrlMentions = "";
        IntegratedGpuSleepWorkaround = false;
        CooldownsDecimalMode = CooldownDecimalMode.LessThanTen;
        TranslationMode = TranslationMode.MergedTranslationFirst;
    }

    public static SettingsContainer Load()
    {
        var settingsPath = SettingsOverride == ""
            ? Path.Combine(App.BasePath, SettingsGlobals.SettingsFileName)
            : SettingsOverride;
        var settings = new JsonSettingsReader().LoadSettings(settingsPath);
        return settings;
    }

    public void Save()
    {
        var toRemove = ChatWindowsSettings.Where(s => s.Tabs.Count == 0).ToList();
        foreach (var s in toRemove) ChatWindowsSettings.Remove(s);
        App.BaseDispatcher.InvokeAsync(() => new JsonSettingsWriter().Save());
    }
}

public class AbnormalitySettingsContainer
{
    public List<uint> Favorites { get; } = [];

    public AbnormalitySettings Self { get; } = new();
    public AbnormalitySettings Group { get; } = new();

    public bool IsFavorite(uint id)
    {
        return Favorites.Contains(id);
    }
}

public class AbnormalitySettings
{
    public List<uint> Collapsible { get; } = [];
    public bool ShowAll { get; set; }
    public Dictionary<Class, List<uint>> Whitelist { get; }

    public AbnormalitySettings()
    {
        Whitelist = Enum.GetValues<Class>()
            .Select(c => (c, new List<uint>()))
            .ToDictionary();
    }

    public bool CanCollapse(uint id)
    {
        return Collapsible.Contains(id);
    }

    public bool CanShow(uint id)
    {
        if (ShowAll) return true;

        if (Whitelist.TryGetValue(Class.Common, out var commonList))
        {
            if (commonList.Contains(id))
                return true;

            if (Whitelist.TryGetValue(Game.Me.Class, out var classList))
            {
                if (!classList.Contains(id))
                    return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}