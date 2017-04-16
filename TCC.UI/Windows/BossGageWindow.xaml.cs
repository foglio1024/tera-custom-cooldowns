using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per BossGageWindow.xaml
    /// </summary>
    public partial class BossGageWindow : Window
    {
        public BossGageWindow()
        {
            InitializeComponent();

            Bosses.DataContext = SessionManager.CurrentBosses;
            Bosses.ItemsSource = SessionManager.CurrentBosses;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            Opacity = 0;
            Topmost = true;

            ContextMenu = new ContextMenu();
            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) =>
            {
                this.Visibility = Visibility.Collapsed;
            };
            ContextMenu.Items.Add(HideButton);


        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //App.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    //SessionManager.CurrentBosses.Clear();
            //}));
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

