using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per RaidMember.xaml
    /// </summary>
    public partial class RaidMember : UserControl
    {
        public RaidMember()
        {
            InitializeComponent();
        }

        private User dc;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dc = (User)DataContext;
            UpdateSettings();

            AnimateIn();
            GroupWindowViewModel.Instance.SettingsUpdated += UpdateSettings;
        }
        private void UpdateSettings()
        {
            SetMP();
            SetHP();
            SetBuffs();
            SetDebuffs();
            SetLaurels();
        }
        private void SetLaurels()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LaurelImage.Visibility = SettingsManager.ShowMembersLaurels ? Visibility.Visible : Visibility.Collapsed;
                });
            }
            catch (Exception)
            {
            }
        }
        private void SetBuffs()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (!(DataContext is User user)) return;

                    Buffs.ItemsSource = SettingsManager.IgnoreGroupBuffs ? null : user.Buffs;
                    BuffGrid.Visibility = SettingsManager.IgnoreGroupBuffs
                        ? Visibility.Collapsed
                        : Visibility.Visible;

                });

            }
            catch (Exception)
            {

            }
        }
        private void SetDebuffs()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if(!(dc is User)) return;
                    Debuffs.ItemsSource = SettingsManager.IgnoreGroupDebuffs ? null : dc.Debuffs;
                    DebuffGrid.Visibility = SettingsManager.IgnoreGroupDebuffs
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                });

            }
            catch (Exception)
            {

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
                MpGrid.Visibility = !SettingsManager.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void SetHP()
        {
            Dispatcher.Invoke(() =>
            {
                HpGrid.Visibility = !SettingsManager.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = false;
        }

        private void ToolTip_OnClosed(object sender, RoutedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = true;
        }

    }
}

