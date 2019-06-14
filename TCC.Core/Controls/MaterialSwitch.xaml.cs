using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls
{
    public partial class MaterialSwitch
    {
        private readonly DoubleAnimation _on;
        private readonly DoubleAnimation _off;

        private readonly ColorAnimation _fillOn;
        private readonly ColorAnimation _fillOff;
        private readonly ColorAnimation _backFillOff;

        private readonly Color _onColor = R.Colors.MainColor;// Color.FromRgb(255, 56, 34);
        private readonly Color _offColor = ((SolidColorBrush)Application.Current.Resources["DefaultBackgroundBrush"]).Color;
        private readonly Color _backOffColor = Colors.Black;

        private readonly TimeSpan _animationDuration = TimeSpan.FromMilliseconds(150);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DependencyPropertyWatcher<bool> _dpw;
        public MaterialSwitch()
        {
            InitializeComponent();

            _on = new DoubleAnimation(20, _animationDuration) { EasingFunction = new QuadraticEase() };
            _off = new DoubleAnimation(0, _animationDuration) { EasingFunction = new QuadraticEase() };

            _fillOn = new ColorAnimation(_onColor, _animationDuration) { EasingFunction = new QuadraticEase() };
            _fillOff = new ColorAnimation(_offColor, _animationDuration) { EasingFunction = new QuadraticEase() };
            _backFillOff = new ColorAnimation(_backOffColor, _animationDuration) { EasingFunction = new QuadraticEase() };
            SwitchHead.RenderTransform = new TranslateTransform(0, 0);
            SwitchHead.Fill = new SolidColorBrush(_offColor);
            SwitchBack.Fill = new SolidColorBrush(_backOffColor);

            _dpw = new DependencyPropertyWatcher<bool>(this, "Status");
            _dpw.PropertyChanged += StatusWatcher_PropertyChanged;
        }

        private void StatusWatcher_PropertyChanged(object sender, EventArgs e)
        {
            if (Status)
            {
                AnimateOn();
            }
            else
            {
                AnimateOff();
            }
        }

        public bool Status
        {
            get => (bool)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(bool), typeof(MaterialSwitch), new PropertyMetadata(false));


        private void AnimateOn()
        {
            SwitchHead.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _on);
            SwitchHead.Fill.BeginAnimation(SolidColorBrush.ColorProperty, _fillOn);
            SwitchBack.Fill.BeginAnimation(SolidColorBrush.ColorProperty, _fillOn);
        }

        private void AnimateOff()
        {
            SwitchHead.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _off);
            SwitchHead.Fill.BeginAnimation(SolidColorBrush.ColorProperty, _fillOff);
            SwitchBack.Fill.BeginAnimation(SolidColorBrush.ColorProperty, _backFillOff);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Status)
                AnimateOn();
            else
                AnimateOff();
        }
    }
}
