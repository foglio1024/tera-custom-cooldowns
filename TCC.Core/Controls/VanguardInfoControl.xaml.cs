using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for VanguardInfoControl.xaml
    /// </summary>
    public partial class VanguardInfoControl : UserControl
    {
        TimeSpan growDuration;
        DoubleAnimation scaleUp;
        DoubleAnimation moveUp;
        DoubleAnimation scaleDown;
        DoubleAnimation moveDown;
        DoubleAnimation scaleRipple;
        DoubleAnimation fadeRipple;


        public VanguardInfoControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, scaleUp);
            rootBorder.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, moveUp);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, scaleDown);
            rootBorder.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, moveDown);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            growDuration = TimeSpan.FromMilliseconds(150);
            scaleUp = new DoubleAnimation(-3, growDuration) { EasingFunction = new QuadraticEase() };
            moveUp =  new DoubleAnimation(8, growDuration) { EasingFunction = new QuadraticEase() };
            scaleDown = new DoubleAnimation(0, growDuration) { EasingFunction = new QuadraticEase() };
            moveDown = new DoubleAnimation(3, growDuration) { EasingFunction = new QuadraticEase() };
            scaleRipple = new DoubleAnimation(0, 7, TimeSpan.FromMilliseconds(650)) { EasingFunction = new QuadraticEase() };
            fadeRipple = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(650));
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var scaleTrans = (ripple.RenderTransform as TransformGroup).Children[0];
            (ripple.RenderTransform as TransformGroup).Children[1] = new TranslateTransform(e.MouseDevice.GetPosition(this).X - ripple.Width / 2, e.MouseDevice.GetPosition(this).Y - ripple.Height / 2);

            scaleTrans.BeginAnimation(ScaleTransform.ScaleXProperty, scaleRipple);
            scaleTrans.BeginAnimation(ScaleTransform.ScaleYProperty, scaleRipple);
            ripple.BeginAnimation(OpacityProperty, fadeRipple);
            InfoWindowViewModel.Instance.SelectCharacter((Character)DataContext);

        }
    }
}
