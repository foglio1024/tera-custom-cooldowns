using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        private readonly bool _characterWindowExtendedMode;
        public static event Action ChatShowChannelChanged;
        public static event Action ChatShowTimestampChanged;

        public WindowSettings CooldownWindowSettings => Settings.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => Settings.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => Settings.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => Settings.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => Settings.CharacterWindowSettings;
        public WindowSettings BossWindowSettings => Settings.BossWindowSettings;
        public WindowSettings FlightWindowSettings => Settings.FlightGaugeWindowSettings;
        public WindowSettings FloatingButtonSettings => Settings.FloatingButtonSettings;

        public bool HideMe
        {
            get => Settings.IgnoreMeInGroupWindow;
            set
            {
                if (Settings.IgnoreMeInGroupWindow == value) return;
                Settings.IgnoreMeInGroupWindow = value;
                if (value) GroupWindowViewModel.Instance.RemoveMe();
                NPC();
            }
        }
        public bool HideBuffs
        {
            get => Settings.IgnoreGroupBuffs;
            set
            {
                if (Settings.IgnoreGroupBuffs == value) return;
                Settings.IgnoreGroupBuffs = value;
                NPC(nameof(HideBuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool HideDebuffs
        {
            get => Settings.IgnoreGroupDebuffs;
            set
            {
                if (Settings.IgnoreGroupDebuffs == value) return;
                Settings.IgnoreGroupDebuffs = value;
                NPC(nameof(HideDebuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool DisableAllPartyAbnormals
        {
            get => Settings.DisablePartyAbnormals;
            set
            {
                if (Settings.DisablePartyAbnormals == value) return;
                Settings.DisablePartyAbnormals = value;
                NPC(nameof(DisableAllPartyAbnormals));
                MessageFactory.Update();
                if (value) GroupWindowViewModel.Instance.ClearAllAbnormalities();
            }
        }
        public bool AccurateHp
        {
            get => Settings.AccurateHp;
            set
            {
                if (Settings.AccurateHp == value) return;
                Settings.AccurateHp = value;
                NPC(nameof(AccurateHp));
                MessageFactory.Update();
            }
        }

        public FlowDirection BuffsDirection
        {
            get => Settings.BuffsDirection;
            set
            {
                if (Settings.BuffsDirection == value) return;
                Settings.BuffsDirection = value;
                BuffBarWindowViewModel.Instance.NotifyDirectionChanged();
                NPC(nameof(BuffsDirection));
            }
        }
        public CooldownBarMode CooldownBarMode
        {
            get => Settings.CooldownBarMode;
            set
            {
                if (Settings.CooldownBarMode == value) return;
                Settings.CooldownBarMode = value;
                CooldownWindowViewModel.Instance.NotifyModeChanged();
                NPC(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => Settings.EnrageLabelMode;
            set
            {
                if (Settings.EnrageLabelMode == value) return;
                Settings.EnrageLabelMode = value;
                NPC(nameof(EnrageLabelMode));
            }
        }

        public bool ChatFadeOut
        {
            get => Settings.ChatFadeOut;
            set
            {
                if (Settings.ChatFadeOut == value) return;
                Settings.ChatFadeOut = value;
                if (value) ChatWindowManager.Instance.RefreshTimer();
                NPC(nameof(ChatFadeOut));
            }
        }
        public string RegionOverride
        {
            get => Settings.RegionOverride;
            set
            {
                if (Settings.RegionOverride == value) return;
                Settings.RegionOverride = value;
                NPC(nameof(RegionOverride));
            }
        }
        public int MaxMessages
        {
            get => Settings.MaxMessages;
            set
            {
                if (Settings.MaxMessages == value) return;
                var val = value;
                if (val < 20)
                {
                    val = 20;
                }
                Settings.MaxMessages = val;
                NPC();
            }
        }
        public int SpamThreshold
        {
            get => Settings.SpamThreshold;
            set
            {
                if (Settings.SpamThreshold == value) return;
                Settings.SpamThreshold = value;
                NPC();
            }
        }
        public bool ShowTimestamp
        {
            get => Settings.ShowTimestamp;
            set
            {
                if (Settings.ShowTimestamp == value) return;
                Settings.ShowTimestamp = value;
                NPC(nameof(ShowTimestamp));
                ChatShowTimestampChanged?.Invoke();
            }

        }
        public bool ShowChannel
        {
            get => Settings.ShowChannel;
            set
            {
                if (Settings.ShowChannel == value) return;
                Settings.ShowChannel = value;
                ChatShowChannelChanged?.Invoke();
                NPC(nameof(ShowChannel));
            }

        }
        public bool ShowOnlyBosses
        {
            get => Settings.ShowOnlyBosses;
            set
            {
                if (Settings.ShowOnlyBosses == value) return;
                Settings.ShowOnlyBosses = value;
                NPC(nameof(ShowOnlyBosses));
            }
        }
        public bool DisableMP
        {
            get => Settings.DisablePartyMP;
            set
            {
                if (Settings.DisablePartyMP == value) return;
                Settings.DisablePartyMP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NPC(nameof(DisableMP));
            }
        }
        public bool DisableHP
        {
            get => Settings.DisablePartyHP;
            set
            {
                if (Settings.DisablePartyHP == value) return;
                Settings.DisablePartyHP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NPC(nameof(DisableHP));
            }
        }
        public bool ShowAwakenIcon
        {
            get => Settings.ShowAwakenIcon;
            set
            {
                if (Settings.ShowAwakenIcon == value) return;
                Settings.ShowAwakenIcon = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC();
            }
        }

        public bool ShowItemsCooldown
        {
            get => Settings.ShowItemsCooldown;
            set
            {
                if (Settings.ShowItemsCooldown == value) return;
                Settings.ShowItemsCooldown = value;
                CooldownWindowViewModel.Instance.NotifyItemsDisplay();
                NPC(nameof(ShowItemsCooldown));
            }
        }
        public bool UseLfg
        {
            get => Settings.LfgEnabled;
            set
            {
                if (Settings.LfgEnabled == value) return;
                Settings.LfgEnabled = value;
                NPC();
            }
        }
        public bool ShowFlightGauge
        {
            get => Settings.ShowFlightEnergy;
            set
            {
                if (Settings.ShowFlightEnergy == value) return;
                Settings.ShowFlightEnergy = value;
                NPC();
            }
        }
        public bool UseHotkeys
        {
            get => Settings.UseHotkeys;
            set
            {
                if (Settings.UseHotkeys == value) return;
                Settings.UseHotkeys = value;
                if (value) KeyboardHook.Instance.RegisterKeyboardHook();
                else KeyboardHook.Instance.UnRegisterKeyboardHook();
                NPC(nameof(UseHotkeys));
            }
        }
        public bool HideHandles
        {
            get => Settings.HideHandles;
            set
            {
                if (Settings.HideHandles == value) return;
                Settings.HideHandles = value;
                NPC(nameof(HideHandles));
            }
        }
        public bool ShowGroupWindowDetails
        {
            get => Settings.ShowGroupWindowDetails;
            set
            {
                if (Settings.ShowGroupWindowDetails == value) return;
                Settings.ShowGroupWindowDetails = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC(nameof(ShowGroupWindowDetails));
            }
        }
        public bool ShowMembersLaurels
        {
            get => Settings.ShowMembersLaurels;
            set
            {
                if (Settings.ShowMembersLaurels == value) return;
                Settings.ShowMembersLaurels = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC(nameof(ShowMembersLaurels));
            }
        }
        public bool AnimateChatMessages
        {
            get => Settings.AnimateChatMessages;
            set
            {
                if (Settings.AnimateChatMessages == value) return;
                Settings.AnimateChatMessages = value;
                NPC(nameof(AnimateChatMessages));
            }
        }
        public bool HhOnlyAggro
        {
            get => Settings.ShowOnlyAggroStacks;
            set
            {
                if (Settings.ShowOnlyAggroStacks == value) return;
                Settings.ShowOnlyAggroStacks = value;
                NPC(nameof(HhOnlyAggro));
            }
        }
        public double FlightGaugeRotation
        {
            get => Settings.FlightGaugeRotation;
            set
            {
                if (Settings.FlightGaugeRotation == value) return;
                Settings.FlightGaugeRotation = value;
                NPC(nameof(FlightGaugeRotation));
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlightGaugeRotation));
            }
        }
        //public bool LfgOn
        //{
        //    get => Settings.LfgOn;
        //    set
        //    {
        //        if (Settings.LfgOn == value) return;
        //        Settings.LfgOn = value;
        //        ChatWindowManager.Instance.LfgOn = value;
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(LfgOn));

        //    }
        //}
        public string Webhook
        {
            get => Settings.Webhook;
            set
            {
                if (value == Settings.Webhook) return;
                Settings.Webhook = value;
                NPC(nameof(Webhook));
            }
        }
        public string WebhookMessage
        {
            get => Settings.WebhookMessage;
            set
            {
                if (value == Settings.WebhookMessage) return;
                Settings.WebhookMessage = value;
                NPC(nameof(WebhookMessage));
            }
        }
        public string TwitchUsername
        {
            get => Settings.TwitchName;
            set
            {
                if (value == Settings.TwitchName) return;
                Settings.TwitchName = value;
                NPC(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => Settings.TwitchToken;
            set
            {
                if (value == Settings.TwitchToken) return;
                Settings.TwitchToken = value;
                NPC(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => Settings.TwitchChannelName;
            set
            {
                if (value == Settings.TwitchChannelName) return;
                Settings.TwitchChannelName = value;
                NPC(nameof(TwitchChannelName));
            }
        }
        public uint GroupSizeThreshold
        {
            get => Settings.GroupSizeThreshold;
            set
            {
                if (Settings.GroupSizeThreshold == value) return;
                Settings.GroupSizeThreshold = value;
                GroupWindowViewModel.Instance.NotifyThresholdChanged();
                NPC(nameof(GroupSizeThreshold));
            }
        }

        public double ChatWindowOpacity
        {
            get => Settings.ChatWindowOpacity;
            set
            {
                if (Settings.ChatWindowOpacity == value) return;
                Settings.ChatWindowOpacity = value;
                ChatWindowManager.Instance.NotifyOpacityChange();
                NPC(nameof(ChatWindowOpacity));
            }
        }
        public int FontSize
        {
            get => Settings.FontSize;
            set
            {
                if (Settings.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                Settings.FontSize = val;
                NPC(nameof(FontSize));
            }
        }
        public SettingsWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //if (Settings.CooldownWindowSettings.Enabled) WindowManager.CooldownWindow.PropertyChanged += CooldownWindow_PropertyChanged;
            //if (Settings.CharacterWindowSettings.Enabled) WindowManager.CharacterWindow.PropertyChanged += CharacterWindow_PropertyChanged;
            //if (Settings.BossWindowSettings.Enabled) WindowManager.BossWindow.PropertyChanged += BossGauge_PropertyChanged;
            //if (Settings.BuffWindowSettings.Enabled) WindowManager.BuffWindow.PropertyChanged += BuffBar_PropertyChanged;
            //if (Settings.GroupWindowSettings.Enabled) WindowManager.GroupWindow.PropertyChanged += GroupWindow_PropertyChanged;
            //if (Settings.ClassWindowSettings.Enabled) WindowManager.ClassWindow.PropertyChanged += ClassWindow_PropertyChanged;
        }

        //private void ClassWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsClassWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsClassWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void GroupWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsGroupWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsGroupWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void BuffBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsBuffWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsBuffWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void BossGauge_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsBossWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;

        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsBossWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void CharacterWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsCharacterWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsCharacterWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}
        //private void CooldownWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Visibility")
        //    {
        //        IsCooldownWindowVisible = ((TccWindow)sender).Visibility == Visibility.Hidden ? false : true;
        //    }
        //    else if (e.PropertyName == "ClickThruMode")
        //    {
        //        //IsCooldownWindowTransparent = ((TccWindow)sender).ClickThru;
        //    }
        //}

        public List<ClickThruMode> ClickThruModes => Utils.ListFromEnum<ClickThruMode>();
        public List<ClickThruMode> ChatClickThruModes => new List<ClickThruMode> { ClickThruMode.Never, ClickThruMode.GameDriven };
        public List<CooldownBarMode> CooldownBarModes => Utils.ListFromEnum<CooldownBarMode>();
        public List<FlowDirection> FlowDirections => Utils.ListFromEnum<FlowDirection>();
        public List<EnrageLabelMode> EnrageLabelModes => Utils.ListFromEnum<EnrageLabelMode>();
        public List<WarriorEdgeMode> WarriorEdgeModes => Utils.ListFromEnum<WarriorEdgeMode>();

        public bool ChatWindowEnabled
        {
            get => Settings.ChatEnabled;
            set
            {
                if (Settings.ChatEnabled == value) return;
                Settings.ChatEnabled = value;
                NPC();
            }
        }

        public ClickThruMode ChatClickThruMode
        {
            get => Settings.ChatClickThruMode;
            set
            {
                if (Settings.ChatClickThruMode == value) return;
                Settings.ChatClickThruMode = value;
                NPC();
            }
        }

        public bool ShowTradeLfgs
        {
            get => Settings.ShowTradeLfg;
            set
            {
                if (Settings.ShowTradeLfg == value) return;
                Settings.ShowTradeLfg = value;
                NPC();
            }
        }

        public bool CharacterWindowCompactMode
        {
            get { return Settings.CharacterWindowCompactMode; }
            set
            {
                if (Settings.CharacterWindowCompactMode == value) return;
                Settings.CharacterWindowCompactMode = value;
                NPC();
                CharacterWindowViewModel.Instance.ExNPC(nameof(CharacterWindowViewModel.CompactMode));
                WindowManager.CharacterWindow.Left = value ? WindowManager.CharacterWindow.Left + 175 :
                    WindowManager.CharacterWindow.Left - 175;

            }
        }

        public bool WarriorShowEdge
        {
            get => Settings.WarriorShowEdge;
            set
            {
                if (Settings.WarriorShowEdge == value) return;
                Settings.WarriorShowEdge = value;
                NPC();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExNPC(nameof(WarriorBarManager.ShowEdge));
            }
        }

        public bool WarriorShowTraverseCut
        {
            get => Settings.WarriorShowTraverseCut;
            set
            {
                if (Settings.WarriorShowTraverseCut == value) return;
                Settings.WarriorShowTraverseCut = value;
                NPC();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExNPC(nameof(WarriorBarManager.ShowTraverseCut));
            }
        }

        public WarriorEdgeMode WarriorEdgeMode
        {
            get => Settings.WarriorEdgeMode;
            set
            {
                if (Settings.WarriorEdgeMode == value) return;
                Settings.WarriorEdgeMode = value;
                NPC();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExNPC(nameof(WarriorBarManager.WarriorEdgeMode));
            }

        }

        public bool FlipFlightGauge
        {
            get => Settings.FlipFlightGauge;
            set
            {
                if (Settings.FlipFlightGauge == value) return;
                Settings.FlipFlightGauge = value;
                NPC();
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlipFlightGauge));
            }
        }
        public bool ForceSoftwareRendering
        {
            get => Settings.ForceSoftwareRendering;
            set
            {
                if (Settings.ForceSoftwareRendering == value) return;
                Settings.ForceSoftwareRendering = value;
                NPC();
                RenderOptions.ProcessRenderMode = value ? RenderMode.SoftwareOnly : RenderMode.Default;
            }
        }

        public bool HighPriority
        {
            get => Settings.HighPriority;
            set
            {
                if (Settings.HighPriority == value) return;
                Settings.HighPriority = value;
                NPC();
                Process.GetCurrentProcess().PriorityClass = value ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            }
        }
    }
}
