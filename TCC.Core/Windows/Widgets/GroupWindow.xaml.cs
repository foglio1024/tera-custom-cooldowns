using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow
    {
        public GroupWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.GroupWindowSettings, ignoreSize: false);
            DataContext = GroupWindowViewModel.Instance;
        }

        private void LootSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!GroupWindowViewModel.Instance.AmILeader) return;
            Proxy.LootSettings();
        }

        private void DisbandButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!GroupWindowViewModel.Instance.AmILeader) return;
            Proxy.DisbandParty();
        }

        private void ResetButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!GroupWindowViewModel.Instance.AmILeader) return;
            Proxy.ResetInstance();
        }

        private void GroupWindow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
            GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
        }

        private void GroupWindow_OnMouseLeave(object sender, MouseEventArgs e)
        {
            GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
            GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
        }

        private void LeaveParty(object sender, RoutedEventArgs e)
        {
            Proxy.LeaveParty();
        }

        private void ShowAbnormalSettings(object sender, RoutedEventArgs e)
        {
            WindowManager.GroupAbnormalConfigWindow.ShowWindow();
        }
    }
}