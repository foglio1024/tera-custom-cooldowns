using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.Group
{
    /// <summary>
    /// Logica di interazione per RaidMember.xaml
    /// </summary>
    public partial class RaidMember //TODO: make base class for this when???
    {
        public RaidMember()
        {
            InitializeComponent();
            Unloaded += (_, __) => { SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged; };
        }

        private User _dc;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = (User)DataContext;
            UpdateSettings();

            AnimateIn();
            GroupWindowViewModel.Instance.SettingsUpdated += UpdateSettings;
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }
        private void UpdateSettings()
        {
            SetMP();
            SetHP();
            SetBuffs();
            SetDebuffs();
            SetLaurels();
            SetAwakenIcon();

        }
        private void OnAbnormalityShapeChanged()
        {
            Buffs.ItemTemplateSelector = null;
            Buffs.ItemTemplateSelector = Application.Current.FindResource("RaidAbnormalityTemplateSelector") as DataTemplateSelector;
            Debuffs.ItemTemplateSelector = null;
            Debuffs.ItemTemplateSelector = Application.Current.FindResource("RaidAbnormalityTemplateSelector") as DataTemplateSelector;

        }

        private void SetAwakenIcon()
        {
            Dispatcher.Invoke(() => {
                if (!(DataContext is User user)) return;
                AwakenIcon.Visibility = TCC.Settings.Settings.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) : Visibility.Collapsed;
            });
        }

        private void SetLaurels()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LaurelImage.Visibility = TCC.Settings.Settings.ShowMembersLaurels ? Visibility.Visible : Visibility.Hidden;
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

                    Buffs.ItemsSource = TCC.Settings.Settings.IgnoreGroupBuffs ? null : user.Buffs;
                    BuffGrid.Visibility = TCC.Settings.Settings.IgnoreGroupBuffs
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
                    // ReSharper disable once IsExpressionAlwaysTrue
                    if(!(_dc is User)) return;
                    Debuffs.ItemsSource = TCC.Settings.Settings.IgnoreGroupDebuffs ? null : _dc.Debuffs;
                    DebuffGrid.Visibility = TCC.Settings.Settings.IgnoreGroupDebuffs
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                });

            }
            catch
            {
                // ignored
            }
        }

        public void AnimateIn()
        {
            var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            BeginAnimation(OpacityProperty, an);
        }
        internal void AnimateOut()
        {
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            BeginAnimation(OpacityProperty, an);
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            Proxy.AskInteractive(dc.ServerId, dc.Name);
        }
        private void SetMP()
        {
            Dispatcher.Invoke(() =>
            {
                MpBar.Visibility = !TCC.Settings.Settings.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void SetHP()
        {
            Dispatcher.Invoke(() =>
            {
                HpBar.Visibility = !TCC.Settings.Settings.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
            });
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

