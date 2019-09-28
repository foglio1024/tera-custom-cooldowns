using System.Windows;
using TCC.Settings;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per WindowButtons.xaml
    /// </summary>
    public partial class WindowButtons
    {
        private WindowSettings Dc => DataContext as WindowSettings;
        public WindowButtons()
        {
            InitializeComponent();
        }


        public string WindowName
        {
            get => (string)GetValue(WindowNameProperty);
            set => SetValue(WindowNameProperty, value);
        }
        public static readonly DependencyProperty WindowNameProperty = DependencyProperty.Register("WindowName", typeof(string), typeof(WindowButtons));

        public Visibility AutoDimButtonVisiblity
        {
            get => (Visibility)GetValue(AutoDimButtonVisiblityProperty);
            set => SetValue(AutoDimButtonVisiblityProperty, value);
        }
        public static readonly DependencyProperty AutoDimButtonVisiblityProperty = DependencyProperty.Register("AutoDimButtonVisiblity", typeof(Visibility), typeof(WindowButtons));

        public Visibility HideButtonVisibility
        {
            get => (Visibility)GetValue(HideButtonVisibilityProperty);
            set => SetValue(HideButtonVisibilityProperty, value);
        }
        public static readonly DependencyProperty HideButtonVisibilityProperty = DependencyProperty.Register("HideButtonVisibility", typeof(Visibility), typeof(WindowButtons));

        private void Hide(object sender, RoutedEventArgs e)
        {
            Dc.Visible = false;
        }

        private void Pin(object sender, RoutedEventArgs e)
        {
            Dc.ShowAlways = !Dc.ShowAlways;
        }


        private void Close(object sender, RoutedEventArgs e)
        {
            Dc.Enabled = false;
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            //_dc = DataContext as WindowSettings;
        }

        private void AutoDim(object sender, RoutedEventArgs e)
        {
            Dc.AutoDim = !Dc.AutoDim;
        }

        private void MakeGlobal(object sender, RoutedEventArgs e)
        {
            Dc.MakePositionsGlobal();
        }
    }
}
