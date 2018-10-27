using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Interaction logic for DungeonInfoControl.xaml
    /// </summary>
    public partial class DungeonInfoControl
    {
        private DoubleAnimation _bubbleScale;
        private DoubleAnimation _fadeIn;

        public DungeonInfoControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _bubbleScale = new DoubleAnimation(.9, 1, TimeSpan.FromMilliseconds(1000)) { EasingFunction = new ElasticEase() };
            _fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        }

        public void AnimateIn()
        {
            Dispatcher.Invoke(() =>
            {
                EntriesBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _bubbleScale);
                EntriesBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _bubbleScale);
                EntriesBubble.Child.BeginAnimation(OpacityProperty, _fadeIn);
            });
        }
    }
}
