using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for EventControl.xaml
    /// </summary>
    public partial class EventControl : UserControl
    {
        DoubleAnimation scaleUp;
        DoubleAnimation scaleDown;
        public EventControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            border.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
            border.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            border.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDown);
            border.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDown);
        }

        private DailyEvent dc;
        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            dc = (DailyEvent) DataContext;
            var fac = 1 / ((1 / .45)*Math.Pow(Math.E,8*dc.DurationFactor) + 1 / (2 * .45));
            scaleUp = new DoubleAnimation(1.01, TimeSpan.FromMilliseconds(800)) { EasingFunction = new ElasticEase() };
            scaleDown = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };

        }
    }
}
