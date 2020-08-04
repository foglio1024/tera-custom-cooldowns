using Nostrum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Analysis;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Update;
using TCC.Utilities;
using TCC.Utils;
using TeraPacketParser;
using CaptureMode = TCC.Data.CaptureMode;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        public static event Action ChatShowChannelChanged = null!;
        public static event Action ChatShowTimestampChanged = null!;
        public static event Action AbnormalityShapeChanged = null!;
        public static event Action SkillShapeChanged = null!;
        public static event Action FontSizeChanged = null!;

        public bool Beta => App.Beta;
        public bool ToolboxMode => App.ToolboxMode;

        public CooldownWindowSettings CooldownWindowSettings => App.Settings.CooldownWindowSettings;
        public ClassWindowSettings ClassWindowSettings => App.Settings.ClassWindowSettings;
        public GroupWindowSettings GroupWindowSettings => App.Settings.GroupWindowSettings;
        public BuffWindowSettings BuffWindowSettings => App.Settings.BuffWindowSettings;
        public CharacterWindowSettings CharacterWindowSettings => App.Settings.CharacterWindowSettings;
        public NpcWindowSettings NpcWindowSettings => App.Settings.NpcWindowSettings;
        public FlightWindowSettings FlightWindowSettings => App.Settings.FlightGaugeWindowSettings;
        public FloatingButtonWindowSettings FloatingButtonSettings => App.Settings.FloatingButtonSettings;
        public CivilUnrestWindowSettings CuWindowSettings => App.Settings.CivilUnrestWindowSettings;
        public LfgWindowSettings LfgWindowSettings => App.Settings.LfgWindowSettings;
        public NotificationAreaSettings NotificationAreaSettings => App.Settings.NotificationAreaSettings;

        public ICommand BrowseUrlCommand { get; }
        public ICommand RegisterWebhookCommand { get; }
        public ICommand OpenWindowCommand { get; }
        public ICommand DownloadBetaCommand { get; }
        public ICommand ResetChatPositionsCommand { get; }
        public ICommand MakePositionsGlobalCommand { get; }
        public ICommand ResetWindowPositionsCommand { get; }
        public ICommand OpenResourcesFolderCommand { get; }
        public ICommand OpenWelcomeWindowCommand { get; }
        public ICommand ClearChatCommand { get; }

        public bool EthicalMode
        {
            get => App.Settings.EthicalMode;
            set
            {
                if (App.Settings.EthicalMode == value) return;
                App.Settings.EthicalMode = value;
                N();
            }
        }
        public bool UseHotkeys
        {
            get => App.Settings.UseHotkeys;
            set
            {
                if (App.Settings.UseHotkeys == value) return;
                App.Settings.UseHotkeys = value;
                if (value) KeyboardHook.Instance.Enable();
                else KeyboardHook.Instance.Disable();
                N(nameof(UseHotkeys));
            }
        }

        public HotKey SettingsHotkey
        {
            get => App.Settings.SettingsHotkey;
            set
            {
                if (App.Settings.SettingsHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.SettingsHotkey, value);
                App.Settings.SettingsHotkey = value;
                N();
            }
        }
        public HotKey SkillSettingsHotkey
        {
            get => App.Settings.SkillSettingsHotkey;
            set
            {
                if (App.Settings.SkillSettingsHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.SkillSettingsHotkey, value);
                App.Settings.SkillSettingsHotkey = value;
                N();
            }
        }
        public HotKey AbnormalSettingsHotkey
        {
            get => App.Settings.AbnormalSettingsHotkey;
            set
            {
                if (App.Settings.AbnormalSettingsHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.AbnormalSettingsHotkey, value);
                App.Settings.AbnormalSettingsHotkey = value;
                N();
            }
        }
        public HotKey ForceClickableChatHotkey
        {
            get => App.Settings.ForceClickableChatHotkey;
            set
            {
                if (App.Settings.ForceClickableChatHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.ForceClickableChatHotkey, value);
                App.Settings.ForceClickableChatHotkey = value;
                N();
            }
        }
        public HotKey DashboardHotkey
        {
            get => App.Settings.DashboardHotkey;
            set
            {
                if (App.Settings.DashboardHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.DashboardHotkey, value);
                App.Settings.DashboardHotkey = value;
                N();
            }
        }
        public HotKey LfgHotkey
        {
            get => App.Settings.LfgHotkey;
            set
            {
                if (App.Settings.LfgHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.LfgHotkey, value);
                App.Settings.LfgHotkey = value;
                N();
            }
        }
        public HotKey ReturnToLobbyHotkey
        {
            get => App.Settings.ReturnToLobbyHotkey;
            set
            {
                if (App.Settings.ReturnToLobbyHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.ReturnToLobbyHotkey, value);
                App.Settings.ReturnToLobbyHotkey = value;
                N();
            }
        }
        public HotKey ToggleBoundariesHotkey
        {
            get => App.Settings.ToggleBoundariesHotkey;
            set
            {
                if (App.Settings.ToggleBoundariesHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.ToggleBoundariesHotkey, value);
                App.Settings.ToggleBoundariesHotkey = value;
                N();
            }
        }
        public HotKey ToggleHideAllHotkey
        {
            get => App.Settings.ToggleHideAllHotkey;
            set
            {
                if (App.Settings.ToggleHideAllHotkey.Equals(value)) return;
                KeyboardHook.Instance.ChangeHotkey(App.Settings.ToggleHideAllHotkey, value);
                App.Settings.ToggleHideAllHotkey = value;
                N();
            }
        }

        private int _khCount;
        private bool _kh;
        public bool KylosHelper
        {
            get => _kh;
            set
            {
                _kh = true;
                switch (_khCount)
                {
                    case 0:
                        Log.N("Exploit alert", "Are you sure you want to enable this?", NotificationType.Warning);
                        break;
                    case 1:
                        Log.N(":thinking:", "You shouldn't use this °L° Are you really sure?", NotificationType.Warning, 3000);
                        break;
                    case 2:
                        Log.N("omegalul", "There's actually no Kylos helper lol. Just memeing. Have fun o/", NotificationType.Warning, 6000);
                        TccUtils.OpenUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
                        break;
                }
                N();

                _khCount++;
                if (_khCount > 2) _khCount = 0;
                _kh = false;
                N();
            }
        }
        public bool DisableLfgChatMessages
        {
            get => App.Settings.DisableLfgChatMessages;
            set
            {
                if (App.Settings.DisableLfgChatMessages == value) return;
                App.Settings.DisableLfgChatMessages = value;
                N();
            }
        }
        public bool BetaNotification
        {
            get => App.Settings.BetaNotification;
            set
            {
                if (App.Settings.BetaNotification == value) return;
                App.Settings.BetaNotification = value;
                N();
            }
        }
        public bool CheckOpcodesHash
        {
            get => App.Settings.CheckOpcodesHash;
            set
            {
                if (App.Settings.CheckOpcodesHash == value) return;
                App.Settings.CheckOpcodesHash = value;
                N();
            }
        }
        public bool CheckGuildBamWithoutOpcode  // by HQ 20190324
        {
            get => App.Settings.CheckGuildBamWithoutOpcode;
            set
            {
                if (App.Settings.CheckGuildBamWithoutOpcode == value) return;
                App.Settings.CheckGuildBamWithoutOpcode = value;
                N();
            }
        }


        public ControlShape AbnormalityShape
        {
            get => App.Settings.AbnormalityShape;
            set
            {
                if (App.Settings.AbnormalityShape == value) return;
                App.Settings.AbnormalityShape = value;
                AbnormalityShapeChanged?.Invoke();
                N();
            }
        }
        public ControlShape SkillShape
        {
            get => App.Settings.SkillShape;
            set
            {
                if (App.Settings.SkillShape == value) return;
                App.Settings.SkillShape = value;
                SkillShapeChanged?.Invoke();
                N();
            }
        }


        //public bool ChatFadeOut
        //{
        //    get => Settings.Settings.ChatFadeOut;
        //    set
        //    {
        //        if (Settings.Settings.ChatFadeOut == value) return;
        //        Settings.Settings.ChatFadeOut = value;
        //        if (value) ChatWindowManager.Instance.ForceHideTimerRefresh();
        //        NPC(nameof(ChatFadeOut));
        //    }
        //}
        public LanguageOverride RegionOverride
        {
            get => App.Settings.LanguageOverride;
            set
            {
                if (App.Settings.LanguageOverride == value) return;
                App.Settings.LanguageOverride = value;
                if (value == LanguageOverride.None) App.Settings.LastLanguage = "EU-EN";
                N(nameof(RegionOverride));
            }
        }
        public int MaxMessages
        {
            get => App.Settings.MaxMessages;
            set
            {
                if (App.Settings.MaxMessages == value) return;
                App.Settings.MaxMessages = value == 0 ? int.MaxValue : value;
                N();
            }
        }
        public int ChatScrollAmount
        {
            get => App.Settings.ChatScrollAmount;
            set
            {
                if (App.Settings.ChatScrollAmount == value) return;
                App.Settings.ChatScrollAmount = value;
                N();
            }
        }
        public int SpamThreshold
        {
            get => App.Settings.SpamThreshold;
            set
            {
                if (App.Settings.SpamThreshold == value) return;
                App.Settings.SpamThreshold = value;
                N();
            }
        }
        public bool ShowTimestamp
        {
            get => App.Settings.ShowTimestamp;
            set
            {
                if (App.Settings.ShowTimestamp == value) return;
                App.Settings.ShowTimestamp = value;
                N(nameof(ShowTimestamp));
                ChatShowTimestampChanged?.Invoke();
            }

        }
        public bool ChatTimestampSeconds
        {
            get => App.Settings.ChatTimestampSeconds;
            set
            {
                if (App.Settings.ChatTimestampSeconds == value) return;
                App.Settings.ChatTimestampSeconds = value;
                N();
            }

        }
        public bool ShowChannel
        {
            get => App.Settings.ShowChannel;
            set
            {
                if (App.Settings.ShowChannel == value) return;
                App.Settings.ShowChannel = value;
                ChatShowChannelChanged?.Invoke();
                N(nameof(ShowChannel));
            }

        }


        public bool FpsAtGuardian
        {
            get => App.Settings.FpsAtGuardian;
            set
            {
                if (App.Settings.FpsAtGuardian == value) return;
                App.Settings.FpsAtGuardian = value;
                N();
            }
        }
        public bool EnableProxy
        {
            get => App.Settings.EnableProxy;
            set
            {
                if (App.Settings.EnableProxy == value) return;
                App.Settings.EnableProxy = value;
                N();
                N(nameof(ClickThruModes));
                StubInterface.Instance.StubClient.UpdateSetting("EnableProxy", App.Settings.ChatEnabled);

            }
        }
        public bool HideHandles
        {
            get => App.Settings.HideHandles;
            set
            {
                if (App.Settings.HideHandles == value) return;
                App.Settings.HideHandles = value;
                N();
            }
        }
        public bool ShowDecimalsInCooldowns
        {
            get => App.Settings.ShowDecimalsInCooldowns;
            set
            {
                if (App.Settings.ShowDecimalsInCooldowns == value) return;
                App.Settings.ShowDecimalsInCooldowns = value;
                N();
            }
        }
        public bool AnimateChatMessages
        {
            get => App.Settings.AnimateChatMessages;
            set
            {
                if (App.Settings.AnimateChatMessages == value) return;
                App.Settings.AnimateChatMessages = value;
                N(nameof(AnimateChatMessages));
            }
        }
        public bool BackgroundNotifications
        {
            get => App.Settings.BackgroundNotifications;
            set
            {
                if (App.Settings.BackgroundNotifications == value) return;
                App.Settings.BackgroundNotifications = value;
                N();
            }
        }

        public bool WebhookEnabledGuildBam
        {
            get => App.Settings.WebhookEnabledGuildBam;
            set
            {
                if (App.Settings.WebhookEnabledGuildBam == value) return;
                App.Settings.WebhookEnabledGuildBam = value;
                N();
            }
        }
        public bool WebhookEnabledFieldBoss
        {
            get => App.Settings.WebhookEnabledFieldBoss;
            set
            {
                if (App.Settings.WebhookEnabledFieldBoss == value) return;
                App.Settings.WebhookEnabledFieldBoss = value;
                N();
            }
        }
        public bool WebhookEnabledMentions
        {
            get => App.Settings.WebhookEnabledMentions;
            set
            {
                if (App.Settings.WebhookEnabledMentions == value) return;
                App.Settings.WebhookEnabledMentions = value;
                N();
            }
        }
        public string WebhookUrlGuildBam
        {
            get => App.Settings.WebhookUrlGuildBam;
            set
            {
                if (value == App.Settings.WebhookUrlGuildBam) return;
                App.Settings.WebhookUrlGuildBam = value;
                N();
            }
        }
        public string WebhookUrlFieldBoss
        {
            get => App.Settings.WebhookUrlFieldBoss;
            set
            {
                if (value == App.Settings.WebhookUrlFieldBoss) return;
                App.Settings.WebhookUrlFieldBoss = value;
                N();
            }
        }
        public string WebhookUrlMentions
        {
            get => App.Settings.WebhookUrlMentions;
            set
            {
                if (value == App.Settings.WebhookUrlMentions) return;
                App.Settings.WebhookUrlMentions = value;
                N();
            }
        }
        public string WebhookMessageGuildBam
        {
            get => App.Settings.WebhookMessageGuildBam;
            set
            {
                if (value == App.Settings.WebhookMessageGuildBam) return;
                App.Settings.WebhookMessageGuildBam = value;
                N();
            }
        }
        public string WebhookMessageFieldBossSpawn
        {
            get => App.Settings.WebhookMessageFieldBossSpawn;
            set
            {
                if (value == App.Settings.WebhookMessageFieldBossSpawn) return;
                App.Settings.WebhookMessageFieldBossSpawn = value;
                N();
            }
        }
        public string WebhookMessageFieldBossDie
        {
            get => App.Settings.WebhookMessageFieldBossDie;
            set
            {
                if (value == App.Settings.WebhookMessageFieldBossDie) return;
                App.Settings.WebhookMessageFieldBossDie = value;
                N();
            }
        }

        public string TwitchUsername
        {
            get => App.Settings.TwitchName;
            set
            {
                if (value == App.Settings.TwitchName) return;
                App.Settings.TwitchName = value;
                N(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => App.Settings.TwitchToken;
            set
            {
                if (value == App.Settings.TwitchToken) return;
                App.Settings.TwitchToken = value;
                N(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => App.Settings.TwitchChannelName;
            set
            {
                if (value == App.Settings.TwitchChannelName) return;
                App.Settings.TwitchChannelName = value;
                N(nameof(TwitchChannelName));
            }
        }

        public CaptureMode CaptureMode
        {
            get => App.Settings.CaptureMode;
            set
            {
                if (App.Settings.CaptureMode == value) return;
                var res = TccMessageBox.Show("TCC", SR.RestartToApplySetting, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Cancel) return;
                App.Settings.CaptureMode = value;
                N();
                if (res == MessageBoxResult.OK) App.Restart();
            }
        }
        public MentionMode MentionMode
        {
            get => App.Settings.MentionMode;
            set
            {
                if (App.Settings.MentionMode == value) return;
                App.Settings.MentionMode = value;
                N();
            }
        }
        public int FontSize
        {
            get => App.Settings.FontSize;
            set
            {
                if (App.Settings.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                App.Settings.FontSize = val;
                FontSizeChanged?.Invoke();
                N(nameof(FontSize));
            }
        }
        public bool ChatWindowEnabled
        {
            get => App.Settings.ChatEnabled;
            set
            {
                if (App.Settings.ChatEnabled == value) return;
                //if (!value)
                //{
                //    TccMessageBox.Show("Warning",
                //        "Disabling this while still having modded Chat2.gpk installed may cause corrupted data to be sent to the server. Make sure you keep Chat enabled active if you're using modded Chat2.gpk.",
                //        MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
                App.Settings.ChatEnabled = value;
                ChatManager.Instance.NotifyEnabledChanged(value);
                //StubInterface.Instance.StubClient.UpdateSetting("ChatEnabled", App.Settings.ChatEnabled);
                N();
            }
        }
        public bool ForceSoftwareRendering
        {
            get => App.Settings.ForceSoftwareRendering;
            set
            {
                if (App.Settings.ForceSoftwareRendering == value) return;
                App.Settings.ForceSoftwareRendering = value;
                N();
                RenderOptions.ProcessRenderMode = value ? RenderMode.SoftwareOnly : RenderMode.Default;
            }
        }
        public bool HighPriority
        {
            get => App.Settings.HighPriority;
            set
            {
                if (App.Settings.HighPriority == value) return;
                App.Settings.HighPriority = value;
                N();
                Process.GetCurrentProcess().PriorityClass = value ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            }
        }

        //public bool ShowConsole
        //{
        //    get => App.Settings.ShowConsole;
        //    set
        //    {
        //        if (App.Settings.ShowConsole == value) return;
        //        App.Settings.ShowConsole = value;
        //        N();

        //        if (value)
        //        {
        //            TccUtils.CreateConsole();
        //            Log.CW("Console opened");
        //        }
        //        else Kernel32.FreeConsole();
        //    }
        //}

        public IEnumerable<ClickThruMode> ClickThruModes
        {
            get
            {
                var ret = EnumUtils.ListFromEnum<ClickThruMode>();
                if (!App.Settings.EnableProxy || PacketAnalyzer.Factory?.ReleaseVersion/100 >= 97) ret.Remove(ClickThruMode.GameDriven);
                return ret;
            }
        }

        //TODO: https://stackoverflow.com/a/17405771 (in Nostrum)
        public IEnumerable<CooldownBarMode> CooldownBarModes => EnumUtils.ListFromEnum<CooldownBarMode>();
        public IEnumerable<FlowDirection> FlowDirections => EnumUtils.ListFromEnum<FlowDirection>();
        public IEnumerable<EnrageLabelMode> EnrageLabelModes => EnumUtils.ListFromEnum<EnrageLabelMode>();
        public IEnumerable<WarriorEdgeMode> WarriorEdgeModes => EnumUtils.ListFromEnum<WarriorEdgeMode>();
        public IEnumerable<ControlShape> ControlShapes => EnumUtils.ListFromEnum<ControlShape>();
        public IEnumerable<GroupWindowLayout> GroupWindowLayouts => EnumUtils.ListFromEnum<GroupWindowLayout>();
        public IEnumerable<GroupHpLabelMode> GroupHpLabelModes => EnumUtils.ListFromEnum<GroupHpLabelMode>();
        public IEnumerable<CaptureMode> CaptureModes => EnumUtils.ListFromEnum<CaptureMode>();
        public IEnumerable<MentionMode> MentionModes => EnumUtils.ListFromEnum<MentionMode>();
        public IEnumerable<LanguageOverride> LanguageOverrides => EnumUtils.ListFromEnum<LanguageOverride>();


        private TSObservableCollection<BlacklistedMonsterVM>? _blacklistedMonsters;
        private bool _showDebugSettings;

        public TSObservableCollection<BlacklistedMonsterVM> BlacklistedMonsters
        {
            get
            {
                _blacklistedMonsters ??= new TSObservableCollection<BlacklistedMonsterVM>(Dispatcher);
                if (Game.DB == null) return _blacklistedMonsters;
                 var bl =Game.DB.MonsterDatabase.GetBlacklistedMonsters();
                bl.ForEach(m =>
                {
                    if (_blacklistedMonsters.Any(x => x.Monster == m)) return;
                    _blacklistedMonsters.Add(new BlacklistedMonsterVM(m));
                });
                _blacklistedMonsters.ToSyncList().ForEach(vm =>
                {
                    if (bl.Contains(vm.Monster)) return;
                    _blacklistedMonsters.Remove(vm);
                });
                return _blacklistedMonsters;
            }
        }

        public bool EnablePlayerMenu
        {
            get => App.Settings.EnablePlayerMenu;
            set
            {
                if (App.Settings.EnablePlayerMenu == value) return;
                App.Settings.EnablePlayerMenu = value;
                N();
                StubInterface.Instance.StubClient.UpdateSetting("EnablePlayerMenu", App.Settings.EnablePlayerMenu);
            }
        }
                
        public bool ShowIngameChat
        {
            get => App.Settings.ShowIngameChat;
            set
            {
                if (App.Settings.ShowIngameChat == value) return;
                App.Settings.ShowIngameChat = value;
                N();
                StubInterface.Instance.StubClient.UpdateSetting("ShowIngameChat", App.Settings.ShowIngameChat);
            }
        }

        public bool ShowDebugSettings
        {
            get => _showDebugSettings;
            set
            {
                if(_showDebugSettings == value) return;
                _showDebugSettings = value;
                N();
            }
        }


        public SettingsWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            KeyboardHook.Instance.RegisterCallback(App.Settings.SettingsHotkey, OnShowSettingsWindowHotkeyPressed);

            BrowseUrlCommand = new RelayCommand(url =>
            {
                TccUtils.OpenUrl(url.ToString());
            });
            RegisterWebhookCommand = new RelayCommand(webhook => Firebase.RegisterWebhook(webhook.ToString(), true));
            OpenWindowCommand = new RelayCommand(winType =>
            {
                var t = (Type) winType;
                if (TccWindow.Exists(t)) return;
                var win = Activator.CreateInstance(t, null) as TccWindow;
                win?.ShowWindow();
            });
            OpenWelcomeWindowCommand = new RelayCommand(_ =>
            {
                new WelcomeWindow().Show();
            });
            DownloadBetaCommand = new RelayCommand(async (_) =>
            {
                if (TccMessageBox.Show(SR.BetaUnstableWarning, MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.Yes)
                {
                    await Task.Factory.StartNew(UpdateManager.ForceUpdateToBeta);
                }
            });
            ResetChatPositionsCommand = new RelayCommand(_ =>
            {
                foreach (var cw in ChatManager.Instance.ChatWindows)
                {
                    cw.ResetToCenter();
                }
            });
            MakePositionsGlobalCommand = new RelayCommand(_ => WindowManager.MakeGlobal());
            ResetWindowPositionsCommand = new RelayCommand(_ => WindowManager.ResetToCenter());
            OpenResourcesFolderCommand = new RelayCommand(_ => Process.Start(Path.Combine(App.ResourcesPath, "config")));
            ClearChatCommand = new RelayCommand(_ => ChatManager.Instance.ClearMessages());

            MonsterDatabase.BlacklistChangedEvent += MonsterDatabase_BlacklistChangedEvent;
            MessageFactory.ReleaseVersionChanged += OnReleaseVersionChanged;
        }

        private void OnReleaseVersionChanged(int obj)
        {
            N(nameof(ClickThruModes));
        }

        private void MonsterDatabase_BlacklistChangedEvent(uint arg1, uint arg2, bool arg3)
        {
            N(nameof(BlacklistedMonsters));
        }

        private void OnShowSettingsWindowHotkeyPressed()
        {
            if (WindowManager.SettingsWindow.IsVisible) WindowManager.SettingsWindow.HideWindow();
            else WindowManager.SettingsWindow.ShowWindow();
        }
        public static void PrintEventsData()
        {
            Log.CW($"ChatShowChannelChanged: {ChatShowChannelChanged?.GetInvocationList().Length}");
            Log.CW($"ChatShowTimestampChanged: {ChatShowTimestampChanged?.GetInvocationList().Length}");
            Log.CW($"FontSizeChanged: {FontSizeChanged?.GetInvocationList().Length}");
        }

    }

    public class BlacklistedMonsterVM : TSPropertyChanged
    {
        public readonly Monster Monster;
        public string Name => Monster.Name;
        public bool IsBoss => Monster.IsBoss;
        public bool IsHidden
        {
            get => Monster.IsHidden;
            set
            {
                if (Monster.IsHidden == value) return;
                Monster.IsHidden = value;
                if (!value) Game.DB.MonsterDatabase.Blacklist(Monster, false);
                N();
            }
        }

        public BlacklistedMonsterVM(Monster m)
        {
            Monster = m;
        }
    }
}
