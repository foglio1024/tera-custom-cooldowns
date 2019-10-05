using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using FoglioUtils.Extensions;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class LfgListWindow
    {
        private LfgListViewModel VM { get; }

        private readonly ColorAnimation _colAn = new ColorAnimation { Duration = TimeSpan.FromMilliseconds(200) };


        public LfgListWindow(LfgListViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            VM = vm;
            VM.PropertyChanged += VM_PropertyChanged;
            VM.Publicized += OnPublicized;
            WindowManager.ForegroundManager.VisibilityChanged += () =>
            {
                if (WindowManager.ForegroundManager.Visible) RefreshTopmost();
            };
            FocusManager.FocusTick += RefreshTopmost;

            //Loaded += OnLoaded;
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            FocusManager.HideFromToolBar(Handle);
            FocusManager.MakeUnfocusable(Handle);
        }

        private void OnPublicized(int cd)
        {
            var an = AnimationFactory.CreateDoubleAnimation(cd * 1000, 1, 0);
            PublicizeBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VM.Creating):
                case nameof(VM.NewMessage):
                    if (VM.Creating)
                    {
                        _colAn.To = string.IsNullOrEmpty(VM.NewMessage) ? R.Colors.HpColor : R.Colors.GreenColor;
                    }
                    else
                    {
                        _colAn.To = R.Colors.DefensiveStanceColor;
                    }
                    var currBg = (SolidColorBrush)CreateMessageBtn.Background;
                    var currCol = currBg.Color;
                    var newBg = new SolidColorBrush(currCol);
                    CreateMessageBtn.Background = newBg;
                    CreateMessageBtn.Background.BeginAnimation(SolidColorBrush.ColorProperty, _colAn);
                    break;
                case nameof(VM.AmIinLfg) when VM.AmIinLfg:
                    LfgMgmtBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                        new DoubleAnimation(1,
                            TimeSpan.FromMilliseconds(150))
                        { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                        new DoubleAnimation(0,
                            TimeSpan.FromMilliseconds(150))
                        { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(4, 0, 4, 0), TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    break;
                case nameof(VM.AmIinLfg):
                    LfgMgmtBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    CreateMessageBtn.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(4), TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                    break;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //CloseWindow();
            HideWindow();
        }
        //public void CloseWindow()
        //{
        //    Dispatcher.InvokeIfRequired(() =>
        //    {
        //        var a = AnimationFactory.CreateDoubleAnimation(150,0,1, completed: (s, ev) =>
        //        {
        //            Hide();
        //            if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        //        });
        //        BeginAnimation(OpacityProperty, a);
        //    }, DispatcherPriority.DataBind);
        //}

        public override void ShowWindow()
        {
            if (VM.StayClosed)
            {
                VM.StayClosed = false;
                return;
            }
            Dispatcher?.BeginInvoke(new Action(() => VM.RefreshSorting()), DispatcherPriority.Background);

            base.ShowWindow();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is Listing l)) return;
            if (l.IsExpanded)
            {
                l.IsExpanded = false;
            }
            else
            {
                var id = l.LeaderId;
                VM.LastClicked = l;
                ProxyInterface.Instance.Stub.RequestPartyInfo(id);
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is User dc)
                ProxyInterface.Instance.Stub.AskInteractive(Game.Me.ServerId, dc.Name);
        }

        private void CreateMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.Creating)
            {
                FocusManager.UndoUnfocusable(Handle);
                Activate();
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                NewMessageTextBox.Focus();
                VM.NewMessage = VM.MyLfg != null ? VM.MyLfg.Message : "";
                VM.Creating = true;
            }
            else if (VM.Creating && !string.IsNullOrEmpty(VM.NewMessage))
            {
                FocusManager.UndoUnfocusable(Handle);
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                ProxyInterface.Instance.Stub.RegisterListing(VM.NewMessage, RaidSwitch.IsOn);
                VM.Creating = false;
                //VM.NewMessage = "";
                VM.NewMessage = VM.MyLfg != null ? VM.MyLfg.Message : "";
                Task.Delay(200).ContinueWith(t =>
                        ProxyInterface.Instance.Stub.RequestListings()
                    );

            }
            else
            {
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                //VM.NewMessage = "";
                VM.NewMessage = VM.MyLfg != null ? VM.MyLfg.Message : "";
                VM.Creating = false;
            }
        }

        private void RemoveMessageButton_Click(object sender, RoutedEventArgs e)
        {
            VM.ForceStopPublicize();
            ProxyInterface.Instance.Stub.RemoveListing();
            ProxyInterface.Instance.Stub.RequestListings();
        }

        private void AcceptApply(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is User user) ProxyInterface.Instance.Stub.GroupInviteUser(user.Name);
        }

        private void InspectApplicant(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is User user) ProxyInterface.Instance.Stub.InspectUser(user.Name);
        }

        private void RefuseApplicant(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is User user) ProxyInterface.Instance.Stub.DeclineUserGroupApply(user.PlayerId);
            ProxyInterface.Instance.Stub.RequestListingCandidates();
        }

        private void ReloadLfgList(object sender, RoutedEventArgs e)
        {
            ProxyInterface.Instance.Stub.RequestListings();
        }

        private void OnLfgMessageMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(((FrameworkElement)sender).DataContext is Listing listing)) return;
            if (!listing.IsTwitch) return;
            Process.Start(listing.TwitchLink);
        }


    }
}
