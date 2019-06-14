using FoglioUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;
using TCC.Settings;
using TCC.Windows;
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

        public WindowSettings CooldownWindowSettings => App.Settings.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => App.Settings.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => App.Settings.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => App.Settings.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => App.Settings.CharacterWindowSettings;
        public WindowSettings BossWindowSettings => App.Settings.NpcWindowSettings;
        public WindowSettings FlightWindowSettings => App.Settings.FlightGaugeWindowSettings;
        public WindowSettings FloatingButtonSettings => App.Settings.FloatingButtonSettings;
        public WindowSettings CuWindowSettings => App.Settings.CivilUnrestWindowSettings;

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
                        WindowManager.FloatingButton.NotifyExtended("Exploit alert", "Are you sure you want to enable this?", NotificationType.Warning);
                        break;
                    case 1:
                        WindowManager.FloatingButton.NotifyExtended(":thinking:", "You shouldn't use this °L° Are you really sure?", NotificationType.Warning, 3000);
                        break;
                    case 2:
                        WindowManager.FloatingButton.NotifyExtended("omegalul", "There's actually no Kylos helper lol. Just memeing. Have fun o/", NotificationType.Warning, 6000);
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
        public bool ShowMembersHpNumbers
        {
            get => App.Settings.GroupWindowSettings.ShowHpLabels;
            set
            {
                if (App.Settings.GroupWindowSettings.ShowHpLabels == value) return;
                App.Settings.GroupWindowSettings.ShowHpLabels = value;
                WindowManager.ViewModels.Group.NotifySettingUpdated();
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
        public bool HideMe
        {
            get => App.Settings.GroupWindowSettings.IgnoreMe;
            set
            {
                if (App.Settings.GroupWindowSettings.IgnoreMe == value) return;
                App.Settings.GroupWindowSettings.IgnoreMe = value;
                WindowManager.ViewModels.Group.ToggleMe(!value);
                N();
            }
        }

        public uint HideBuffsThreshold
        {
            get => App.Settings.GroupWindowSettings.HideBuffsThreshold;
            set
            {
                if (App.Settings.GroupWindowSettings.HideBuffsThreshold == value) return;
                App.Settings.GroupWindowSettings.HideBuffsThreshold = value;
                N();
                WindowManager.ViewModels.Group.NotifySettingUpdated();
            }
        }
        public uint HideDebuffsThreshold
        {
            get => App.Settings.GroupWindowSettings.HideDebuffsThreshold;
            set
            {
                if (App.Settings.GroupWindowSettings.HideDebuffsThreshold == value) return;
                App.Settings.GroupWindowSettings.HideDebuffsThreshold = value;
                N();
                WindowManager.ViewModels.Group.NotifySettingUpdated();
            }
        }
        public uint HideHpThreshold
        {
            get => App.Settings.GroupWindowSettings.HideHpThreshold;
            set
            {
                if (App.Settings.GroupWindowSettings.HideHpThreshold == value) return;
                App.Settings.GroupWindowSettings.HideHpThreshold = value;
                N();
                WindowManager.ViewModels.Group.NotifySettingUpdated();
            }
        }
        public uint HideMpThreshold
        {
            get => App.Settings.GroupWindowSettings.HideMpThreshold;
            set
            {
                if (App.Settings.GroupWindowSettings.HideMpThreshold == value) return;
                App.Settings.GroupWindowSettings.HideMpThreshold = value;
                N();
                WindowManager.ViewModels.Group.NotifySettingUpdated();
            }
        }
        public uint DisableAbnormalitiesThreshold
        {
            get => App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold;
            set
            {
                if (App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold == value) return;
                App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold = value;
                N();
                WindowManager.ViewModels.Group.NotifySettingUpdated();
            }
        }

        public bool AccurateHp
        {
            get => App.Settings.NpcWindowSettings.AccurateHp;
            set
            {
                if (App.Settings.NpcWindowSettings.AccurateHp == value) return;
                App.Settings.NpcWindowSettings.AccurateHp = value;
                N(nameof(AccurateHp));
                PacketAnalyzer.Processor.Update();
            }
        }

        public FlowDirection BuffsDirection
        {
            get => App.Settings.BuffWindowSettings.Direction;
            set
            {
                if (App.Settings.BuffWindowSettings.Direction == value) return;
                App.Settings.BuffWindowSettings.Direction = value;
                WindowManager.ViewModels.Abnormal.ExN(nameof(BuffBarWindowViewModel.Direction));
                N(nameof(BuffsDirection));
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
        public CooldownBarMode CooldownBarMode
        {
            get => App.Settings.CooldownWindowSettings.Mode;
            set
            {
                if (App.Settings.CooldownWindowSettings.Mode == value) return;
                App.Settings.CooldownWindowSettings.Mode = value;
                WindowManager.ViewModels.Cooldowns.NotifyModeChanged();
                N(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => App.Settings.NpcWindowSettings.EnrageLabelMode;
            set
            {
                if (App.Settings.NpcWindowSettings.EnrageLabelMode == value) return;
                App.Settings.NpcWindowSettings.EnrageLabelMode = value;
                N(nameof(EnrageLabelMode));
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
        public bool ShowOnlyBosses
        {
            get => App.Settings.NpcWindowSettings.ShowOnlyBosses;
            set
            {
                if (App.Settings.NpcWindowSettings.ShowOnlyBosses == value) return;
                App.Settings.NpcWindowSettings.ShowOnlyBosses = value;
                N(nameof(ShowOnlyBosses));
            }
        }
        public bool ShowAwakenIcon
        {
            get => App.Settings.GroupWindowSettings.ShowAwakenIcon;
            set
            {
                if (App.Settings.GroupWindowSettings.ShowAwakenIcon == value) return;
                App.Settings.GroupWindowSettings.ShowAwakenIcon = value;
                WindowManager.ViewModels.Group.NotifySettingUpdated();
                N();
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

        public bool ShowItemsCooldown
        {
            get => App.Settings.CooldownWindowSettings.ShowItems;
            set
            {
                if (App.Settings.CooldownWindowSettings.ShowItems == value) return;
                App.Settings.CooldownWindowSettings.ShowItems = value;
                WindowManager.ViewModels.Cooldowns.NotifyItemsDisplay();
                N();
            }
        }
        public bool UseLfg
        {
            get => App.Settings.LfgEnabled;
            set
            {
                if (App.Settings.LfgEnabled == value) return;
                App.Settings.LfgEnabled = value;
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
        public bool ShowFlightGauge
        {
            get => App.Settings.FlightGaugeWindowSettings.Enabled;
            set
            {
                if (App.Settings.FlightGaugeWindowSettings.Enabled== value) return;
                App.Settings.FlightGaugeWindowSettings.Enabled = value;
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
                if (value) KeyboardHook.Instance.RegisterKeyboardHook();
                else KeyboardHook.Instance.UnRegisterKeyboardHook();
                N(nameof(UseHotkeys));
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
        public bool ShowGroupWindowDetails
        {
            get => App.Settings.GroupWindowSettings.ShowDetails;
            set
            {
                if (App.Settings.GroupWindowSettings.ShowDetails == value) return;
                App.Settings.GroupWindowSettings.ShowDetails = value;
                WindowManager.ViewModels.Group.NotifySettingUpdated();
                N(nameof(ShowGroupWindowDetails));
            }
        }
        public bool ShowMembersLaurels
        {
            get => App.Settings.GroupWindowSettings.ShowLaurels;
            set
            {
                if (App.Settings.GroupWindowSettings.ShowLaurels == value) return;
                App.Settings.GroupWindowSettings.ShowLaurels = value;
                WindowManager.ViewModels.Group.NotifySettingUpdated();
                N(nameof(ShowMembersLaurels));
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
        public bool HhOnlyAggro
        {
            get => App.Settings.GroupWindowSettings.ShowOnlyAggroStacks;
            set
            {
                if (App.Settings.GroupWindowSettings.ShowOnlyAggroStacks == value) return;
                App.Settings.GroupWindowSettings.ShowOnlyAggroStacks = value;
                N(nameof(HhOnlyAggro));
            }
        }
        public double FlightGaugeRotation
        {
            get => App.Settings.FlightGaugeWindowSettings.Rotation;
            set
            {
                if (App.Settings.FlightGaugeWindowSettings.Rotation == value) return;
                App.Settings.FlightGaugeWindowSettings.Rotation = value;
                N(nameof(FlightGaugeRotation));
                WindowManager.ViewModels.FlightGauge.ExN(nameof(FlightGaugeRotation));
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
        public bool ShowNotificationBubble
        {
            get => App.Settings.FloatingButtonSettings.ShowNotificationBubble;
            set
            {
                if (App.Settings.FloatingButtonSettings.ShowNotificationBubble == value) return;
                App.Settings.FloatingButtonSettings.ShowNotificationBubble = value;
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
        public uint GroupSizeThreshold
        {
            get => App.Settings.GroupWindowSettings.GroupSizeThreshold;
            set
            {
                if (App.Settings.GroupWindowSettings.GroupSizeThreshold == value) return;
                App.Settings.GroupWindowSettings.GroupSizeThreshold = value;
                WindowManager.ViewModels.Group.NotifyThresholdChanged();
                N(nameof(GroupSizeThreshold));
            }
        }
        public bool Npcap
        {
            get => App.Settings.Npcap;
            set
            {
                if (App.Settings.Npcap == value) return;
                var res = TccMessageBox.Show("TCC", "TCC needs to be restarted to apply this setting. Restart now?",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Cancel) return;
                App.Settings.Npcap = value;
                N();
                if (res == MessageBoxResult.OK) App.Restart();
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
        //public double ChatWindowOpacity
        //{
        //    get => Settings.Settings.ChatWindowOpacity;
        //    set
        //    {
        //        if (Settings.Settings.ChatWindowOpacity == value) return;
        //        Settings.Settings.ChatWindowOpacity = value;
        //        ChatWindowManager.Instance.NotifyOpacityChange();
        //        NPC(nameof(ChatWindowOpacity));
        //    }
        //}
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
        public SettingsWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
        public bool Experimental => App.Experimental;

        public IEnumerable<ClickThruMode> ClickThruModes => EnumUtils.ListFromEnum<ClickThruMode>();
        //public IEnumerable<ClickThruMode> ChatClickThruModes => new List<ClickThruMode> { ClickThruMode.Never, ClickThruMode.GameDriven };
        public IEnumerable<CooldownBarMode> CooldownBarModes => EnumUtils.ListFromEnum<CooldownBarMode>();
        public IEnumerable<FlowDirection> FlowDirections => EnumUtils.ListFromEnum<FlowDirection>();
        public IEnumerable<EnrageLabelMode> EnrageLabelModes => EnumUtils.ListFromEnum<EnrageLabelMode>();
        public IEnumerable<WarriorEdgeMode> WarriorEdgeModes => EnumUtils.ListFromEnum<WarriorEdgeMode>();
        public IEnumerable<ControlShape> ControlShapes => EnumUtils.ListFromEnum<ControlShape>();
        public IEnumerable<GroupWindowLayout> GroupWindowLayouts => EnumUtils.ListFromEnum<GroupWindowLayout>();
        public IEnumerable<CaptureMode> CaptureModes => EnumUtils.ListFromEnum<CaptureMode>();

        public bool ChatWindowEnabled
        {
            get => App.Settings.ChatEnabled;
            set
            {
                if (App.Settings.ChatEnabled == value) return;
                App.Settings.ChatEnabled = value;
                N();
            }
        }

        public bool ShowTradeLfgs
        {
            get => App.Settings.ShowTradeLfg;
            set
            {
                if (App.Settings.ShowTradeLfg == value) return;
                App.Settings.ShowTradeLfg = value;
                N();
            }
        }

        public bool CharacterWindowCompactMode
        {
            get => App.Settings.CharacterWindowSettings.CompactMode;
            set
            {
                if (App.Settings.CharacterWindowSettings.CompactMode == value) return;
                App.Settings.CharacterWindowSettings.CompactMode = value;
                N();
                WindowManager.ViewModels.Character.InvokeCompactModeChanged();

            }
        }

        public bool WarriorShowEdge
        {
            get => App.Settings.ClassWindowSettings.WarriorShowEdge;
            set
            {
                if (App.Settings.ClassWindowSettings.WarriorShowEdge == value) return;
                App.Settings.ClassWindowSettings.WarriorShowEdge = value;
                N();
                TccUtils.CurrentClassVM<WarriorLayoutVM>()?.ExN(nameof(WarriorLayoutVM.ShowEdge));
            }
        }
        public bool SorcererReplacesElementsInCharWindow
        {
            get => App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow;
            set
            {
                if (App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow == value) return;
                App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow = value;
                N();
                WindowManager.ViewModels.Character.ExN(nameof(CharacterWindowViewModel.ShowElements));
            }
        }

        public bool WarriorShowTraverseCut
        {
            get => App.Settings.ClassWindowSettings.WarriorShowTraverseCut;
            set
            {
                if (App.Settings.ClassWindowSettings.WarriorShowTraverseCut == value) return;
                App.Settings.ClassWindowSettings.WarriorShowTraverseCut = value;
                N();
                TccUtils.CurrentClassVM<WarriorLayoutVM>()?.ExN(nameof(WarriorLayoutVM.ShowTraverseCut));
            }
        }

        public WarriorEdgeMode WarriorEdgeMode
        {
            get => App.Settings.ClassWindowSettings.WarriorEdgeMode;
            set
            {
                if (App.Settings.ClassWindowSettings.WarriorEdgeMode == value) return;
                App.Settings.ClassWindowSettings.WarriorEdgeMode = value;
                N();
                TccUtils.CurrentClassVM<WarriorLayoutVM>()?.ExN(nameof(WarriorLayoutVM.WarriorEdgeMode));
            }

        }
        public GroupWindowLayout GroupWindowLayout
        {
            get => App.Settings.GroupWindowSettings.Layout;
            set
            {
                if (App.Settings.GroupWindowSettings.Layout == value) return;
                App.Settings.GroupWindowSettings.Layout = value;
                N();
                WindowManager.ViewModels.Group.ExN(nameof(GroupWindowViewModel.GroupWindowLayout));
                WindowManager.ViewModels.Group.ExN(nameof(GroupWindowViewModel.All));
                WindowManager.ViewModels.Group.ExN(nameof(GroupWindowViewModel.Dps));
                WindowManager.ViewModels.Group.ExN(nameof(GroupWindowViewModel.Healers));
                WindowManager.ViewModels.Group.ExN(nameof(GroupWindowViewModel.Tanks));
            }

        }

        public bool FlipFlightGauge
        {
            get => App.Settings.FlightGaugeWindowSettings.Flip;
            set
            {
                if (App.Settings.FlightGaugeWindowSettings.Flip == value) return;
                App.Settings.FlightGaugeWindowSettings.Flip = value;
                N();
                WindowManager.ViewModels.FlightGauge.ExN(nameof(FlipFlightGauge));
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

    }
}
