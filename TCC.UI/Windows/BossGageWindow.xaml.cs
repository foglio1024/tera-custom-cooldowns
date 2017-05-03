using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace TCC.Converters
{

    public class HarrowholdBossesVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

namespace TCC.Windows
{

    public partial class BossGageWindow : Window
    {

        public BossGageWindow()
        {
            InitializeComponent();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            Topmost = true;

            ContextMenu = new ContextMenu();
            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) =>
            {
                this.Visibility = Visibility.Collapsed;
                VisibilityChanged?.Invoke(this, new PropertyChangedEventArgs("Visiblity"));
                Console.WriteLine("Boss visibility change invoked");

            };
            ContextMenu.Items.Add(HideButton);           
        }

        public event PropertyChangedEventHandler VisibilityChanged;

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

