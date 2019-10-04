using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FoglioUtils;
using FoglioUtils.Extensions;

namespace TCC.Windows
{
    public class TccWindow : Window
    {
        public event Action Hidden;
        public event Action Showed;

        private readonly DoubleAnimation _showAnim;
        private readonly DoubleAnimation _hideAnim;

        public IntPtr Handle { get; private set; }


        public TccWindow()
        {
            Closing += OnClosing;
            Loaded += OnLoaded;
            _showAnim = AnimationFactory.CreateDoubleAnimation(150, 1);
            _hideAnim = AnimationFactory.CreateDoubleAnimation(150, 0, completed: (_, __) =>
            {
                Hide();
                if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                Hidden?.Invoke();
            });
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            Handle = new WindowInteropHelper(this).Handle;
        }

        public void HideWindow()
        {
            BeginAnimation(OpacityProperty, _hideAnim);

        }
        public void ShowWindow()
        {
            if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
            Dispatcher?.Invoke(() =>
            {
                Topmost = false; Topmost = true;
                //Opacity = 0;
                Show();
                Showed?.Invoke();
                BeginAnimation(OpacityProperty, _showAnim);
            });
        }
        protected void Drag(object sender, MouseButtonEventArgs e)
        {
            this.TryDragMove();
        }
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            HideWindow();
        }
    }
}