using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Logica di interazione per VanguardInfoPopup.xaml
    /// </summary>
    public partial class VanguardInfoPopup : UserControl
    {
        public VanguardInfoPopup()
        {
            InitializeComponent();
            Loaded += VanguardInfoPopup_Loaded;
        }

        private void VanguardInfoPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var ease = new ElasticEase() {Oscillations = 1};
            var time = TimeSpan.FromMilliseconds(500);
            var time2 = TimeSpan.FromMilliseconds(250);
            var an = new DoubleAnimation(.95, 1, time){EasingFunction = ease};
            var an2 = new DoubleAnimation(0, 1, time2);
            this.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
            this.BeginAnimation(OpacityProperty, an2);
        }
    }
}
