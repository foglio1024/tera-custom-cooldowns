using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow
    {
        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);
        public InfoWindowViewModel VM => Dispatcher.Invoke(() => DataContext as InfoWindowViewModel);

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
            VM.SaveToFile();
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
            FocusManager.MakeUnfocusable(handle);
        }

    }
}
