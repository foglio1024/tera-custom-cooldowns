using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Parsing;

namespace TCC
{

    /// <summary>
    /// Logica di interazione per StaminaGauge.xaml
    /// </summary>
    public partial class StaminaGauge : Window, INotifyPropertyChanged
    {
        public StaminaGauge()
        {
            InitializeComponent();
        }

        private bool enabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Enabled
        {
            get { return enabled; }
            set {
                if(enabled != value)
                {
                    enabled = value;
                    NotifyPropertyChanged("Enabled");
                }

            }
        }

        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public void Init(System.Windows.Media.Color c)
        {
            Dispatcher.Invoke(() =>
            {
                gauge = new StaminaGaugeControl(c);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            //Opacity = 0;
            ContextMenu = new ContextMenu();
            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) => 
            {
                this.Visibility = Visibility.Collapsed;
            };
            ContextMenu.Items.Add(HideButton);

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Properties.Settings.Default.ClassGaugeLeft = Left;
            //Properties.Settings.Default.ClassGaugeTop= Top;
            //Properties.Settings.Default.Save();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
