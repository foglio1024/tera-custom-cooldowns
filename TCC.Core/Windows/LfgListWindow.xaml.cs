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
using TCC.Data;
using TCC.Data.Pc;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per LfgListWindow.xaml
    /// </summary>
    public partial class LfgListWindow
    {
        public LfgListViewModel VM => Dispatcher.Invoke(() => DataContext as LfgListViewModel);
        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        private readonly ColorAnimation _colAn = new ColorAnimation { Duration = TimeSpan.FromMilliseconds(200) };


        public LfgListWindow()
        {
            InitializeComponent();
            DataContext = new LfgListViewModel();
            VM.PropertyChanged += VM_PropertyChanged;
            VM.Publicized += OnPublicized;
            WindowManager.ForegroundManager.VisibilityChanged += () =>
            {
                if (WindowManager.ForegroundManager.Visible) RefreshTopmost();
            };
            FocusManager.FocusTick += RefreshTopmost;

            Closing += (_, ev) =>
            {
                ev.Cancel = true;
                CloseWindow();
            };
        }

        private void OnPublicized(int cd)
        {
            var an = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(cd));
            PublicizeBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);

        }

        private void RefreshTopmost()
        {
            if (FocusManager.PauseTopmost) return;

            Dispatcher.Invoke(() =>
            {
                Topmost = false;
                Topmost = true;
            });
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VM.Creating):
                case nameof(VM.NewMessage):
                    if (VM.Creating)
                    {
                        _colAn.To = string.IsNullOrEmpty(VM.NewMessage)
                            ? R.Colors.HpColor //(Color)Application.Current.FindResource("HpColor")
                            : R.Colors.GreenColor;//(Color)Application.Current.FindResource("GreenColor");
                    }
                    else
                    {
                        _colAn.To = R.Colors.DefensiveStanceColor; //(Color)Application.Current.FindResource("DefensiveStanceColor");
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

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
        public void CloseWindow()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
                a.Completed += (s, ev) =>
                {
                    Hide();
                    if (Settings.SettingsHolder.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

                };
                BeginAnimation(OpacityProperty, a);
            }, DispatcherPriority.DataBind);
        }

        internal void ShowWindow()
        {
            if (VM.StayClosed)
            {
                VM.StayClosed = false;
                return;
            }
            if (Settings.SettingsHolder.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                VM.RefreshSorting();
            }), DispatcherPriority.Background);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                if (IsVisible) return;
                Opacity = 0;
                Show();
                FocusManager.HideFromToolBar(Handle);
                FocusManager.MakeUnfocusable(Handle);
                BeginAnimation(OpacityProperty, animation);
            }), DispatcherPriority.Background);
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
                ProxyInterop.Proxy.RequestPartyInfo(id);
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is User dc)
                ProxyInterop.Proxy.AskInteractive(SessionManager.CurrentPlayer.ServerId, dc.Name);
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
                ProxyInterop.Proxy.RegisterLfg(VM.NewMessage, RaidSwitch.IsOn);
                VM.Creating = false;
                //VM.NewMessage = "";
                VM.NewMessage = VM.MyLfg != null ? VM.MyLfg.Message : "";
                Task.Delay(200).ContinueWith(t => ProxyInterop.Proxy.RequestLfgList());
            }
            else
            {
                NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                //VM.NewMessage = "";
                VM.NewMessage = VM.MyLfg != null ? VM.MyLfg.Message : "";
                VM.Creating = false;
            }
        }

        //private void PublicizeBtn_Click(object sender, RoutedEventArgs e)
        //{

        //    PublicizeBtn.IsEnabled = false;
        //    PublicizeBtn.IsHitTestVisible = false;
        //    var an = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(3000));
        //    an.Completed += (s, ev) =>
        //    {
        //        PublicizeBarGovernor.LayoutTransform = new ScaleTransform(0, 1);
        //        PublicizeBtn.IsEnabled = true;
        //        PublicizeBtn.IsHitTestVisible = true;
        //    };
        //    PublicizeBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
        //    ProxyInterop.Proxy.PublicizeLfg();
        //}

        private void RemoveMessageButton_Click(object sender, RoutedEventArgs e)
        {
            VM.ForceStopPublicize();
            ProxyInterop.Proxy.RemoveLfg();
            ProxyInterop.Proxy.RequestLfgList();
            ProxyInterop.Proxy.RequestLfgList();
        }

        private void AcceptApply(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is User user) ProxyInterop.Proxy.PartyInvite(user.Name);
        }

        private void InspectApplicant(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is User user) ProxyInterop.Proxy.Inspect(user.Name);
        }

        private void RefuseApplicant(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is User user) ProxyInterop.Proxy.DeclineApply(user.PlayerId);
            ProxyInterop.Proxy.RequestCandidates();
        }

        private void ReloadLfgList(object sender, RoutedEventArgs e)
        {
            ProxyInterop.Proxy.RequestLfgList();
        }

        private void OnLfgMessageMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(((FrameworkElement)sender).DataContext is Listing listing)) return;
            if (!listing.IsTwitch) return;
            Process.Start(listing.TwitchLink);
        }


    }
}
