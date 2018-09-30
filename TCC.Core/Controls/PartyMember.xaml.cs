using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
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
            GroupWindowViewModel.Instance.SettingsUpdated += UpdateSettings;
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }

        private void OnAbnormalityShapeChanged()
        {
            Buffs.ItemTemplateSelector = null;
            Buffs.ItemTemplateSelector = Application.Current.FindResource("PartyAbnormalityTemplateSelector") as DataTemplateSelector;
            Debuffs.ItemTemplateSelector = null;
            Debuffs.ItemTemplateSelector = Application.Current.FindResource("PartyAbnormalityTemplateSelector") as DataTemplateSelector;
            
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

        private void SetAbnormalityTemplate()
        {
            Buffs.ItemTemplate = Application.Current.FindResource(Settings.AbnormalityShape == AbnormalityShape.Square ? "SquarePartyAbnormality" : "RoundPartyAbnormality") as DataTemplate;
            Debuffs.ItemTemplate = Application.Current.FindResource(Settings.AbnormalityShape == AbnormalityShape.Square ? "SquarePartyAbnormality" : "RoundPartyAbnormality") as DataTemplate;
        }

        private void SetAwakenIcon()
        {
            Dispatcher.Invoke(() => {
                if (!(DataContext is User user)) return;
                AwakenIcon.Visibility = Settings.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) : 
                    Visibility.Collapsed;
                AwakenBorder.Visibility = Settings.ShowAwakenIcon ? (user.Awakened ? Visibility.Visible : Visibility.Collapsed) : 
                    Visibility.Collapsed;
            });
        }

        private void SetLaurels()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LaurelImage.Visibility = Settings.ShowMembersLaurels ? Visibility.Visible : Visibility.Collapsed;
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
                    Buffs.ItemsSource = Settings.IgnoreGroupBuffs ? null : user.Buffs;
                    BuffGrid.Visibility = Settings.IgnoreGroupBuffs
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
                    Debuffs.ItemsSource = Settings.IgnoreGroupDebuffs ? null : ((User)DataContext).Debuffs;
                    DebuffGrid.Visibility = Settings.IgnoreGroupDebuffs
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
                MpBar.Visibility = !Settings.DisablePartyMP ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void SetHp()
        {
            Dispatcher.Invoke(() =>
            {
                HpBar.Visibility = !Settings.DisablePartyHP ? Visibility.Visible : Visibility.Collapsed;
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
            Proxy.AskInteractive(dc.ServerId, dc.Name);
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
