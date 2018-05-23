using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;

namespace TCC.ViewModels
{
    public class SettingsWindowViewModel : TSPropertyChanged
    {
        public static event Action ChatShowChannelChanged;
        public static event Action ChatShowTimestampChanged;

        public WindowSettings CooldownWindowSettings => SettingsManager.CooldownWindowSettings;
        public WindowSettings ClassWindowSettings => SettingsManager.ClassWindowSettings;
        public WindowSettings GroupWindowSettings => SettingsManager.GroupWindowSettings;
        public WindowSettings BuffWindowSettings => SettingsManager.BuffWindowSettings;
        public WindowSettings CharacterWindowSettings => SettingsManager.CharacterWindowSettings;
        public WindowSettings BossWindowSettings => SettingsManager.BossWindowSettings;
        public WindowSettings FlightWindowSettings => SettingsManager.FlightGaugeWindowSettings;

        public bool HideMe
        {
            get => SettingsManager.IgnoreMeInGroupWindow;
            set
            {
                if (SettingsManager.IgnoreMeInGroupWindow == value) return;
                SettingsManager.IgnoreMeInGroupWindow = value;
                if (value == true) GroupWindowViewModel.Instance.RemoveMe();
                NPC("HideMe");
            }
        }
        public bool HideBuffs
        {
            get => SettingsManager.IgnoreGroupBuffs;
            set
            {
                if (SettingsManager.IgnoreGroupBuffs == value) return;
                SettingsManager.IgnoreGroupBuffs = value;
                NPC(nameof(HideBuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool HideDebuffs
        {
            get => SettingsManager.IgnoreGroupDebuffs;
            set
            {
                if (SettingsManager.IgnoreGroupDebuffs == value) return;
                SettingsManager.IgnoreGroupDebuffs = value;
                NPC(nameof(HideDebuffs));
                GroupWindowViewModel.Instance.NotifySettingUpdated();
            }
        }
        public bool DisableAllPartyAbnormals
        {
            get => SettingsManager.DisablePartyAbnormals;
            set
            {
                if (SettingsManager.DisablePartyAbnormals == value) return;
                SettingsManager.DisablePartyAbnormals = value;
                NPC(nameof(DisableAllPartyAbnormals));
                MessageFactory.Update();
                if (value == true) GroupWindowViewModel.Instance.ClearAllAbnormalities();
            }
        }

        public FlowDirection BuffsDirection
        {
            get => SettingsManager.BuffsDirection;
            set
            {
                if (SettingsManager.BuffsDirection == value) return;
                SettingsManager.BuffsDirection = value;
                BuffBarWindowViewModel.Instance.NotifyDirectionChanged();
                NPC(nameof(BuffsDirection));
            }
        }
        public CooldownBarMode CooldownBarMode
        {
            get => SettingsManager.CooldownBarMode;
            set
            {
                if (SettingsManager.CooldownBarMode == value) return;
                SettingsManager.CooldownBarMode = value;
                CooldownWindowViewModel.Instance.NotifyModeChanged();
                NPC(nameof(CooldownBarMode));
            }
        }
        public EnrageLabelMode EnrageLabelMode
        {
            get => SettingsManager.EnrageLabelMode;
            set
            {
                if (SettingsManager.EnrageLabelMode == value) return;
                SettingsManager.EnrageLabelMode = value;
                NPC(nameof(EnrageLabelMode));
            }
        }

        public bool ChatFadeOut
        {
            get => SettingsManager.ChatFadeOut;
            set
            {
                if (SettingsManager.ChatFadeOut == value) return;
                SettingsManager.ChatFadeOut = value;
                if (value) ChatWindowManager.Instance.RefreshTimer();
                NPC(nameof(ChatFadeOut));
            }
        }
        public string RegionOverride
        {
            get => SettingsManager.RegionOverride;
            set
            {
                if (SettingsManager.RegionOverride == value) return;
                SettingsManager.RegionOverride = value;
                NPC(nameof(RegionOverride));
            }
        }
        public int MaxMessages
        {
            get => SettingsManager.MaxMessages;
            set
            {
                if (SettingsManager.MaxMessages == value) return;
                var val = value;
                if (val < 20)
                {
                    val = 20;
                }
                SettingsManager.MaxMessages = val;
                NPC("MaxMessages");
            }
        }
        public int SpamThreshold
        {
            get => SettingsManager.SpamThreshold;
            set
            {
                if (SettingsManager.SpamThreshold == value) return;
                SettingsManager.SpamThreshold = value;
                NPC("SpamThreshold");
            }
        }
        public bool ShowTimestamp
        {
            get => SettingsManager.ShowTimestamp;
            set
            {
                if (SettingsManager.ShowTimestamp == value) return;
                SettingsManager.ShowTimestamp = value;
                NPC(nameof(ShowTimestamp));
                ChatShowTimestampChanged?.Invoke();
            }

        }
        public bool ShowChannel
        {
            get => SettingsManager.ShowChannel;
            set
            {
                if (SettingsManager.ShowChannel == value) return;
                SettingsManager.ShowChannel = value;
                ChatShowChannelChanged?.Invoke();
                NPC(nameof(ShowChannel));
            }

        }
        public bool ShowOnlyBosses
        {
            get => SettingsManager.ShowOnlyBosses;
            set
            {
                if (SettingsManager.ShowOnlyBosses == value) return;
                SettingsManager.ShowOnlyBosses = value;
                NPC(nameof(ShowOnlyBosses));
            }
        }
        public bool DisableMP
        {
            get => SettingsManager.DisablePartyMP;
            set
            {
                if (SettingsManager.DisablePartyMP == value) return;
                SettingsManager.DisablePartyMP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NPC(nameof(DisableMP));
            }
        }
        public bool DisableHP
        {
            get => SettingsManager.DisablePartyHP;
            set
            {
                if (SettingsManager.DisablePartyHP == value) return;
                SettingsManager.DisablePartyHP = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                MessageFactory.Update();
                NPC(nameof(DisableHP));
            }
        }

        public bool ShowItemsCooldown
        {
            get => SettingsManager.ShowItemsCooldown;
            set
            {
                if (SettingsManager.ShowItemsCooldown == value) return;
                SettingsManager.ShowItemsCooldown = value;
                CooldownWindowViewModel.Instance.NotifyItemsDisplay();
                NPC(nameof(ShowItemsCooldown));
            }
        }
        public bool UseLfg
        {
            get => SettingsManager.LfgEnabled;
            set
            {
                if (SettingsManager.LfgEnabled == value) return;
                SettingsManager.LfgEnabled = value;
                NPC();
            }
        }
        public bool ShowFlightGauge
        {
            get => SettingsManager.ShowFlightEnergy;
            set
            {
                if (SettingsManager.ShowFlightEnergy == value) return;
                SettingsManager.ShowFlightEnergy = value;
                NPC();
            }
        }
        public bool UseHotkeys
        {
            get => SettingsManager.UseHotkeys;
            set
            {
                if (SettingsManager.UseHotkeys == value) return;
                SettingsManager.UseHotkeys = value;
                if (value) KeyboardHook.Instance.RegisterKeyboardHook();
                else KeyboardHook.Instance.UnRegisterKeyboardHook();
                NPC(nameof(UseHotkeys));
            }
        }
        public bool ShowGroupWindowDetails
        {
            get => SettingsManager.ShowGroupWindowDetails;
            set
            {
                if (SettingsManager.ShowGroupWindowDetails == value) return;
                SettingsManager.ShowGroupWindowDetails = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC(nameof(ShowGroupWindowDetails));
            }
        }
        public bool ShowMembersLaurels
        {
            get => SettingsManager.ShowMembersLaurels;
            set
            {
                if (SettingsManager.ShowMembersLaurels == value) return;
                SettingsManager.ShowMembersLaurels = value;
                GroupWindowViewModel.Instance.NotifySettingUpdated();
                NPC(nameof(ShowMembersLaurels));
            }
        }
        public bool AnimateChatMessages
        {
            get => SettingsManager.AnimateChatMessages;
            set
            {
                if (SettingsManager.AnimateChatMessages == value) return;
                SettingsManager.AnimateChatMessages = value;
                NPC(nameof(AnimateChatMessages));
            }
        }
        public bool HhOnlyAggro
        {
            get => SettingsManager.ShowOnlyAggroStacks;
            set
            {
                if (SettingsManager.ShowOnlyAggroStacks == value) return;
                SettingsManager.ShowOnlyAggroStacks = value;
                NPC(nameof(HhOnlyAggro));
            }
        }
        //public bool LfgOn
        //{
        //    get => SettingsManager.LfgOn;
        //    set
        //    {
        //        if (SettingsManager.LfgOn == value) return;
        //        SettingsManager.LfgOn = value;
        //        ChatWindowManager.Instance.LfgOn = value;
        //        MessageFactory.Update();
        //        NotifyPropertyChanged(nameof(LfgOn));

        //    }
        //}
        public string Webhook
        {
            get => SettingsManager.Webhook;
            set
            {
                if (value == SettingsManager.Webhook) return;
                SettingsManager.Webhook = value;
                NPC(nameof(Webhook));
            }
        }
        public string WebhookMessage
        {
            get => SettingsManager.WebhookMessage;
            set
            {
                if (value == SettingsManager.WebhookMessage) return;
                SettingsManager.WebhookMessage = value;
                NPC(nameof(WebhookMessage));
            }
        }
        public string TwitchUsername
        {
            get => SettingsManager.TwitchName;
            set
            {
                if (value == SettingsManager.TwitchName) return;
                SettingsManager.TwitchName = value;
                NPC(nameof(TwitchUsername));
            }
        }
        public string TwitchToken
        {
            get => SettingsManager.TwitchToken;
            set
            {
                if (value == SettingsManager.TwitchToken) return;
                SettingsManager.TwitchToken = value;
                NPC(nameof(TwitchToken));
            }
        }
        public string TwitchChannelName
        {
            get => SettingsManager.TwitchChannelName;
            set
            {
                if (value == SettingsManager.TwitchChannelName) return;
                SettingsManager.TwitchChannelName = value;
                NPC(nameof(TwitchChannelName));
            }
        }
        public uint GroupSizeThreshold
        {
            get => SettingsManager.GroupSizeThreshold;
            set
            {
                if (SettingsManager.GroupSizeThreshold == value) return;
                SettingsManager.GroupSizeThreshold = value;
                GroupWindowViewModel.Instance.NotifyThresholdChanged();
                NPC(nameof(GroupSizeThreshold));
            }
        }

        public double ChatWindowOpacity
        {
            get => SettingsManager.ChatWindowOpacity;
            set
            {
                if (SettingsManager.ChatWindowOpacity == value) return;
                SettingsManager.ChatWindowOpacity = value;
                ChatWindowManager.Instance.NotifyOpacityChange();
                NPC(nameof(ChatWindowOpacity));
            }
        }
        public int FontSize
        {
            get => SettingsManager.FontSize;
            set
            {
                if (SettingsManager.FontSize == value) return;
                var val = value;
                if (val < 10) val = 10;
                SettingsManager.FontSize = val;
                NPC(nameof(FontSize));
            }
        }
        public SettingsWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //if (SettingsManager.CooldownWindowSettings.Enabled) WindowManager.CooldownWindow.PropertyChanged += CooldownWindow_PropertyChanged;
            //if (SettingsManager.CharacterWindowSettings.Enabled) WindowManager.CharacterWindow.PropertyChanged += CharacterWindow_PropertyChanged;
            //if (SettingsManager.BossWindowSettings.Enabled) WindowManager.BossWindow.PropertyChanged += BossGauge_PropertyChanged;
            //if (SettingsManager.BuffWindowSettings.Enabled) WindowManager.BuffWindow.PropertyChanged += BuffBar_PropertyChanged;
            //if (SettingsManager.GroupWindowSettings.Enabled) WindowManager.GroupWindow.PropertyChanged += GroupWindow_PropertyChanged;
            //if (SettingsManager.ClassWindowSettings.Enabled) WindowManager.ClassWindow.PropertyChanged += ClassWindow_PropertyChanged;
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
        public List<CooldownBarMode> CooldownBarModes => Utils.ListFromEnum<CooldownBarMode>();
        public List<FlowDirection> FlowDirections => Utils.ListFromEnum<FlowDirection>();
        public List<EnrageLabelMode> EnrageLabelModes => Utils.ListFromEnum<EnrageLabelMode>();

        public bool ChatWindowEnabled
        {
            get => SettingsManager.ChatEnabled;
            set
            {
                if (SettingsManager.ChatEnabled == value) return;
                SettingsManager.ChatEnabled = value;
                NPC();
            }
        }
        public bool ShowTradeLfgs
        {
            get => SettingsManager.ShowTradeLfg;
            set
            {
                if (SettingsManager.ShowTradeLfg == value) return;
                SettingsManager.ShowTradeLfg = value;
                NPC();
            }
        }
    }
}
