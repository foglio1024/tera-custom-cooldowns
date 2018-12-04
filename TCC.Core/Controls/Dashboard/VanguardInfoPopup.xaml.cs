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
