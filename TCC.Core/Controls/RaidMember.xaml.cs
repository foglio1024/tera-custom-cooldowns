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
        User dc;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dc = (User)DataContext;
            SetMP();
            SetHP();
            SetBuffs();
            SetDebuffs();

            AnimateIn();
            GroupWindowViewModel.Instance.SettingsUpdated += UpdateSettings;
        }
        private void UpdateSettings()
        {
            SetMP();
            SetHP();
            SetBuffs();
            SetDebuffs();
        }
        private void SetBuffs()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (!(DataContext is User user)) return;

                    buffs.ItemsSource = SettingsManager.IgnoreGroupBuffs ? null : user.Buffs;
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
                    debuffs.ItemsSource = SettingsManager.IgnoreGroupDebuffs ? null : dc.Debuffs;
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
                mpGrid.Visibility = !SettingsManager.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void SetHP()
        {
            Dispatcher.Invoke(() =>
            {
                hpGrid.Visibility = !SettingsManager.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        {
            FocusManager.Running = false;
        }

        private void ToolTip_OnClosed(object sender, RoutedEventArgs e)
        {
            FocusManager.Running = true;
        }

    }
}

