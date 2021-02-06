using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Interop.Proxy;
using TCC.UI.Controls.Abnormalities;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets
{
    public partial class GroupWindow
    {
        private GroupWindowViewModel VM { get;}

        public GroupWindow(GroupWindowViewModel vm)
        {
            DataContext = vm;
            VM = vm;
            InitializeComponent();
            BoundaryRef = Boundary;
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(VM.Settings!);
        }

        //TODO: to commands in VM
        private void DisbandButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!VM.AmILeader) return;
            StubInterface.Instance.StubClient.DisbandGroup(); //ProxyOld.DisbandParty();
        }

        //TODO: to commands in VM
        private void ResetButtonClicked(object sender, RoutedEventArgs e)
        {
            if(!VM.AmILeader) return;
            StubInterface.Instance.StubClient.ResetInstance(); //ProxyOld.ResetInstance();
        }

        private void GroupWindow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            AbnormalityIndicatorBase.InvokeVisibilityChanged(this, true);
            GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
            GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
        }

        private void GroupWindow_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Task.Delay(1000).ContinueWith(_ => Dispatcher.InvokeAsync(() =>
            {
                if (IsMouseOver) return;
                AbnormalityIndicatorBase.InvokeVisibilityChanged(this, false);
            }));
            GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
            GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
        }

        //TODO: to commands in VM
        private void LeaveParty(object sender, RoutedEventArgs e)
        {
            StubInterface.Instance.StubClient.LeaveGroup(); //ProxyOld.LeaveParty();
        }

        //TODO: to commands in VM
        private void ShowAbnormalSettings(object sender, RoutedEventArgs e)
        {
            new GroupAbnormalConfigWindow().ShowWindow();
            //WindowManager.GroupAbnormalConfigWindow.ShowWindow();
        }
    }
}