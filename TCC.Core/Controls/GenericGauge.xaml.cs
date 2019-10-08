using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FoglioUtils;

namespace TCC.Controls
{
    public partial class GenericGauge : INotifyPropertyChanged
    {
        private const int AnimTime = 200;
        private readonly DoubleAnimation _a;
        private readonly DependencyPropertyWatcher<float> _curValwatcher; //https://blogs.msdn.microsoft.com/flaviencharlon/2012/12/07/getting-change-notifications-from-any-dependency-property-in-windows-store-apps/
        public GenericGauge()
        {
            InitializeComponent();
            
            _curValwatcher = new DependencyPropertyWatcher<float>(this, "CurrentVal");
            _curValwatcher.PropertyChanged += CurValWatcher_PropertyChanged;
            _a = new DoubleAnimation(0, TimeSpan.FromMilliseconds(AnimTime)) { EasingFunction = new QuadraticEase() };
            Bar.RenderTransform = new ScaleTransform(1, 1, 0, .5);

        }

        private void CurValWatcher_PropertyChanged(object sender, EventArgs e)
        {
            Factor = MaxVal > 0 ? _curValwatcher.Value / MaxVal : 0;
        }

        private double _factor;
        public double Factor
        {
            get => _factor;
            set
            {
                if (_factor == value) return;
                _factor = value;               
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Factor)));
                AnimateBar(value);
            }
        }

        private void AnimateBar(double factor)
        {
            if (factor > 1)
            {
                _a.To = 1;
            }
            else
            {
                _a.To = factor;
            }

            Bar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _a);

        }

        public SolidColorBrush BarColor
        {
            get => (SolidColorBrush)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }
        public static readonly DependencyProperty BarColorProperty = DependencyProperty.Register("BarColor", typeof(SolidColorBrush), typeof(GenericGauge));

        public string GaugeName
        {
            get => (string)GetValue(GaugeNameProperty);
            set => SetValue(GaugeNameProperty, value);
        }
        public static readonly DependencyProperty GaugeNameProperty = DependencyProperty.Register("GaugeName", typeof(string), typeof(GenericGauge));

        public int MaxVal
        {
            get => (int)GetValue(MaxValProperty);
            set => SetValue(MaxValProperty, value);
        }
        public static readonly DependencyProperty MaxValProperty = DependencyProperty.Register("MaxVal", typeof(int), typeof(GenericGauge));
        
        public float CurrentVal
        {
            get => (float)GetValue(CurrentValProperty);
            set => SetValue(CurrentValProperty, value);
        }
        public static readonly DependencyProperty CurrentValProperty = DependencyProperty.Register("CurrentVal", typeof(float), typeof(GenericGauge));

        public bool ShowPercentage
        {
            get => (bool)GetValue(ShowPercentageProperty);
            set => SetValue(ShowPercentageProperty, value);
        }
        public static readonly DependencyProperty ShowPercentageProperty = DependencyProperty.Register("ShowPercentage", typeof(bool), typeof(GenericGauge));
               
        public bool ShowValues
        {
            get => (bool)GetValue(ShowValuesProperty);
            set => SetValue(ShowValuesProperty, value);
        }
        public static readonly DependencyProperty ShowValuesProperty = DependencyProperty.Register("ShowValues", typeof(bool), typeof(GenericGauge));

        public bool ShowName
        {
            get => (bool)GetValue(ShowNameProperty);
            set => SetValue(ShowNameProperty, value);
        }
        public static readonly DependencyProperty ShowNameProperty = DependencyProperty.Register("ShowName", typeof(bool), typeof(GenericGauge));

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}