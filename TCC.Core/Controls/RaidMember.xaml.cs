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
    public partial class RaidMember : UserControl //TODO: make base class for this when???
    {
        public RaidMember()
        {
            InitializeComponent();
            Unloaded += (_, __) => { SettingsWindowViewModel.AbnormalityShapeChanged -= OnAbnormalityShapeChanged; };
        }

        private User dc;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dc = (User)DataContext;
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
            Buffs.ItemTemplateSelector = Application.Current.FindResource("PartyAbnormalityTemplateSelector") as DataTemplateSelector;
            Debuffs.ItemTemplateSelector = null;
            Debuffs.ItemTemplateSelector = Application.Current.FindResource("PartyAbnormalityTemplateSelector") as DataTemplateSelector;

        }
        private void SetAbnormalityTemplate()
        {
            Buffs.ItemTemplate = Application.Current.FindResource(Settings.AbnormalityShape == AbnormalityShape.Square
                ? "SquarePartyAbnormality"
                : "RoundPartyAbnormality") as DataTemplate;

            Debuffs.ItemTemplate = Application.Current.FindResource(Settings.AbnormalityShape == AbnormalityShape.Square
                ? "SquarePartyAbnormality"
                : "RoundPartyAbnormality") as DataTemplate;
        }

        private void SetAwakenIcon()
        {
            Dispatcher.Invoke(() => {
                if (!(DataContext is User user)) return;
                AwakenIcon.Visibility = Settings.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) : Visibility.Collapsed;
            });
        }

        private void SetLaurels()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LaurelImage.Visibility = Settings.ShowMembersLaurels ? Visibility.Visible : Visibility.Hidden;
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

                    Buffs.ItemsSource = Settings.IgnoreGroupBuffs ? null : user.Buffs;
                    BuffGrid.Visibility = Settings.IgnoreGroupBuffs
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
                    Debuffs.ItemsSource = Settings.IgnoreGroupDebuffs ? null : dc.Debuffs;
                    DebuffGrid.Visibility = Settings.IgnoreGroupDebuffs
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
                MpBar.Visibility = !Settings.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void SetHP()
        {
            Dispatcher.Invoke(() =>
            {
                HpBar.Visibility = !Settings.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
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

