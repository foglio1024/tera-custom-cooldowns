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
    /// Interaction logic for Ripple.xaml
    /// </summary>
    public partial class Ripple : UserControl
    {
        public Ripple()
        {
            InitializeComponent();
            scaleRipple = new DoubleAnimation(0, 20, TimeSpan.FromMilliseconds(650)) { EasingFunction = new QuadraticEase() };
            fadeRipple = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(650)) { EasingFunction = new QuadraticEase() };
        }
        DoubleAnimation scaleRipple;
        DoubleAnimation fadeRipple;
        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var scaleTrans = (ripple.RenderTransform as TransformGroup).Children[0];
            (ripple.RenderTransform as TransformGroup).Children[1] = new TranslateTransform(e.MouseDevice.GetPosition(this).X - ripple.Width / 2, e.MouseDevice.GetPosition(this).Y - ripple.Height / 2);
            var fac = (this.ActualWidth * this.ActualHeight) / 100;
            scaleRipple.To = fac + 5;
            scaleRipple.Duration = TimeSpan.FromMilliseconds((fac/30)*650);
            scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, scaleRipple);
            scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, scaleRipple);
            ripple.BeginAnimation(OpacityProperty, fadeRipple);
        }
    }
}
