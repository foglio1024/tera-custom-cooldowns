using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per MaterialSwitch.xaml
    /// </summary>
    public partial class MaterialSwitch : UserControl
    {
        private DoubleAnimation on;
        private DoubleAnimation off;

        private ColorAnimation fillOn;
        private ColorAnimation fillOff;
        private ColorAnimation backFillOff;

        private Color onColor = Color.FromRgb(255, 56, 34);
        private Color offColor = ((SolidColorBrush)Application.Current.Resources["DefaultBackgroundColor"]).Color;
        private Color backOffColor = Colors.Black;

        private TimeSpan animationDuration = TimeSpan.FromMilliseconds(150);

        private DependencyPropertyWatcher<bool> statusWatcher;

        public MaterialSwitch()
        {
            InitializeComponent();

            on = new DoubleAnimation(20, animationDuration) { EasingFunction = new QuadraticEase() };
            off = new DoubleAnimation(0, animationDuration) { EasingFunction = new QuadraticEase() };

            fillOn = new ColorAnimation(onColor, animationDuration) {EasingFunction = new QuadraticEase() };
            fillOff = new ColorAnimation(offColor, animationDuration) { EasingFunction = new QuadraticEase() };
            backFillOff = new ColorAnimation(backOffColor, animationDuration) { EasingFunction = new QuadraticEase() };
            switchHead.RenderTransform = new TranslateTransform(0, 0);
            switchHead.Fill = new SolidColorBrush(offColor);
            switchBack.Fill = new SolidColorBrush(backOffColor);

            statusWatcher = new DependencyPropertyWatcher<bool>(this, "Status");
            statusWatcher.PropertyChanged += StatusWatcher_PropertyChanged;
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
            get { return (bool)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(bool), typeof(MaterialSwitch), new PropertyMetadata(false));


        public void AnimateOn()
        {
            switchHead.RenderTransform.BeginAnimation(TranslateTransform.XProperty, on);
            switchHead.Fill.BeginAnimation(SolidColorBrush.ColorProperty, fillOn);
            switchBack.Fill.BeginAnimation(SolidColorBrush.ColorProperty, fillOn);
        }
        public void AnimateOff()
        {
            switchHead.RenderTransform.BeginAnimation(TranslateTransform.XProperty, off);
            switchHead.Fill.BeginAnimation(SolidColorBrush.ColorProperty, fillOff);
            switchBack.Fill.BeginAnimation(SolidColorBrush.ColorProperty, backFillOff);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
    }
}
