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

        public WindowSettings CooldownWindowSettings => SettingsHolder.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => SettingsHolder.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => SettingsHolder.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => SettingsHolder.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => SettingsHolder.CharacterWindowSettings;
        public WindowSettings BossWindowSettings => SettingsHolder.BossWindowSettings;
        public WindowSettings FlightWindowSettings => SettingsHolder.FlightGaugeWindowSettings;
        public WindowSettings FloatingButtonSettings => SettingsHolder.FloatingButtonSettings;
        public WindowSettings CuWindowSettings => SettingsHolder.CivilUnrestWindowSettings;

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
            get => SettingsHolder.EthicalMode;
            set
            {
                if (SettingsHolder.EthicalMode == value) return;
                SettingsHolder.EthicalMode = value;
                N();
            }
        }
        public bool CheckOpcodesHash
        {
            get => SettingsHolder.CheckOpcodesHash;
            set
            {
                if (SettingsHolder.CheckOpcodesHash == value) return;
                SettingsHolder.CheckOpcodesHash = value;
                N();
            }
        }
        public bool HideMe
        {
            get => SettingsHolder.IgnoreMeInGroupWindow;
            set
            {
                if (SettingsHolder.IgnoreMeInGroupWindow == value) return;
                SettingsHolder.IgnoreMeInGroupWindow = value;
                if (value) GroupWindowViewModel.Instance.RemoveMe();
                N();
            }
        }
        public bool HideBuffs
        {
            get => SettingsHolder.IgnoreGroupBuffs;
            set
            {
                if (SettingsHolder.IgnoreGroupBuffs == value) return;
                SettingsHolder.IgnoreGroupBuffs = value;
                N(nameof(HideBuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool HideDebuffs
        {
            get => SettingsHolder.IgnoreGroupDebuffs;
            set
            {
                if (SettingsHolder.IgnoreGroupDebuffs == value) return;
                SettingsHolder.IgnoreGroupDebuffs = value;
                N(nameof(HideDebuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool DisableAllPartyAbnormals
        {
            get => SettingsHolder.DisablePartyAbnormals;
            set
            {
                if (SettingsHolder.DisablePartyAbnormals == value) return;
                SettingsHolder.DisablePartyAbnormals = value;
                N(nameof(DisableAllPartyAbnormals));
                MessageFactory.Update();
                if (value) GroupWindowViewModel.Instance.ClearAllAbnormalities();
            }
        }
        public bool AccurateHp
        {
            get => SettingsHolder.AccurateHp;
            set
            {
                if (SettingsHolder.AccurateHp == value) return;
                SettingsHolder.AccurateHp = value;
                N(nameof(AccurateHp));
                MessageFactory.Update();
            }
        }

        public FlowDirection BuffsDirection
        {
            get => SettingsHolder.BuffsDirection;
            set
            {
                if (SettingsHolder.BuffsDirection == value) return;
                SettingsHolder.BuffsDirection = value;
                BuffBarWindowViewModel.Instance.ExN(nameof(BuffBarWindowViewModel.Direction));
                N(nameof(BuffsDirection));
            }
        }
        public ControlShape AbnormalityShape
        {
            get => SettingsHolder.AbnormalityShape;
            set
            {
                if (SettingsHolder.AbnormalityShape == value) return;
                SettingsHolder.AbnormalityShape = value;
                AbnormalityShapeChanged?.Invoke();
                N();
            }
        }
        public ControlShape SkillShape
        {
            get => SettingsHolder.SkillShape;
            set
            {
                if (SettingsHolder.SkillShape == value) return;
                SettingsHolder.SkillShape = value;
                SkillShapeChanged?.Invoke();
                N();
            }
        }
        public CooldownBarMode CooldownBarMode
        {
            get => SettingsHolder.CooldownBarMode;
            set
            {
                if (SettingsHolder.CooldownBarMode == value) return;
                SettingsHolder.CooldownBarMode = value;
                CooldownWindowViewModel.Instance.NotifyModeChanged();
                N(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => SettingsHolder.EnrageLabelMode;
            set
            {
                if (SettingsHolder.EnrageLabelMode == value) return;
                SettingsHolder.EnrageLabelMode = value;
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
            get => SettingsHolder.RegionOverride;
            set
            {
                if (SettingsHolder.RegionOverride == value) return;
                SettingsHolder.RegionOverride = value;
                N(nameof(RegionOverride));
            }
        }
        public int MaxMessages
        {
            get => SettingsHolder.MaxMessages;
            set
            {
                if (SettingsHolder.MaxMessages == value) return;
                var val = value;
                if (val < 20)
                {
                    val = 20;
                }
                SettingsHolder.MaxMessages = val;
                N();
            }
        }
        public int ChatScrollAmount
        {
            get => SettingsHolder.ChatScrollAmount;
            set
            {
                if (SettingsHolder.ChatScrollAmount == value) return;
                SettingsHolder.ChatScrollAmount = value;
                N();
            }
        }
        public int SpamThreshold
        {
            get => SettingsHolder.SpamThreshold;
            set
            {
                if (SettingsHolder.SpamThreshold == value) return;
                SettingsHolder.SpamThreshold = value;
                N();
            }
        }
        public bool ShowTimestamp
        {
            get => SettingsHolder.ShowTimestamp;
            set
            {
                if (SettingsHolder.ShowTimestamp == value) return;
                SettingsHolder.ShowTimestamp = value;
                N(nameof(ShowTimestamp));
                ChatShowTimestampChanged?.Invoke();
            }

        }
        public bool ShowChannel
        {
            get => SettingsHolder.ShowChannel;
            set
            {
                if (SettingsHolder.ShowChannel == value) return;
                SettingsHolder.ShowChannel = value;
                ChatShowChannelChanged?.Invoke();
                N(nameof(ShowChannel));
            }

        }
        public bool ShowOnlyBosses
        {
            get => SettingsHolder.ShowOnlyBosses;
            set
            {
                if (SettingsHolder.ShowOnlyBosses == value) return;
                SettingsHolder.ShowOnlyBosses = value;
                N(nameof(ShowOnlyBosses));
            }
        }
        public bool DisableMP
        {
            get => SettingsHolder.DisablePartyMP;
            set
            {
                if (SettingsHolder.DisablePartyMP == value) return;
                SettingsHolder.DisablePartyMP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                N(nameof(DisableMP));
            }
        }
        public bool DisableHP
        {
            get => SettingsHolder.DisablePartyHP;
            set
            {
                if (SettingsHolder.DisablePartyHP == value) return;
                SettingsHolder.DisablePartyHP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                N(nameof(DisableHP));
            }
        }
        public bool ShowAwakenIcon
        {
            get => SettingsHolder.ShowAwakenIcon;
            set
            {
                if (SettingsHolder.ShowAwakenIcon == value) return;
                SettingsHolder.ShowAwakenIcon = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                N();
            }
        }

        public bool ShowItemsCooldown
        {
            get => SettingsHolder.ShowItemsCooldown;
            set
            {
                if (SettingsHolder.ShowItemsCooldown == value) return;
                SettingsHolder.ShowItemsCooldown = value;
                CooldownWindowViewModel.Instance.NotifyItemsDisplay();
                N();
            }
        }
        public bool UseLfg
        {
            get => SettingsHolder.LfgEnabled;
            set
            {
                if (SettingsHolder.LfgEnabled == value) return;
                SettingsHolder.LfgEnabled = value;
                N();
            }
        }
        public bool ShowFlightGauge
        {
            get => SettingsHolder.ShowFlightEnergy;
            set
            {
                if (SettingsHolder.ShowFlightEnergy == value) return;
                SettingsHolder.ShowFlightEnergy = value;
                N();
            }
        }
        public bool UseHotkeys
        {
            get => SettingsHolder.UseHotkeys;
            set
            {
                if (SettingsHolder.UseHotkeys == value) return;
                SettingsHolder.UseHotkeys = value;
                if (value) KeyboardHook.Instance.RegisterKeyboardHook();
                else KeyboardHook.Instance.UnRegisterKeyboardHook();
                N(nameof(UseHotkeys));
            }
        }
        public bool HideHandles
        {
            get => SettingsHolder.HideHandles;
            set
            {
                if (SettingsHolder.HideHandles == value) return;
                SettingsHolder.HideHandles = value;
                N(nameof(HideHandles));
            }
        }
        public bool ShowGroupWindowDetails
        {
            get => SettingsHolder.ShowGroupWindowDetails;
            set
            {
                if (SettingsHolder.ShowGroupWindowDetails == value) return;
                SettingsHolder.ShowGroupWindowDetails = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                N(nameof(ShowGroupWindowDetails));
            }
        }
        public bool ShowMembersLaurels
        {
            get => SettingsHolder.ShowMembersLaurels;
            set
            {
                if (SettingsHolder.ShowMembersLaurels == value) return;
                SettingsHolder.ShowMembersLaurels = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                N(nameof(ShowMembersLaurels));
            }
        }
        public bool AnimateChatMessages
        {
            get => SettingsHolder.AnimateChatMessages;
            set
            {
                if (SettingsHolder.AnimateChatMessages == value) return;
                SettingsHolder.AnimateChatMessages = value;
                N(nameof(AnimateChatMessages));
            }
        }
        public bool HhOnlyAggro
        {
            get => SettingsHolder.ShowOnlyAggroStacks;
            set
            {
                if (SettingsHolder.ShowOnlyAggroStacks == value) return;
                SettingsHolder.ShowOnlyAggroStacks = value;
                N(nameof(HhOnlyAggro));
            }
        }
        public double FlightGaugeRotation
        {
            get => SettingsHolder.FlightGaugeRotation;
            set
            {
                if (SettingsHolder.FlightGaugeRotation == value) return;
                SettingsHolder.FlightGaugeRotation = value;
                N(nameof(FlightGaugeRotation));
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlightGaugeRotation));
            }
        }

        public bool DiscordWebhookEnabled
        {
            get => SettingsHolder.DiscordWebhookEnabled;
            set
            {
                if (SettingsHolder.DiscordWebhookEnabled == value) return;
                SettingsHolder.DiscordWebhookEnabled = value;
                N();
            }
        }
        public bool ShowNotificationBubble
        {
            get => SettingsHolder.ShowNotificationBubble;
            set
            {
                if (SettingsHolder.ShowNotificationBubble == value) return;
                SettingsHolder.ShowNotificationBubble = value;
                N();
            }
        }
        public string Webhook
        {
            get => SettingsHolder.Webhook;
            set
            {
                if (value == SettingsHolder.Webhook) return;
                SettingsHolder.Webhook = value;
                N(nameof(Webhook));
            }
        }
        public string WebhookMessage
        {
            get => SettingsHolder.WebhookMessage;
            set
            {
                if (value == SettingsHolder.WebhookMessage) return;
                SettingsHolder.WebhookMessage = value;
                N(nameof(WebhookMessage));
            }
        }
        public string TwitchUsername
        {
            get => SettingsHolder.TwitchName;
            set
            {
                if (value == SettingsHolder.TwitchName) return;
                SettingsHolder.TwitchName = value;
                N(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => SettingsHolder.TwitchToken;
            set
            {
                if (value == SettingsHolder.TwitchToken) return;
                SettingsHolder.TwitchToken = value;
                N(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => SettingsHolder.TwitchChannelName;
            set
            {
                if (value == SettingsHolder.TwitchChannelName) return;
                SettingsHolder.TwitchChannelName = value;
                N(nameof(TwitchChannelName));
            }
        }
        public uint GroupSizeThreshold
        {
            get => SettingsHolder.GroupSizeThreshold;
            set
            {
                if (SettingsHolder.GroupSizeThreshold == value) return;
                SettingsHolder.GroupSizeThreshold = value;
                GroupWindowViewModel.Instance.NotifyThresholdChanged();
                N(nameof(GroupSizeThreshold));
            }
        }
        public bool Npcap
        {
            get => SettingsHolder.Npcap;
            set
            {
                if (SettingsHolder.Npcap == value) return;
                var res = TccMessageBox.Show("TCC", "TCC needs to be restarted to apply this setting. Restart now?",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.Cancel) return;
                SettingsHolder.Npcap = value;
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
            get => SettingsHolder.FontSize;
            set
            {
                if (SettingsHolder.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                SettingsHolder.FontSize = val;
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
            get => SettingsHolder.ChatEnabled;
            set
            {
                if (SettingsHolder.ChatEnabled == value) return;
                SettingsHolder.ChatEnabled = value;
                N();
            }
        }

        public ClickThruMode ChatClickThruMode
        {
            get => SettingsHolder.ChatClickThruMode;
            set
            {
                if (SettingsHolder.ChatClickThruMode == value) return;
                SettingsHolder.ChatClickThruMode = value;
                N();
            }
        }

        public bool ShowTradeLfgs
        {
            get => SettingsHolder.ShowTradeLfg;
            set
            {
                if (SettingsHolder.ShowTradeLfg == value) return;
                SettingsHolder.ShowTradeLfg = value;
                N();
            }
        }

        public bool CharacterWindowCompactMode
        {
            get => SettingsHolder.CharacterWindowCompactMode;
            set
            {
                if (SettingsHolder.CharacterWindowCompactMode == value) return;
                SettingsHolder.CharacterWindowCompactMode = value;
                N();
                CharacterWindowViewModel.Instance.ExN(nameof(CharacterWindowViewModel.CompactMode));
                WindowManager.CharacterWindow.Left = value ? WindowManager.CharacterWindow.Left + 175 :
                    WindowManager.CharacterWindow.Left - 175;

            }
        }

        public bool WarriorShowEdge
        {
            get => SettingsHolder.WarriorShowEdge;
            set
            {
                if (SettingsHolder.WarriorShowEdge == value) return;
                SettingsHolder.WarriorShowEdge = value;
                N();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExN(nameof(WarriorBarManager.ShowEdge));
            }
        }
        public bool SorcererReplacesElementsInCharWindow
        {
            get => SettingsHolder.SorcererReplacesElementsInCharWindow;
            set
            {
                if (SettingsHolder.SorcererReplacesElementsInCharWindow == value) return;
                SettingsHolder.SorcererReplacesElementsInCharWindow = value;
                N();
                CharacterWindowViewModel.Instance.ExN(nameof(CharacterWindowViewModel.ShowElements));
            }
        }

        public bool WarriorShowTraverseCut
        {
            get => SettingsHolder.WarriorShowTraverseCut;
            set
            {
                if (SettingsHolder.WarriorShowTraverseCut == value) return;
                SettingsHolder.WarriorShowTraverseCut = value;
                N();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExN(nameof(WarriorBarManager.ShowTraverseCut));
            }
        }

        public WarriorEdgeMode WarriorEdgeMode
        {
            get => SettingsHolder.WarriorEdgeMode;
            set
            {
                if (SettingsHolder.WarriorEdgeMode == value) return;
                SettingsHolder.WarriorEdgeMode = value;
                N();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExN(nameof(WarriorBarManager.WarriorEdgeMode));
            }

        }

        public bool FlipFlightGauge
        {
            get => SettingsHolder.FlipFlightGauge;
            set
            {
                if (SettingsHolder.FlipFlightGauge == value) return;
                SettingsHolder.FlipFlightGauge = value;
                N();
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlipFlightGauge));
            }
        }
        public bool ForceSoftwareRendering
        {
            get => SettingsHolder.ForceSoftwareRendering;
            set
            {
                if (SettingsHolder.ForceSoftwareRendering == value) return;
                SettingsHolder.ForceSoftwareRendering = value;
                N();
                RenderOptions.ProcessRenderMode = value ? RenderMode.SoftwareOnly : RenderMode.Default;
            }
        }

        public bool HighPriority
        {
            get => SettingsHolder.HighPriority;
            set
            {
                if (SettingsHolder.HighPriority == value) return;
                SettingsHolder.HighPriority = value;
                N();
                Process.GetCurrentProcess().PriorityClass = value ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            }
        }

    }
}
