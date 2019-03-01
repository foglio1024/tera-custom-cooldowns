using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Data.Pc;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Controls.Group
{

    public class GroupMemberBase : UserControl, INotifyPropertyChanged
    {
        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private DataTemplateSelector _abnormalityDataTemplateSelector;
        protected DataTemplateSelector AbnormalityDataTemplateSelector
        {
            get => _abnormalityDataTemplateSelector;
            set
            {
                CurrentAbnormalityTemplateSelector = value;
            }

        }

        private DataTemplateSelector _currentAbnormalityTemplateSelector;

        public bool ShowHp => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideHpThreshold;
        public bool ShowMp => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideMpThreshold;
        public bool ShowBuffs => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideBuffsThreshold;
        public bool ShowDebuffs => WindowManager.GroupWindow.VM.Size <= SettingsHolder.HideDebuffsThreshold;
        public bool ShowLaurel => SettingsHolder.ShowMembersLaurels;
        public bool ShowAwaken => SettingsHolder.ShowAwakenIcon;
        public DataTemplateSelector CurrentAbnormalityTemplateSelector
        {
            get => _currentAbnormalityTemplateSelector;
            protected set
            {
                if (_currentAbnormalityTemplateSelector == value) return;
                _currentAbnormalityTemplateSelector = value;
                NPC();
            }
        }
        public IEnumerable BuffsSource => ShowBuffs ? (DataContext as User)?.Buffs : null;
        public IEnumerable DebuffsSource => ShowDebuffs ? (DataContext as User)?.Debuffs : null;

        public GroupMemberBase()
        {
            Loaded += (_, __) =>
            {
                UpdateSettings();
                WindowManager.GroupWindow.VM.SettingsUpdated += UpdateSettings;
                WindowManager.GroupWindow.VM.PropertyChanged += (___, args) => { if (args.PropertyName == nameof(GroupWindowViewModel.Size)) UpdateSettings(); };
                SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
            };
            Unloaded += (_, __) => SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged;
        }

        private void UpdateSettings()
        {
            NPC(nameof(ShowHp));
            NPC(nameof(ShowMp));
            NPC(nameof(ShowBuffs));
            NPC(nameof(ShowDebuffs));
            NPC(nameof(BuffsSource));
            NPC(nameof(DebuffsSource));
            NPC(nameof(ShowLaurel));
            NPC(nameof(ShowAwaken));
        }
        private void OnAbnormalityShapeChanged()
        {
            CurrentAbnormalityTemplateSelector = null;
            CurrentAbnormalityTemplateSelector = AbnormalityDataTemplateSelector;
        }
        private void AnimateIn()
        {
            var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            BeginAnimation(OpacityProperty, an);
        }
        private void AnimateOut()
        {
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            BeginAnimation(OpacityProperty, an);
        }
        protected void ShowUserMenu(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            Proxy.Proxy.AskInteractive(dc.ServerId, dc.Name);
        }
        protected void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = true;
        }
        protected void ToolTip_OnClosed(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = false;
        }
    }

    public partial class PartyMember : GroupMemberBase //TODO: make base class for this when???
    {
        public PartyMember()
        {
            AbnormalityDataTemplateSelector = R.TemplateSelectors.PartyAbnormalityTemplateSelector;
            InitializeComponent();
            //Unloaded += (_, __) => { SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged; };
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    UpdateSettings();
        //    AnimateIn();
        //    WindowManager.GroupWindow.VM.SettingsUpdated += UpdateSettings;
        //    SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        //}

        //private void OnAbnormalityShapeChanged()
        //{
        //    Buffs.RefreshTemplate(R.TemplateSelectors.PartyAbnormalityTemplateSelector);
        //    Debuffs.RefreshTemplate(R.TemplateSelectors.PartyAbnormalityTemplateSelector);
        //}
        //private void AnimateIn()
        //{
        //    var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
        //    BeginAnimation(OpacityProperty, an);
        //}

        //private void UpdateSettings()
        //{
        //    SetMp();
        //    SetHp();
        //    SetBuffs();
        //    SetDebuffs();
        //    SetLaurels();
        //    SetAwakenIcon();
        //}


        //private void SetAwakenIcon()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        if (!(DataContext is User user)) return;
        //        AwakenIcon.Visibility = TCC.Settings.SettingsHolder.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) :
        //            Visibility.Collapsed;
        //        AwakenBorder.Visibility = TCC.Settings.SettingsHolder.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) :
        //            Visibility.Collapsed;
        //    });
        //}

        //private void SetLaurels()
        //{
        //    try
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            LaurelImage.Visibility = TCC.Settings.SettingsHolder.ShowMembersLaurels ? Visibility.Visible : Visibility.Collapsed;
        //        });
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //}
        //private void SetBuffs()
        //{
        //    try
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            if (!(DataContext is User user)) return;
        //            Buffs.ItemsSource = TCC.Settings.SettingsHolder.IgnoreGroupBuffs ? null : user.Buffs;
        //            BuffGrid.Visibility = TCC.Settings.SettingsHolder.IgnoreGroupBuffs
        //                ? Visibility.Collapsed
        //                : Visibility.Visible;
        //        });
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //}
        //private void SetDebuffs()
        //{
        //    try
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            if (!(DataContext is User)) return;
        //            Debuffs.ItemsSource = TCC.Settings.SettingsHolder.IgnoreGroupDebuffs ? null : ((User)DataContext).Debuffs;
        //            DebuffGrid.Visibility = TCC.Settings.SettingsHolder.IgnoreGroupDebuffs
        //                ? Visibility.Collapsed
        //                : Visibility.Visible;
        //        });
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //}
        //private void SetMp()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        MpBar.Visibility = !TCC.Settings.SettingsHolder.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
        //    });
        //}
        //private void SetHp()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        HpBar.Visibility = !SettingsHolder.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
        //    });
        //}


        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //}

        //private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var dc = (User)DataContext;
        //    Proxy.Proxy.AskInteractive(dc.ServerId, dc.Name);
        //}

        //private void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        //{
        //    FocusManager.PauseTopmost = true;
        //}

        //private void ToolTip_OnClosed(object sender, RoutedEventArgs e)
        //{
        //    FocusManager.PauseTopmost = false;
        //}
    }
}
