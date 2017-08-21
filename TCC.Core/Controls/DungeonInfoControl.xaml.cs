using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for DungeonInfoControl.xaml
    /// </summary>
    public partial class DungeonInfoControl : UserControl
    {
        TimeSpan growDuration;
        DoubleAnimation scaleUp;
        DoubleAnimation moveUp;
        DoubleAnimation scaleDown;
        DoubleAnimation moveDown;
        DoubleAnimation bubbleScale;
        DoubleAnimation fadeIn;

        public DungeonInfoControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, scaleUp);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            rootBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, scaleDown);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            growDuration = TimeSpan.FromMilliseconds(150);
            scaleUp = new DoubleAnimation(2, growDuration) { EasingFunction = new QuadraticEase() };
            moveUp = new DoubleAnimation(10, growDuration) { EasingFunction = new QuadraticEase() };
            scaleDown = new DoubleAnimation(0, growDuration) { EasingFunction = new QuadraticEase() };
            moveDown = new DoubleAnimation(4, growDuration) { EasingFunction = new QuadraticEase() };
            bubbleScale = new DoubleAnimation(.9, 1, TimeSpan.FromMilliseconds(1000)) { EasingFunction = new ElasticEase() };
            fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        }
        public void AnimateIn()
        {
            Dispatcher.Invoke(() =>
            {
                entriesBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, bubbleScale);
                entriesBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, bubbleScale);
                entriesBubble.Child.BeginAnimation(OpacityProperty, fadeIn);
            });
        }
    }
}
