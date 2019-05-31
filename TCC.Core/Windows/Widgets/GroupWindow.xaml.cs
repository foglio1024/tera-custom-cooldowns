using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow
    {
        public GroupWindowViewModel VM { get;}

        public GroupWindow()
        {
            DataContext = new GroupWindowViewModel();
            VM = DataContext as GroupWindowViewModel;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.SettingsHolder.GroupWindowSettings, ignoreSize: false);
        }


        private void DisbandButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!VM.AmILeader) return;
            ProxyInterface.Instance.Stub.DisbandGroup(); //ProxyOld.DisbandParty();
        }

        private void ResetButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!VM.AmILeader) return;
            ProxyInterface.Instance.Stub.ResetInstance(); //ProxyOld.ResetInstance();
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
            ProxyInterface.Instance.Stub.LeaveGroup(); //ProxyOld.LeaveParty();
        }

        private void ShowAbnormalSettings(object sender, RoutedEventArgs e)
        {
            new GroupAbnormalConfigWindow().ShowWindow();
            //WindowManager.GroupAbnormalConfigWindow.ShowWindow();
        }
    }
}