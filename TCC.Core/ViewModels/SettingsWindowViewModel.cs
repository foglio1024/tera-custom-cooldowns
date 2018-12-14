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

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        public static event Action ChatShowChannelChanged;
        public static event Action ChatShowTimestampChanged;
        public static event Action AbnormalityShapeChanged;
        public static event Action SkillShapeChanged;
        public static event Action FontSizeChanged;

        public WindowSettings CooldownWindowSettings => SettingsStorage.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => SettingsStorage.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => SettingsStorage.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => SettingsStorage.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => SettingsStorage.CharacterWindowSettings;
        public WindowSettings BossWindowSettings => SettingsStorage.BossWindowSettings;
        public WindowSettings FlightWindowSettings => SettingsStorage.FlightGaugeWindowSettings;
        public WindowSettings FloatingButtonSettings => SettingsStorage.FloatingButtonSettings;
        public WindowSettings CuWindowSettings => SettingsStorage.CivilUnrestWindowSettings;

        private int _khCount = 0;
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
                        WindowManager.FloatingButton.NotifyExtended("Exploit alert", "Are you sure you want to enable this?", NotificationType.Warning, 4000);
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
            get => SettingsStorage.EthicalMode;
            set
            {
                if (SettingsStorage.EthicalMode == value) return;
                SettingsStorage.EthicalMode = value;
                N();
            }
        }
        public bool CheckOpcodesHash
        {
            get => SettingsStorage.CheckOpcodesHash;
            set
            {
                if (SettingsStorage.CheckOpcodesHash == value) return;
                SettingsStorage.CheckOpcodesHash = value;
                N();
            }
        }
        public bool HideMe
        {
            get => SettingsStorage.IgnoreMeInGroupWindow;
            set
            {
                if (SettingsStorage.IgnoreMeInGroupWindow == value) return;
                SettingsStorage.IgnoreMeInGroupWindow = value;
                if (value) GroupWindowViewModel.Instance.RemoveMe();
                N();
            }
        }
        public bool HideBuffs
        {
            get => SettingsStorage.IgnoreGroupBuffs;
            set
            {
                if (SettingsStorage.IgnoreGroupBuffs == value) return;
                SettingsStorage.IgnoreGroupBuffs = value;
                N(nameof(HideBuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool HideDebuffs
        {
            get => SettingsStorage.IgnoreGroupDebuffs;
            set
            {
                if (SettingsStorage.IgnoreGroupDebuffs == value) return;
                SettingsStorage.IgnoreGroupDebuffs = value;
                N(nameof(HideDebuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool DisableAllPartyAbnormals
        {
            get => SettingsStorage.DisablePartyAbnormals;
            set
            {
                if (SettingsStorage.DisablePartyAbnormals == value) return;
                SettingsStorage.DisablePartyAbnormals = value;
                N(nameof(DisableAllPartyAbnormals));
                MessageFactory.Update();
                if (value) GroupWindowViewModel.Instance.ClearAllAbnormalities();
            }
        }
        public bool AccurateHp
        {
            get => SettingsStorage.AccurateHp;
            set
            {
                if (SettingsStorage.AccurateHp == value) return;
                SettingsStorage.AccurateHp = value;
                N(nameof(AccurateHp));
                MessageFactory.Update();
            }
        }

        public FlowDirection BuffsDirection
        {
            get => SettingsStorage.BuffsDirection;
            set
            {
                if (SettingsStorage.BuffsDirection == value) return;
                SettingsStorage.BuffsDirection = value;
                BuffBarWindowViewModel.Instance.ExN(nameof(BuffBarWindowViewModel.Direction));
                N(nameof(BuffsDirection));
            }
        }
        public ControlShape AbnormalityShape
        {
            get => SettingsStorage.AbnormalityShape;
            set
            {
                if (SettingsStorage.AbnormalityShape == value) return;
                SettingsStorage.AbnormalityShape = value;
                AbnormalityShapeChanged?.Invoke();
                N();
            }
        }
        public ControlShape SkillShape
        {
            get => SettingsStorage.SkillShape;
            set
            {
                if (SettingsStorage.SkillShape == value) return;
                SettingsStorage.SkillShape = value;
                SkillShapeChanged?.Invoke();
                N();
            }
        }
        public CooldownBarMode CooldownBarMode
        {
            get => SettingsStorage.CooldownBarMode;
            set
            {
                if (SettingsStorage.CooldownBarMode == value) return;
                SettingsStorage.CooldownBarMode = value;
                CooldownWindowViewModel.Instance.NotifyModeChanged();
                N(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => SettingsStorage.EnrageLabelMode;
            set
            {
                if (SettingsStorage.EnrageLabelMode == value) return;
                SettingsStorage.EnrageLabelMode = value;
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
            get => SettingsStorage.RegionOverride;
            set
            {
                if (SettingsStorage.RegionOverride == value) return;
                SettingsStorage.RegionOverride = value;
                N(nameof(RegionOverride));
            }
        }
        public int MaxMessages
        {
            get => SettingsStorage.MaxMessages;
            set
            {
                if (SettingsStorage.MaxMessages == value) return;
                var val = value;
                if (val < 20)
                {
                    val = 20;
                }
                SettingsStorage.MaxMessages = val;
                N();
            }
        }
        public int SpamThreshold
        {
            get => SettingsStorage.SpamThreshold;
            set
            {
                if (SettingsStorage.SpamThreshold == value) return;
                SettingsStorage.SpamThreshold = value;
                N();
            }
        }
        public bool ShowTimestamp
        {
            get => SettingsStorage.ShowTimestamp;
            set
            {
                if (SettingsStorage.ShowTimestamp == value) return;
                SettingsStorage.ShowTimestamp = value;
                N(nameof(ShowTimestamp));
                ChatShowTimestampChanged?.Invoke();
            }

        }
        public bool ShowChannel
        {
            get => SettingsStorage.ShowChannel;
            set
            {
                if (SettingsStorage.ShowChannel == value) return;
                SettingsStorage.ShowChannel = value;
                ChatShowChannelChanged?.Invoke();
                N(nameof(ShowChannel));
            }

        }
        public bool ShowOnlyBosses
        {
            get => SettingsStorage.ShowOnlyBosses;
            set
            {
                if (SettingsStorage.ShowOnlyBosses == value) return;
                SettingsStorage.ShowOnlyBosses = value;
                N(nameof(ShowOnlyBosses));
            }
        }
        public bool DisableMP
        {
            get => SettingsStorage.DisablePartyMP;
            set
            {
                if (SettingsStorage.DisablePartyMP == value) return;
                SettingsStorage.DisablePartyMP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                N(nameof(DisableMP));
            }
        }
        public bool DisableHP
        {
            get => SettingsStorage.DisablePartyHP;
            set
            {
                if (SettingsStorage.DisablePartyHP == value) return;
                SettingsStorage.DisablePartyHP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                N(nameof(DisableHP));
            }
        }
        public bool ShowAwakenIcon
        {
            get => SettingsStorage.ShowAwakenIcon;
            set
            {
                if (SettingsStorage.ShowAwakenIcon == value) return;
                SettingsStorage.ShowAwakenIcon = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                N();
            }
        }

        public bool ShowItemsCooldown
        {
            get => SettingsStorage.ShowItemsCooldown;
            set
            {
                if (SettingsStorage.ShowItemsCooldown == value) return;
                SettingsStorage.ShowItemsCooldown = value;
                CooldownWindowViewModel.Instance.NotifyItemsDisplay();
                N();
            }
        }
        public bool UseLfg
        {
            get => SettingsStorage.LfgEnabled;
            set
            {
                if (SettingsStorage.LfgEnabled == value) return;
                SettingsStorage.LfgEnabled = value;
                N();
            }
        }
        public bool ShowFlightGauge
        {
            get => SettingsStorage.ShowFlightEnergy;
            set
            {
                if (SettingsStorage.ShowFlightEnergy == value) return;
                SettingsStorage.ShowFlightEnergy = value;
                N();
            }
        }
        public bool UseHotkeys
        {
            get => SettingsStorage.UseHotkeys;
            set
            {
                if (SettingsStorage.UseHotkeys == value) return;
                SettingsStorage.UseHotkeys = value;
                if (value) KeyboardHook.Instance.RegisterKeyboardHook();
                else KeyboardHook.Instance.UnRegisterKeyboardHook();
                N(nameof(UseHotkeys));
            }
        }
        public bool HideHandles
        {
            get => SettingsStorage.HideHandles;
            set
            {
                if (SettingsStorage.HideHandles == value) return;
                SettingsStorage.HideHandles = value;
                N(nameof(HideHandles));
            }
        }
        public bool ShowGroupWindowDetails
        {
            get => SettingsStorage.ShowGroupWindowDetails;
            set
            {
                if (SettingsStorage.ShowGroupWindowDetails == value) return;
                SettingsStorage.ShowGroupWindowDetails = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                N(nameof(ShowGroupWindowDetails));
            }
        }
        public bool ShowMembersLaurels
        {
            get => SettingsStorage.ShowMembersLaurels;
            set
            {
                if (SettingsStorage.ShowMembersLaurels == value) return;
                SettingsStorage.ShowMembersLaurels = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                N(nameof(ShowMembersLaurels));
            }
        }
        public bool AnimateChatMessages
        {
            get => SettingsStorage.AnimateChatMessages;
            set
            {
                if (SettingsStorage.AnimateChatMessages == value) return;
                SettingsStorage.AnimateChatMessages = value;
                N(nameof(AnimateChatMessages));
            }
        }
        public bool HhOnlyAggro
        {
            get => SettingsStorage.ShowOnlyAggroStacks;
            set
            {
                if (SettingsStorage.ShowOnlyAggroStacks == value) return;
                SettingsStorage.ShowOnlyAggroStacks = value;
                N(nameof(HhOnlyAggro));
            }
        }
        public double FlightGaugeRotation
        {
            get => SettingsStorage.FlightGaugeRotation;
            set
            {
                if (SettingsStorage.FlightGaugeRotation == value) return;
                SettingsStorage.FlightGaugeRotation = value;
                N(nameof(FlightGaugeRotation));
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlightGaugeRotation));
            }
        }

        public bool DiscordWebhookEnabled
        {
            get => SettingsStorage.DiscordWebhookEnabled;
            set
            {
                if (SettingsStorage.DiscordWebhookEnabled == value) return;
                SettingsStorage.DiscordWebhookEnabled = value;
                N();
            }
        }
        public bool ShowNotificationBubble
        {
            get => SettingsStorage.ShowNotificationBubble;
            set
            {
                if (SettingsStorage.ShowNotificationBubble == value) return;
                SettingsStorage.ShowNotificationBubble = value;
                N();
            }
        }
        public string Webhook
        {
            get => SettingsStorage.Webhook;
            set
            {
                if (value == SettingsStorage.Webhook) return;
                SettingsStorage.Webhook = value;
                N(nameof(Webhook));
            }
        }
        public string WebhookMessage
        {
            get => SettingsStorage.WebhookMessage;
            set
            {
                if (value == SettingsStorage.WebhookMessage) return;
                SettingsStorage.WebhookMessage = value;
                N(nameof(WebhookMessage));
            }
        }
        public string TwitchUsername
        {
            get => SettingsStorage.TwitchName;
            set
            {
                if (value == SettingsStorage.TwitchName) return;
                SettingsStorage.TwitchName = value;
                N(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => SettingsStorage.TwitchToken;
            set
            {
                if (value == SettingsStorage.TwitchToken) return;
                SettingsStorage.TwitchToken = value;
                N(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => SettingsStorage.TwitchChannelName;
            set
            {
                if (value == SettingsStorage.TwitchChannelName) return;
                SettingsStorage.TwitchChannelName = value;
                N(nameof(TwitchChannelName));
            }
        }
        public uint GroupSizeThreshold
        {
            get => SettingsStorage.GroupSizeThreshold;
            set
            {
                if (SettingsStorage.GroupSizeThreshold == value) return;
                SettingsStorage.GroupSizeThreshold = value;
                GroupWindowViewModel.Instance.NotifyThresholdChanged();
                N(nameof(GroupSizeThreshold));
            }
        }
        public bool Npcap
        {
            get => SettingsStorage.Npcap;
            set
            {
                if (SettingsStorage.Npcap == value) return;
                var res = TccMessageBox.Show("TCC", "TCC needs to be restarted to apply this setting. Restart now?",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.Cancel) return;
                SettingsStorage.Npcap = value;
                N();
                if(res == MessageBoxResult.OK) App.Restart();
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
            get => SettingsStorage.FontSize;
            set
            {
                if (SettingsStorage.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                SettingsStorage.FontSize = val;
                FontSizeChanged?.Invoke();
                N(nameof(FontSize));
            }
        }
        public SettingsWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
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

        public IEnumerable<ClickThruMode> ClickThruModes => Utils.ListFromEnum<ClickThruMode>();
        public IEnumerable<ClickThruMode> ChatClickThruModes => new List<ClickThruMode> { ClickThruMode.Never, ClickThruMode.GameDriven };
        public IEnumerable<CooldownBarMode> CooldownBarModes => Utils.ListFromEnum<CooldownBarMode>();
        public IEnumerable<FlowDirection> FlowDirections => Utils.ListFromEnum<FlowDirection>();
        public IEnumerable<EnrageLabelMode> EnrageLabelModes => Utils.ListFromEnum<EnrageLabelMode>();
        public IEnumerable<WarriorEdgeMode> WarriorEdgeModes => Utils.ListFromEnum<WarriorEdgeMode>();
        public IEnumerable<ControlShape> ControlShapes => Utils.ListFromEnum<ControlShape>();

        public bool ChatWindowEnabled
        {
            get => SettingsStorage.ChatEnabled;
            set
            {
                if (SettingsStorage.ChatEnabled == value) return;
                SettingsStorage.ChatEnabled = value;
                N();
            }
        }

        public ClickThruMode ChatClickThruMode
        {
            get => SettingsStorage.ChatClickThruMode;
            set
            {
                if (SettingsStorage.ChatClickThruMode == value) return;
                SettingsStorage.ChatClickThruMode = value;
                N();
            }
        }

        public bool ShowTradeLfgs
        {
            get => SettingsStorage.ShowTradeLfg;
            set
            {
                if (SettingsStorage.ShowTradeLfg == value) return;
                SettingsStorage.ShowTradeLfg = value;
                N();
            }
        }

        public bool CharacterWindowCompactMode
        {
            get => SettingsStorage.CharacterWindowCompactMode;
            set
            {
                if (SettingsStorage.CharacterWindowCompactMode == value) return;
                SettingsStorage.CharacterWindowCompactMode = value;
                N();
                CharacterWindowViewModel.Instance.ExN(nameof(CharacterWindowViewModel.CompactMode));
                WindowManager.CharacterWindow.Left = value ? WindowManager.CharacterWindow.Left + 175 :
                    WindowManager.CharacterWindow.Left - 175;

            }
        }

        public bool WarriorShowEdge
        {
            get => SettingsStorage.WarriorShowEdge;
            set
            {
                if (SettingsStorage.WarriorShowEdge == value) return;
                SettingsStorage.WarriorShowEdge = value;
                N();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExN(nameof(WarriorBarManager.ShowEdge));
            }
        }
        public bool SorcererReplacesElementsInCharWindow
        {
            get => SettingsStorage.SorcererReplacesElementsInCharWindow;
            set
            {
                if (SettingsStorage.SorcererReplacesElementsInCharWindow == value) return;
                SettingsStorage.SorcererReplacesElementsInCharWindow = value;
                N();
                CharacterWindowViewModel.Instance.ExN(nameof(CharacterWindowViewModel.ShowElements));
            }
        }

        public bool WarriorShowTraverseCut
        {
            get => SettingsStorage.WarriorShowTraverseCut;
            set
            {
                if (SettingsStorage.WarriorShowTraverseCut == value) return;
                SettingsStorage.WarriorShowTraverseCut = value;
                N();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExN(nameof(WarriorBarManager.ShowTraverseCut));
            }
        }

        public WarriorEdgeMode WarriorEdgeMode
        {
            get => SettingsStorage.WarriorEdgeMode;
            set
            {
                if (SettingsStorage.WarriorEdgeMode == value) return;
                SettingsStorage.WarriorEdgeMode = value;
                N();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExN(nameof(WarriorBarManager.WarriorEdgeMode));
            }

        }

        public bool FlipFlightGauge
        {
            get => SettingsStorage.FlipFlightGauge;
            set
            {
                if (SettingsStorage.FlipFlightGauge == value) return;
                SettingsStorage.FlipFlightGauge = value;
                N();
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlipFlightGauge));
            }
        }
        public bool ForceSoftwareRendering
        {
            get => SettingsStorage.ForceSoftwareRendering;
            set
            {
                if (SettingsStorage.ForceSoftwareRendering == value) return;
                SettingsStorage.ForceSoftwareRendering = value;
                N();
                RenderOptions.ProcessRenderMode = value ? RenderMode.SoftwareOnly : RenderMode.Default;
            }
        }

        public bool HighPriority
        {
            get => SettingsStorage.HighPriority;
            set
            {
                if (SettingsStorage.HighPriority == value) return;
                SettingsStorage.HighPriority = value;
                N();
                Process.GetCurrentProcess().PriorityClass = value ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            }
        }

    }
}
