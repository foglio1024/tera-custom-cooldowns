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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per MaterialSwitch.xaml
    /// </summary>
    public partial class MaterialSwitch : UserControl
    {
        DoubleAnimation on;
        DoubleAnimation off;

        ColorAnimation fillOn;
        ColorAnimation fillOff;
        ColorAnimation backFillOff;

        Color onColor = Color.FromRgb(255, 56, 34);
        Color offColor = ((SolidColorBrush)Application.Current.Resources["Colors.App.DefaultBackground"]).Color;
        Color backOffColor = Colors.Black;

        private TimeSpan animationDuration = TimeSpan.FromMilliseconds(150);

        DependencyPropertyWatcher<bool> statusWatcher;

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
