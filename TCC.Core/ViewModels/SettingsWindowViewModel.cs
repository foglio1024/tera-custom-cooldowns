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

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        public static event Action ChatShowChannelChanged;
        public static event Action ChatShowTimestampChanged;
        public static event Action AbnormalityShapeChanged;
        public static event Action SkillShapeChanged;
        public static event Action FontSizeChanged;

        public WindowSettings CooldownWindowSettings => Settings.Settings.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => Settings.Settings.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => Settings.Settings.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => Settings.Settings.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => Settings.Settings.CharacterWindowSettings;
        public WindowSettings BossWindowSettings => Settings.Settings.BossWindowSettings;
        public WindowSettings FlightWindowSettings => Settings.Settings.FlightGaugeWindowSettings;
        public WindowSettings FloatingButtonSettings => Settings.Settings.FloatingButtonSettings;
        public WindowSettings CuWindowSettings => Settings.Settings.CivilUnrestWindowSettings;

        private int khCount = 0;
        private bool _kh;
        public bool KylosHelper
        {
            get => _kh;
            set
            {
                _kh = true;
                switch (khCount)
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
                NPC();

                khCount++;
                if (khCount > 2) khCount = 0;
                _kh = false;
                NPC();
            }
        }

        public bool EthicalMode
        {
            get => Settings.Settings.EthicalMode;
            set
            {
                if (Settings.Settings.EthicalMode == value) return;
                Settings.Settings.EthicalMode = value;
                NPC();
            }
        }
        public bool HideMe
        {
            get => Settings.Settings.IgnoreMeInGroupWindow;
            set
            {
                if (Settings.Settings.IgnoreMeInGroupWindow == value) return;
                Settings.Settings.IgnoreMeInGroupWindow = value;
                if (value) GroupWindowViewModel.Instance.RemoveMe();
                NPC();
            }
        }
        public bool HideBuffs
        {
            get => Settings.Settings.IgnoreGroupBuffs;
            set
            {
                if (Settings.Settings.IgnoreGroupBuffs == value) return;
                Settings.Settings.IgnoreGroupBuffs = value;
                NPC(nameof(HideBuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool HideDebuffs
        {
            get => Settings.Settings.IgnoreGroupDebuffs;
            set
            {
                if (Settings.Settings.IgnoreGroupDebuffs == value) return;
                Settings.Settings.IgnoreGroupDebuffs = value;
                NPC(nameof(HideDebuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool DisableAllPartyAbnormals
        {
            get => Settings.Settings.DisablePartyAbnormals;
            set
            {
                if (Settings.Settings.DisablePartyAbnormals == value) return;
                Settings.Settings.DisablePartyAbnormals = value;
                NPC(nameof(DisableAllPartyAbnormals));
                MessageFactory.Update();
                if (value) GroupWindowViewModel.Instance.ClearAllAbnormalities();
            }
        }
        public bool AccurateHp
        {
            get => Settings.Settings.AccurateHp;
            set
            {
                if (Settings.Settings.AccurateHp == value) return;
                Settings.Settings.AccurateHp = value;
                NPC(nameof(AccurateHp));
                MessageFactory.Update();
            }
        }

        public FlowDirection BuffsDirection
        {
            get => Settings.Settings.BuffsDirection;
            set
            {
                if (Settings.Settings.BuffsDirection == value) return;
                Settings.Settings.BuffsDirection = value;
                BuffBarWindowViewModel.Instance.ExNPC(nameof(BuffBarWindowViewModel.Direction));
                NPC(nameof(BuffsDirection));
            }
        }
        public ControlShape AbnormalityShape
        {
            get => Settings.Settings.AbnormalityShape;
            set
            {
                if (Settings.Settings.AbnormalityShape == value) return;
                Settings.Settings.AbnormalityShape = value;
                AbnormalityShapeChanged?.Invoke();
                NPC();
            }
        }
        public ControlShape SkillShape
        {
            get => Settings.Settings.SkillShape;
            set
            {
                if (Settings.Settings.SkillShape == value) return;
                Settings.Settings.SkillShape = value;
                SkillShapeChanged?.Invoke();
                NPC();
            }
        }
        public CooldownBarMode CooldownBarMode
        {
            get => Settings.Settings.CooldownBarMode;
            set
            {
                if (Settings.Settings.CooldownBarMode == value) return;
                Settings.Settings.CooldownBarMode = value;
                CooldownWindowViewModel.Instance.NotifyModeChanged();
                NPC(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => Settings.Settings.EnrageLabelMode;
            set
            {
                if (Settings.Settings.EnrageLabelMode == value) return;
                Settings.Settings.EnrageLabelMode = value;
                NPC(nameof(EnrageLabelMode));
            }
        }

        public bool ChatFadeOut
        {
            get => Settings.Settings.ChatFadeOut;
            set
            {
                if (Settings.Settings.ChatFadeOut == value) return;
                Settings.Settings.ChatFadeOut = value;
                if (value) ChatWindowManager.Instance.RefreshHideTimer();
                NPC(nameof(ChatFadeOut));
            }
        }
        public string RegionOverride
        {
            get => Settings.Settings.RegionOverride;
            set
            {
                if (Settings.Settings.RegionOverride == value) return;
                Settings.Settings.RegionOverride = value;
                NPC(nameof(RegionOverride));
            }
        }
        public int MaxMessages
        {
            get => Settings.Settings.MaxMessages;
            set
            {
                if (Settings.Settings.MaxMessages == value) return;
                var val = value;
                if (val < 20)
                {
                    val = 20;
                }
                Settings.Settings.MaxMessages = val;
                NPC();
            }
        }
        public int SpamThreshold
        {
            get => Settings.Settings.SpamThreshold;
            set
            {
                if (Settings.Settings.SpamThreshold == value) return;
                Settings.Settings.SpamThreshold = value;
                NPC();
            }
        }
        public bool ShowTimestamp
        {
            get => Settings.Settings.ShowTimestamp;
            set
            {
                if (Settings.Settings.ShowTimestamp == value) return;
                Settings.Settings.ShowTimestamp = value;
                NPC(nameof(ShowTimestamp));
                ChatShowTimestampChanged?.Invoke();
            }

        }
        public bool ShowChannel
        {
            get => Settings.Settings.ShowChannel;
            set
            {
                if (Settings.Settings.ShowChannel == value) return;
                Settings.Settings.ShowChannel = value;
                ChatShowChannelChanged?.Invoke();
                NPC(nameof(ShowChannel));
            }

        }
        public bool ShowOnlyBosses
        {
            get => Settings.Settings.ShowOnlyBosses;
            set
            {
                if (Settings.Settings.ShowOnlyBosses == value) return;
                Settings.Settings.ShowOnlyBosses = value;
                NPC(nameof(ShowOnlyBosses));
            }
        }
        public bool DisableMP
        {
            get => Settings.Settings.DisablePartyMP;
            set
            {
                if (Settings.Settings.DisablePartyMP == value) return;
                Settings.Settings.DisablePartyMP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NPC(nameof(DisableMP));
            }
        }
        public bool DisableHP
        {
            get => Settings.Settings.DisablePartyHP;
            set
            {
                if (Settings.Settings.DisablePartyHP == value) return;
                Settings.Settings.DisablePartyHP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NPC(nameof(DisableHP));
            }
        }
        public bool ShowAwakenIcon
        {
            get => Settings.Settings.ShowAwakenIcon;
            set
            {
                if (Settings.Settings.ShowAwakenIcon == value) return;
                Settings.Settings.ShowAwakenIcon = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC();
            }
        }

        public bool ShowItemsCooldown
        {
            get => Settings.Settings.ShowItemsCooldown;
            set
            {
                if (Settings.Settings.ShowItemsCooldown == value) return;
                Settings.Settings.ShowItemsCooldown = value;
                CooldownWindowViewModel.Instance.NotifyItemsDisplay();
                NPC(nameof(ShowItemsCooldown));
            }
        }
        public bool UseLfg
        {
            get => Settings.Settings.LfgEnabled;
            set
            {
                if (Settings.Settings.LfgEnabled == value) return;
                Settings.Settings.LfgEnabled = value;
                NPC();
            }
        }
        public bool ShowFlightGauge
        {
            get => Settings.Settings.ShowFlightEnergy;
            set
            {
                if (Settings.Settings.ShowFlightEnergy == value) return;
                Settings.Settings.ShowFlightEnergy = value;
                NPC();
            }
        }
        public bool UseHotkeys
        {
            get => Settings.Settings.UseHotkeys;
            set
            {
                if (Settings.Settings.UseHotkeys == value) return;
                Settings.Settings.UseHotkeys = value;
                if (value) KeyboardHook.Instance.RegisterKeyboardHook();
                else KeyboardHook.Instance.UnRegisterKeyboardHook();
                NPC(nameof(UseHotkeys));
            }
        }
        public bool HideHandles
        {
            get => Settings.Settings.HideHandles;
            set
            {
                if (Settings.Settings.HideHandles == value) return;
                Settings.Settings.HideHandles = value;
                NPC(nameof(HideHandles));
            }
        }
        public bool ShowGroupWindowDetails
        {
            get => Settings.Settings.ShowGroupWindowDetails;
            set
            {
                if (Settings.Settings.ShowGroupWindowDetails == value) return;
                Settings.Settings.ShowGroupWindowDetails = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC(nameof(ShowGroupWindowDetails));
            }
        }
        public bool ShowMembersLaurels
        {
            get => Settings.Settings.ShowMembersLaurels;
            set
            {
                if (Settings.Settings.ShowMembersLaurels == value) return;
                Settings.Settings.ShowMembersLaurels = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC(nameof(ShowMembersLaurels));
            }
        }
        public bool AnimateChatMessages
        {
            get => Settings.Settings.AnimateChatMessages;
            set
            {
                if (Settings.Settings.AnimateChatMessages == value) return;
                Settings.Settings.AnimateChatMessages = value;
                NPC(nameof(AnimateChatMessages));
            }
        }
        public bool HhOnlyAggro
        {
            get => Settings.Settings.ShowOnlyAggroStacks;
            set
            {
                if (Settings.Settings.ShowOnlyAggroStacks == value) return;
                Settings.Settings.ShowOnlyAggroStacks = value;
                NPC(nameof(HhOnlyAggro));
            }
        }
        public double FlightGaugeRotation
        {
            get => Settings.Settings.FlightGaugeRotation;
            set
            {
                if (Settings.Settings.FlightGaugeRotation == value) return;
                Settings.Settings.FlightGaugeRotation = value;
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
            get => Settings.Settings.Webhook;
            set
            {
                if (value == Settings.Settings.Webhook) return;
                Settings.Settings.Webhook = value;
                NPC(nameof(Webhook));
            }
        }
        public string WebhookMessage
        {
            get => Settings.Settings.WebhookMessage;
            set
            {
                if (value == Settings.Settings.WebhookMessage) return;
                Settings.Settings.WebhookMessage = value;
                NPC(nameof(WebhookMessage));
            }
        }
        public string TwitchUsername
        {
            get => Settings.Settings.TwitchName;
            set
            {
                if (value == Settings.Settings.TwitchName) return;
                Settings.Settings.TwitchName = value;
                NPC(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => Settings.Settings.TwitchToken;
            set
            {
                if (value == Settings.Settings.TwitchToken) return;
                Settings.Settings.TwitchToken = value;
                NPC(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => Settings.Settings.TwitchChannelName;
            set
            {
                if (value == Settings.Settings.TwitchChannelName) return;
                Settings.Settings.TwitchChannelName = value;
                NPC(nameof(TwitchChannelName));
            }
        }
        public uint GroupSizeThreshold
        {
            get => Settings.Settings.GroupSizeThreshold;
            set
            {
                if (Settings.Settings.GroupSizeThreshold == value) return;
                Settings.Settings.GroupSizeThreshold = value;
                GroupWindowViewModel.Instance.NotifyThresholdChanged();
                NPC(nameof(GroupSizeThreshold));
            }
        }

        public double ChatWindowOpacity
        {
            get => Settings.Settings.ChatWindowOpacity;
            set
            {
                if (Settings.Settings.ChatWindowOpacity == value) return;
                Settings.Settings.ChatWindowOpacity = value;
                ChatWindowManager.Instance.NotifyOpacityChange();
                NPC(nameof(ChatWindowOpacity));
            }
        }
        public int FontSize
        {
            get => Settings.Settings.FontSize;
            set
            {
                if (Settings.Settings.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                Settings.Settings.FontSize = val;
                FontSizeChanged?.Invoke();
                NPC(nameof(FontSize));
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

        public List<ClickThruMode> ClickThruModes => Utils.ListFromEnum<ClickThruMode>();
        public List<ClickThruMode> ChatClickThruModes => new List<ClickThruMode> { ClickThruMode.Never, ClickThruMode.GameDriven };
        public List<CooldownBarMode> CooldownBarModes => Utils.ListFromEnum<CooldownBarMode>();
        public List<FlowDirection> FlowDirections => Utils.ListFromEnum<FlowDirection>();
        public List<EnrageLabelMode> EnrageLabelModes => Utils.ListFromEnum<EnrageLabelMode>();
        public List<WarriorEdgeMode> WarriorEdgeModes => Utils.ListFromEnum<WarriorEdgeMode>();
        public List<ControlShape> ControlShapes => Utils.ListFromEnum<ControlShape>();

        public bool ChatWindowEnabled
        {
            get => Settings.Settings.ChatEnabled;
            set
            {
                if (Settings.Settings.ChatEnabled == value) return;
                Settings.Settings.ChatEnabled = value;
                NPC();
            }
        }

        public ClickThruMode ChatClickThruMode
        {
            get => Settings.Settings.ChatClickThruMode;
            set
            {
                if (Settings.Settings.ChatClickThruMode == value) return;
                Settings.Settings.ChatClickThruMode = value;
                NPC();
            }
        }

        public bool ShowTradeLfgs
        {
            get => Settings.Settings.ShowTradeLfg;
            set
            {
                if (Settings.Settings.ShowTradeLfg == value) return;
                Settings.Settings.ShowTradeLfg = value;
                NPC();
            }
        }

        public bool CharacterWindowCompactMode
        {
            get => Settings.Settings.CharacterWindowCompactMode;
            set
            {
                if (Settings.Settings.CharacterWindowCompactMode == value) return;
                Settings.Settings.CharacterWindowCompactMode = value;
                NPC();
                CharacterWindowViewModel.Instance.ExNPC(nameof(CharacterWindowViewModel.CompactMode));
                WindowManager.CharacterWindow.Left = value ? WindowManager.CharacterWindow.Left + 175 :
                    WindowManager.CharacterWindow.Left - 175;

            }
        }

        public bool WarriorShowEdge
        {
            get => Settings.Settings.WarriorShowEdge;
            set
            {
                if (Settings.Settings.WarriorShowEdge == value) return;
                Settings.Settings.WarriorShowEdge = value;
                NPC();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExNPC(nameof(WarriorBarManager.ShowEdge));
            }
        }
        public bool SorcererReplacesElementsInCharWindow
        {
            get => Settings.Settings.SorcererReplacesElementsInCharWindow;
            set
            {
                if (Settings.Settings.SorcererReplacesElementsInCharWindow == value) return;
                Settings.Settings.SorcererReplacesElementsInCharWindow = value;
                NPC();
                CharacterWindowViewModel.Instance.ExNPC(nameof(CharacterWindowViewModel.ShowElements));
            }
        }

        public bool WarriorShowTraverseCut
        {
            get => Settings.Settings.WarriorShowTraverseCut;
            set
            {
                if (Settings.Settings.WarriorShowTraverseCut == value) return;
                Settings.Settings.WarriorShowTraverseCut = value;
                NPC();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExNPC(nameof(WarriorBarManager.ShowTraverseCut));
            }
        }

        public WarriorEdgeMode WarriorEdgeMode
        {
            get => Settings.Settings.WarriorEdgeMode;
            set
            {
                if (Settings.Settings.WarriorEdgeMode == value) return;
                Settings.Settings.WarriorEdgeMode = value;
                NPC();
                if (ClassWindowViewModel.Instance.CurrentManager is WarriorBarManager wm) wm.ExNPC(nameof(WarriorBarManager.WarriorEdgeMode));
            }

        }

        public bool FlipFlightGauge
        {
            get => Settings.Settings.FlipFlightGauge;
            set
            {
                if (Settings.Settings.FlipFlightGauge == value) return;
                Settings.Settings.FlipFlightGauge = value;
                NPC();
                WindowManager.FlightDurationWindow.ExNPC(nameof(FlipFlightGauge));
            }
        }
        public bool ForceSoftwareRendering
        {
            get => Settings.Settings.ForceSoftwareRendering;
            set
            {
                if (Settings.Settings.ForceSoftwareRendering == value) return;
                Settings.Settings.ForceSoftwareRendering = value;
                NPC();
                RenderOptions.ProcessRenderMode = value ? RenderMode.SoftwareOnly : RenderMode.Default;
            }
        }

        public bool HighPriority
        {
            get => Settings.Settings.HighPriority;
            set
            {
                if (Settings.Settings.HighPriority == value) return;
                Settings.Settings.HighPriority = value;
                NPC();
                Process.GetCurrentProcess().PriorityClass = value ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            }
        }

    }
}
