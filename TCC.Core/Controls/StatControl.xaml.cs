using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per StatControl.xaml
    /// </summary>
    public partial class StatControl : UserControl, INotifyPropertyChanged
    {
        public StatControl()
        {
            InitializeComponent();
        }


        public double Angle
        {
            get
            {
                if (_context != null) return _context.Factor * 359.9;
                return 0;

            }
        }
        private SolidColorBrush currentColor;
        public SolidColorBrush CurrentColor
        {
            get => currentColor;
            set
            {
                if (currentColor == value) return;
                currentColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentColor"));
            }
        }
        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(StatControl));

        public SolidColorBrush StatusColor
        {
            get { return (SolidColorBrush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }
        public static readonly DependencyProperty StatusColorProperty = DependencyProperty.Register("StatusColor", typeof(SolidColorBrush), typeof(StatControl));


        StatTracker _context;

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (StatTracker)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
            if (StatusColor == null) StatusColor = Color;
            CurrentColor = Color;
        }

        private void _context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Factor")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Angle"));
            }
            if (e.PropertyName == "Status")
            {
                if (_context.Status)
                {
                    CurrentColor = StatusColor;

                }
                else
                {
                    CurrentColor = Color;
                }
            }
        }
    }
}
