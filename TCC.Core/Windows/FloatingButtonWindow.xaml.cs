using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Interaction logic for FloatingButtonWindow.xaml
    /// </summary>
    public partial class FloatingButtonWindow : Window
    {
        public FloatingButtonWindow()
        {
            InitializeComponent();
        }

        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var _handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(_handle);
            FocusManager.HideFromToolBar(_handle);

            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget == null) return;
            var m = source.CompositionTarget.TransformToDevice;
            var dx = m.M11;
            var dy = m.M22;
            Left = 0;
            Top = screen.Bounds.Height * dx / 2 - this.ActualHeight/2;

            WindowManager.TccVisibilityChanged += WindowManager_TccVisibilityChanged;

        }
        public void AnimateContentOpacity(double opacity)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                ((FrameworkElement)this.Content).BeginAnimation(OpacityProperty, new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(250)));
            }, System.Windows.Threading.DispatcherPriority.DataBind);
        }

        private void WindowManager_TccVisibilityChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshTopmost();
            if (WindowManager.IsTccVisible) AnimateContentOpacity(1);
            else AnimateContentOpacity(0);
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-32, -1, TimeSpan.FromMilliseconds(150)) {EasingFunction = new QuadraticEase() });
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-1, -32, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InfoWindowViewModel.Instance.ShowWindow();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowManager.Settings.ShowWindow();
        }
        public void RefreshTopmost()
        {
            Dispatcher.InvokeIfRequired(() => { Topmost = false; Topmost = true; }, System.Windows.Threading.DispatcherPriority.DataBind);
        }

    }
}
