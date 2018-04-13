using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per WindowButtons.xaml
    /// </summary>
    public partial class WindowButtons : UserControl, INotifyPropertyChanged
    {
        WindowSettings _dc => this.DataContext as WindowSettings;
        public WindowButtons()
        {
            InitializeComponent();
        }


        public string WindowName
        {
            get { return (string)GetValue(WindowNameProperty); }
            set { SetValue(WindowNameProperty, value); }
        }
        public static readonly DependencyProperty WindowNameProperty = DependencyProperty.Register("WindowName", typeof(string), typeof(WindowButtons));



        public Visibility AutoDimButtonVisiblity
        {
            get { return (Visibility)GetValue(AutoDimButtonVisiblityProperty); }
            set { SetValue(AutoDimButtonVisiblityProperty, value); }
        }
        public static readonly DependencyProperty AutoDimButtonVisiblityProperty = DependencyProperty.Register("AutoDimButtonVisiblity", typeof(Visibility), typeof(WindowButtons));


        public event PropertyChangedEventHandler PropertyChanged;

        private void Hide(object sender, RoutedEventArgs e)
        {
            _dc.Visible = false;
        }

        private void Pin(object sender, RoutedEventArgs e)
        {
            _dc.ShowAlways = !_dc.ShowAlways;
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_dc.ShowAlways)));
        }


        private void Close(object sender, RoutedEventArgs e)
        {
            _dc.Enabled = false;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            //_dc = DataContext as WindowSettings;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null) {Console.WriteLine($"Data context is null"); return; }
            var t = DataContext.GetType().Name;
            Console.WriteLine($"Data context is {t}");
        }

        private void AutoDim(object sender, RoutedEventArgs e)
        {
            _dc.AutoDim = !_dc.AutoDim;
        }
    }
}
