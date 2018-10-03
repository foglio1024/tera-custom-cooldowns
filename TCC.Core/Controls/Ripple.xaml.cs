using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for Ripple.xaml
    /// </summary>
    public partial class Ripple
    {
        private const int AnimTime = 650;

        public Ripple()
        {
            InitializeComponent();
            _scaleRipple = new DoubleAnimation(0, 20, TimeSpan.FromMilliseconds(AnimTime)) { EasingFunction = new QuadraticEase() };
            _fadeRipple = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(AnimTime)) { EasingFunction = new QuadraticEase() };
        }

        private readonly DoubleAnimation _scaleRipple;
        private readonly DoubleAnimation _fadeRipple;
        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var scaleTrans = (RippleCircle.RenderTransform as TransformGroup)?.Children[0];
            ((TransformGroup) RippleCircle.RenderTransform).Children[1] = new TranslateTransform(
                e.MouseDevice.GetPosition(this).X - RippleCircle.Width / 2,
                e.MouseDevice.GetPosition(this).Y - RippleCircle.Height / 2);
            var fac = (ActualWidth * ActualHeight) / 100;
            _scaleRipple.To = fac + 5;
            _scaleRipple.Duration = TimeSpan.FromMilliseconds((fac/30)* AnimTime);
            if (scaleTrans != null)
            {
                scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, _scaleRipple);
                scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, _scaleRipple);
            }

            RippleCircle.BeginAnimation(OpacityProperty, _fadeRipple);
        }
    }
}
