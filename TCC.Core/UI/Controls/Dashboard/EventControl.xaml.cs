using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.UI.Controls.Dashboard
{
    /// <summary>
    /// Interaction logic for EventControl.xaml
    /// </summary>
    public partial class EventControl
    {
        private DoubleAnimation _scaleUp;
        private DoubleAnimation _scaleDown;

        public EventControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _scaleUp);
            Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _scaleUp);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _scaleDown);
            Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _scaleDown);
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            _scaleUp = new DoubleAnimation(1.01, TimeSpan.FromMilliseconds(800)) { EasingFunction = new ElasticEase() };
            _scaleDown = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };

        }
    }
}
