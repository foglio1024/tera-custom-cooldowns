using FoglioUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Data;
using TCC.Interop;
using TCC.Settings;
using TCC.Utilities;
using TCC.Windows;
using CaptureMode = TCC.Data.CaptureMode;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        public static event Action ChatShowChannelChanged;
        public static event Action ChatShowTimestampChanged;
        public static event Action AbnormalityShapeChanged;
        public static event Action SkillShapeChanged;
        public static event Action FontSizeChanged;

        public bool Experimental => App.Experimental;

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
                        WindowManager.ViewModels.NotificationAreaVM.Enqueue("Exploit alert", "Are you sure you want to enable this?", NotificationType.Warning);
                        break;
                    case 1:
                        WindowManager.ViewModels.NotificationAreaVM.Enqueue(":thinking:", "You shouldn't use this °L° Are you really sure?", NotificationType.Warning, 3000);
                        break;
                    case 2:
                        WindowManager.ViewModels.NotificationAreaVM.Enqueue("omegalul", "There's actually no Kylos helper lol. Just memeing. Have fun o/", NotificationType.Warning, 6000);
                        Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
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
        public bool ExperimentalNotification
        {
            get => App.Settings.ExperimentalNotification;
            set
            {
                if (App.Settings.ExperimentalNotification == value) return;
                App.Settings.ExperimentalNotification = value;
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
        public string RegionOverride
        {
            get => App.Settings.LanguageOverride;
            set
            {
                if (App.Settings.LanguageOverride == value) return;
                App.Settings.LanguageOverride = value;
                if (value == "") App.Settings.LastLanguage = "EU-EN";
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
            }
        }
        public bool HideHandles
        {
            get => App.Settings.HideHandles;
            set
            {
                if (App.Settings.HideHandles == value) return;
                App.Settings.HideHandles = value;
                N(nameof(HideHandles));
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
                var res = TccMessageBox.Show("TCC", "TCC needs to be restarted to apply this setting. Restart now?",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Cancel) return;
                App.Settings.CaptureMode = value;
                N();
                if (res == MessageBoxResult.OK) App.Restart();
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
                App.Settings.ChatEnabled = value;
                ChatWindowManager.Instance.NotifyEnabledChanged(value);
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

        public IEnumerable<ClickThruMode> ClickThruModes => EnumUtils.ListFromEnum<ClickThruMode>();
        public IEnumerable<CooldownBarMode> CooldownBarModes => EnumUtils.ListFromEnum<CooldownBarMode>();
        public IEnumerable<FlowDirection> FlowDirections => EnumUtils.ListFromEnum<FlowDirection>();
        public IEnumerable<EnrageLabelMode> EnrageLabelModes => EnumUtils.ListFromEnum<EnrageLabelMode>();
        public IEnumerable<WarriorEdgeMode> WarriorEdgeModes => EnumUtils.ListFromEnum<WarriorEdgeMode>();
        public IEnumerable<ControlShape> ControlShapes => EnumUtils.ListFromEnum<ControlShape>();
        public IEnumerable<GroupWindowLayout> GroupWindowLayouts => EnumUtils.ListFromEnum<GroupWindowLayout>();
        public IEnumerable<CaptureMode> CaptureModes => EnumUtils.ListFromEnum<CaptureMode>();

        public SettingsWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            KeyboardHook.Instance.RegisterCallback(App.Settings.SettingsHotkey, OnShowSettingsWindowHotkeyPressed);

            BrowseUrlCommand = new RelayCommand(url => Process.Start(url.ToString()));
            RegisterWebhookCommand = new RelayCommand(webhook => Firebase.RegisterWebhook(webhook.ToString(), true));
            OpenWindowCommand = new RelayCommand(winType =>
            {
                var t = (Type)winType;
                var win = Activator.CreateInstance(t, null) as TccWindow;
                win?.ShowWindow();
            });
            DownloadBetaCommand = new RelayCommand(async (_) =>
            {
                if (TccMessageBox.Show("Warning: beta build could be unstable. Proceed?",
                        MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.Yes)
                {
                    await Task.Factory.StartNew(UpdateManager.ForceUpdateExperimental);
                }
            });
            ResetChatPositionsCommand = new RelayCommand(_ =>
            {
                foreach (var cw in ChatWindowManager.Instance.ChatWindows)
                {
                    cw.ResetToCenter();
                }
            });
            MakePositionsGlobalCommand = new RelayCommand(_ => WindowManager.MakeGlobal());
            ResetWindowPositionsCommand = new RelayCommand(_ => WindowManager.ResetToCenter());
            OpenResourcesFolderCommand = new RelayCommand(_ => Process.Start(Path.Combine(App.BasePath, "resources/config")));
            ClearChatCommand = new RelayCommand(_ => ChatWindowManager.Instance.ClearMessages());
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
}
