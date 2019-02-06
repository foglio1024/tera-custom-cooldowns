using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Data.Pc;
using TCC.ViewModels;

namespace TCC.Controls.Group
{
    public partial class PartyMember //TODO: make base class for this when???
    {
        public PartyMember()
        {
            InitializeComponent();
            Unloaded += (_, __) => { SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged; };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSettings();
            AnimateIn();
            WindowManager.GroupWindow.VM.SettingsUpdated += UpdateSettings;
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }

        private void OnAbnormalityShapeChanged()
        {
            Buffs.RefreshTemplate(R.TemplateSelectors.PartyAbnormalityTemplateSelector);
            Debuffs.RefreshTemplate(R.TemplateSelectors.PartyAbnormalityTemplateSelector);
        }

        private void UpdateSettings()
        {
            SetMp();
            SetHp();
            SetBuffs();
            SetDebuffs();
            SetLaurels();
            SetAwakenIcon();
        }


        private void SetAwakenIcon()
        {
            Dispatcher.Invoke(() =>
            {
                if (!(DataContext is User user)) return;
                AwakenIcon.Visibility = TCC.Settings.SettingsHolder.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) :
                    Visibility.Collapsed;
                AwakenBorder.Visibility = TCC.Settings.SettingsHolder.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) :
                    Visibility.Collapsed;
            });
        }

        private void SetLaurels()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LaurelImage.Visibility = TCC.Settings.SettingsHolder.ShowMembersLaurels ? Visibility.Visible : Visibility.Collapsed;
                });
            }
            catch
            {
                // ignored
            }
        }
        private void SetBuffs()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (!(DataContext is User user)) return;
                    Buffs.ItemsSource = TCC.Settings.SettingsHolder.IgnoreGroupBuffs ? null : user.Buffs;
                    BuffGrid.Visibility = TCC.Settings.SettingsHolder.IgnoreGroupBuffs
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                });
            }
            catch
            {
                // ignored
            }
        }
        private void SetDebuffs()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (!(DataContext is User)) return;
                    Debuffs.ItemsSource = TCC.Settings.SettingsHolder.IgnoreGroupDebuffs ? null : ((User)DataContext).Debuffs;
                    DebuffGrid.Visibility = TCC.Settings.SettingsHolder.IgnoreGroupDebuffs
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                });
            }
            catch
            {
                // ignored
            }
        }
        private void SetMp()
        {
            Dispatcher.Invoke(() =>
            {
                MpBar.Visibility = !TCC.Settings.SettingsHolder.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void SetHp()
        {
            Dispatcher.Invoke(() =>
            {
                HpBar.Visibility = !TCC.Settings.SettingsHolder.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void AnimateIn()
        {
            var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            BeginAnimation(OpacityProperty, an);
        }

        internal void AnimateOut()
        {
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            BeginAnimation(OpacityProperty, an);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            Proxy.Proxy.AskInteractive(dc.ServerId, dc.Name);
        }

        private void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = true;
        }

        private void ToolTip_OnClosed(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = false;
        }
    }
}
