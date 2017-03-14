using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            if (Properties.Settings.Default.Transparent)
            {
                FocusManager.MakeTransparent(hwnd);
            }

        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SessionManager.CurrentBosses.Clear();
            }));
        }
    }
}

