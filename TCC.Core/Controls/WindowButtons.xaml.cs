using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per WindowButtons.xaml
    /// </summary>
    public partial class WindowButtons : UserControl, INotifyPropertyChanged
    {
        private WindowSettings _dc => DataContext as WindowSettings;
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



        public Visibility HideButtonVisibility
        {
            get { return (Visibility)GetValue(HideButtonVisibilityProperty); }
            set { SetValue(HideButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty HideButtonVisibilityProperty = DependencyProperty.Register("HideButtonVisibility", typeof(Visibility), typeof(WindowButtons));



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
            //Console.WriteLine($"Data context is {t}");
        }

        private void AutoDim(object sender, RoutedEventArgs e)
        {
            _dc.AutoDim = !_dc.AutoDim;
        }
    }
}
