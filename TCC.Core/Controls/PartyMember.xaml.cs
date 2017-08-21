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
    /// Logica di interazione per PartyMember.xaml
    /// </summary>
    public partial class PartyMember : UserControl
    {
        public PartyMember()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetMP();
            SetHP();

            AnimateIn();
            GroupWindowViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
        }
        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GroupWindowViewModel.Instance.MPenabled))
            {
                SetMP();
            }
            else if (e.PropertyName == nameof(GroupWindowViewModel.Instance.HPenabled))
            {
                SetHP();
            }
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (User)DataContext;
            ProxyInterop.SendAskInteractiveMessage(dc.ServerId, dc.Name);
        }
    }
}
