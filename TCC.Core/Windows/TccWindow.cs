using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Windows
{
    public class TccWindow : Window
    {
        public event Action Hidden;
        public event Action Showed;
        public void HideWindow()
        {
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
            a.Completed += (s, ev) =>
            {
                Hide();
                if (Settings.SettingsStorage.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                Hidden?.Invoke();
            };
            BeginAnimation(OpacityProperty, a);
        }
        public void ShowWindow()
        {
            if (Settings.SettingsStorage.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
            Dispatcher.Invoke(() =>
            {
                Topmost = false; Topmost = true;
                Opacity = 0;
                Show();
                Showed?.Invoke();
                BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            });
        }
    }
}