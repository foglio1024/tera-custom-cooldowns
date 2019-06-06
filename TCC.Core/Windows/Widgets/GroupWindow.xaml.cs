using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public partial class GroupWindow
    {
        private GroupWindowViewModel VM { get;}

        public GroupWindow(GroupWindowViewModel vm)
        {
            DataContext = vm;
            VM = DataContext as GroupWindowViewModel;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(App.Settings.GroupWindowSettings);
        }

        //TODO: to commands in VM
        private void DisbandButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!VM.AmILeader) return;
            ProxyInterface.Instance.Stub.DisbandGroup(); //ProxyOld.DisbandParty();
        }

        //TODO: to commands in VM
        private void ResetButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!VM.AmILeader) return;
            ProxyInterface.Instance.Stub.ResetInstance(); //ProxyOld.ResetInstance();
        }

        //TODO: to commands in VM?
        private void GroupWindow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
            GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
        }

        //TODO: to commands in VM?
        private void GroupWindow_OnMouseLeave(object sender, MouseEventArgs e)
        {
            GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
            GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
        }

        //TODO: to commands in VM
        private void LeaveParty(object sender, RoutedEventArgs e)
        {
            ProxyInterface.Instance.Stub.LeaveGroup(); //ProxyOld.LeaveParty();
        }

        //TODO: to commands in VM
        private void ShowAbnormalSettings(object sender, RoutedEventArgs e)
        {
            new GroupAbnormalConfigWindow().ShowWindow();
            //WindowManager.GroupAbnormalConfigWindow.ShowWindow();
        }
    }
}