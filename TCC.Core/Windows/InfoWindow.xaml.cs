using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow
    {
        public InfoWindow()
        {
            InitializeComponent();
            Closing += OnClosing;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            CloseWindow(null, null);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            HideWindow();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // ignored
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            FocusManager.HideFromToolBar(handle);
        }
        internal void ShowWindow()
        {
            Dispatcher.Invoke(() =>
            {
                Topmost = false; Topmost = true;
                Opacity = 0;
                Show();
                //Activate();
                BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            });
        }

        public void HideWindow()
        {
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
            a.Completed += (s, ev) => { Hide(); InfoWindowViewModel.Instance.SaveToFile(); };
            BeginAnimation(OpacityProperty, a);
        }
    }
}
