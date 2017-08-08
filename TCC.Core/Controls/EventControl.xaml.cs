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
    /// Interaction logic for EventControl.xaml
    /// </summary>
    public partial class EventControl : UserControl
    {
        DoubleAnimation scaleUp;
        DoubleAnimation scaleDown;
        public EventControl()
        {
            InitializeComponent();
            scaleUp = new DoubleAnimation(1.5, TimeSpan.FromMilliseconds(800)) {EasingFunction = new ElasticEase() };
            scaleDown = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150)) {EasingFunction = new QuadraticEase() };
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
    }
}
