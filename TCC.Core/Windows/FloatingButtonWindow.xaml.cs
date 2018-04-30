using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Interaction logic for FloatingButtonWindow.xaml
    /// </summary>
    public partial class FloatingButtonWindow
    {
        public FloatingButtonWindow()
        {
            InitializeComponent();
        }

        private Timer _t;
        private DoubleAnimation _an;
        private void FloatinButtonLoaded(object sender, RoutedEventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(handle);
            FocusManager.HideFromToolBar(handle);

            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget == null) return;
            var m = source.CompositionTarget.TransformToDevice;
            var _ = m.M11;
            var dy = m.M22;
            Left = 0;
            Top = screen.Bounds.Height * dy / 2 - ActualHeight / 2;

            WindowManager.TccVisibilityChanged += WindowManager_TccVisibilityChanged;
            _t = new Timer {Interval = 2000};
            _t.Tick += RepeatAnimation;
            _an = new DoubleAnimation(.75, 1, TimeSpan.FromMilliseconds(800)) { EasingFunction = new ElasticEase() };
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

        private void AnimateContentOpacity(double opacity)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                ((FrameworkElement)Content).BeginAnimation(OpacityProperty, new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(250)));
            }, System.Windows.Threading.DispatcherPriority.DataBind);
        }

        private void WindowManager_TccVisibilityChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshTopmost();
            AnimateContentOpacity(WindowManager.IsFocused ? 1 : 0);
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-32, -1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-1, -32, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });

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
            InfoWindowViewModel.Instance.ShowWindow();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowManager.Settings.ShowWindow();
        }

        private void RefreshTopmost()
        {
            Dispatcher.InvokeIfRequired(() => { Topmost = false; Topmost = true; }, System.Windows.Threading.DispatcherPriority.DataBind);
        }

        public void StartNotifying(int closeEventsCount)
        {
            Dispatcher.Invoke(() =>
            {
                EventAmountTb.Text = closeEventsCount.ToString();
                NotificationBubble.Visibility = Visibility.Visible;
                Animate();
                _t.Start();
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Proxy.RequestLfgList();
        }
    }
}
