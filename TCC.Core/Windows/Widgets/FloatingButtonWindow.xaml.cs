using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls.Chat;
using TCC.Interop.Proxy;
using FoglioUtils.Extensions;
using TCC.Utils;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    
    // TODO: refactor this when???
    public partial class FloatingButtonWindow
    {
        public FloatingButtonWindow()
        {
            InitializeComponent();
            MainContent = WindowContent;
            ButtonsRef = null;
            _canMove = false;
            Init(App.Settings.FloatingButtonSettings);
        }

        private Timer _t;
        private DispatcherTimer _n;
        private DoubleAnimation _an;
        private void FloatinButtonLoaded(object sender, RoutedEventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(handle);
            FocusManager.HideFromToolBar(handle);

            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget == null) return;
            var m = source.CompositionTarget.TransformToDevice;
            var _ = m.M11;
            Left = 0;
            Top = Screen.PrimaryScreen.Bounds.Height / 2 - ActualHeight / 2;

            WindowManager.VisibilityManager.VisibilityChanged += OnTccVisibilityChanged;
            _t = new Timer { Interval = 2000 };
            _t.Tick += RepeatAnimation;
            _n = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(4000) };
            _n.Tick += _n_Tick;
            _an = new DoubleAnimation(.75, 1, TimeSpan.FromMilliseconds(800)) { EasingFunction = new ElasticEase() };
            _queue = new Queue<Tuple<string, string, NotificationType, uint>>();
        }


        private void RepeatAnimation(object sender, EventArgs e)
        {
            Animate();
        }

        private void Animate()
        {
            NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _an);
            NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _an);
        }

        private void OnTccVisibilityChanged()
        {
            //if(Screen.AllScreens.ToList().IndexOf(ScreenFromWindowCenter()) == FocusManager.TeraScreenIndex) return;

            if (FocusManager.TeraScreen == null) return;
            var teraScreenBounds = FocusManager.TeraScreen.Bounds;
            Left = teraScreenBounds.X;
            Top = teraScreenBounds.Y + teraScreenBounds.Height / 2;
            //RefreshTopmost();
            //AnimateContentOpacity(WindowManager.ForegroundManager.Visible ? 1 : 0);
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-32, -1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-1, -32, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NotificationBubble.Visibility != Visibility.Hidden)
            {
                NotificationBubble.Visibility = Visibility.Hidden;
                NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                _t.Stop();
            }
            WindowManager.DashboardWindow.ShowWindow();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowManager.SettingsWindow.ShowWindow();
        }

        private void RefreshTopmost()
        {
            if (FocusManager.PauseTopmost) return;

            Dispatcher.InvokeIfRequired(() => { Topmost = false; Topmost = true; }, DispatcherPriority.DataBind);
        }

        public void StartNotifying(int closeEventsCount)
        {
            Dispatcher?.Invoke(() =>
            {
                EventAmountTb.Text = closeEventsCount.ToString();
                NotificationBubble.Visibility = Visibility.Visible;
                Animate();
                _t.Start();
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ProxyInterface.Instance.Stub.RequestListings();
        }

        private bool _busy;
        private Queue<Tuple<string, string, NotificationType, uint>> _queue;
        public void NotifyExtended(string title, string msg, NotificationType type, uint timeMs = 4000)
        {
            if (_busy)
            {
                Dispatcher?.Invoke(() =>
                {
                    if (msg != NotificationText.Text 
                   || title != NotificationTitle.Text)
                        _queue.Enqueue(new Tuple<string, string, NotificationType, uint>(title, msg, type, timeMs));
                });
                return;
            }

            _busy = true;
            Dispatcher.Invoke(() =>
            {
                NotificationText.Text = msg;
                NotificationTitle.Text = title;
                switch (type)
                {
                    case NotificationType.Normal:
                        NotificationColorBorder.Background = R.Brushes.TccNormalGradientBrush;
                        break;
                    case NotificationType.Success:
                        NotificationColorBorder.Background = R.Brushes.TccGreenGradientBrush; 
                        break;
                    case NotificationType.Warning:
                        NotificationColorBorder.Background = R.Brushes.TccYellowGradientBrush; 
                        break;
                    case NotificationType.Error:
                        NotificationColorBorder.Background = R.Brushes.TccRedGradientBrush; 
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                RefreshTopmost();
                NotificationContainer.RenderTransform.BeginAnimation(TranslateTransform.XProperty,
                    new DoubleAnimation(50, 0, TimeSpan.FromMilliseconds(250))
                    {
                        EasingFunction = new QuadraticEase()
                    });
                NotificationContent.BeginAnimation(OpacityProperty,
                    new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200))
                    {
                        EasingFunction = new QuadraticEase()
                    });

                NotificationTimeGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(timeMs)));
                _n.Interval = TimeSpan.FromMilliseconds(timeMs);
                _n.Start();
            });
        }

        private void _n_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                NotificationContainer.RenderTransform.BeginAnimation(TranslateTransform.XProperty,
                    new DoubleAnimation(0, -50, TimeSpan.FromMilliseconds(250))
                    {
                        EasingFunction = new QuadraticEase()
                    });
                NotificationContent.BeginAnimation(OpacityProperty,
                    new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
                    {
                        EasingFunction = new QuadraticEase()
                    });
            });
            _n.Stop();
            _busy = false;
            if (_queue.Count <= 0) return;
            var tuple = _queue.Dequeue();
            NotifyExtended(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        //public void OpenPlayerMenu()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        FocusManager.PauseTopmost = true;//FocusTimer.Enabled = false;
        //        RefreshTopmost();
        //        if (PlayerInfo.IsOpen) ClosePlayerMenu();
        //        PlayerMenuVM.Refresh();
        //        Task.Delay(100).ContinueWith(t =>
        //        {
        //            Dispatcher.Invoke(() =>
        //            {
        //                PlayerInfo.IsOpen = true;
        //                ((PlayerMenuWindow) PlayerInfo.Child).AnimateOpening();
        //            });
        //        });

        //    });
        //}
        //public void ClosePlayerMenu()
        //{
        //    //TODO
        //    Dispatcher.Invoke(() =>
        //    {
        //        //if (((PlayerMenuWindow)PlayerInfo.Child).MgPopup.IsMouseOver) return;
        //        //if (((PlayerMenuWindow)PlayerInfo.Child).FpsUtilsPopup.IsMouseOver) return;
        //        //FocusManager.PauseTopmost = false; //.FocusTimer.Enabled = true;
        //        //PlayerInfo.IsOpen = false;
        //    });
        //}
        //public void SetMoongourdButtonVisibility()
        //{
        //    //TODO
        //    Dispatcher.Invoke(() =>
        //    {
        //        //((PlayerMenuWindow)PlayerInfo.Child).SetMoongourdVisibility();
        //    });
        //}
    }
}
