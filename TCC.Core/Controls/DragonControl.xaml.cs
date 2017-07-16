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
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Interaction logic for DragonControl.xaml
    /// </summary>
    public partial class DragonControl : UserControl
    {
        public DragonControl()
        {
            InitializeComponent();
            shieldArcAn = new DoubleAnimation(0, 359.99, TimeSpan.FromSeconds(15));

        }

        Boss dc;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dc = (Boss)DataContext;
            dc.PropertyChanged += Dc_PropertyChanged;
            enrageLine.LayoutTransform = dc.CurrentPercentage > dc.EnragePattern.Percentage ? new RotateTransform((dc.CurrentPercentage - dc.EnragePattern.Percentage) * 3.6) : new RotateTransform(0);

        }

        DoubleAnimation shieldArcAn;

        private void Dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(dc.Shield))
            {
                if (dc.Shield == ShieldStatus.On)
                {
                    shieldArcAn = new DoubleAnimation(0, 359.99, TimeSpan.FromSeconds(15));
                    shieldArc.BeginAnimation(Arc.EndAngleProperty, shieldArcAn);
                }
                else
                {
                    shieldArcAn.BeginTime = null;
                    shieldArc.BeginAnimation(Arc.EndAngleProperty, shieldArcAn);
                }
            }
            else if(e.PropertyName == nameof(dc.Enraged))
            {
                if (dc.Enraged)
                {
                    enrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.99, 0, TimeSpan.FromSeconds(dc.EnragePattern.Duration)));
                }
                else
                {
                    enrageLine.LayoutTransform = dc.CurrentPercentage > dc.EnragePattern.Percentage ? new RotateTransform((dc.CurrentPercentage - dc.EnragePattern.Percentage) * 3.6) : new RotateTransform(0);
                }
            }
            else if(e.PropertyName == nameof(dc.CurrentHP))
            {
                if (dc.Enraged)
                {
                    enrageLine.LayoutTransform = new RotateTransform(dc.CurrentFactor * 359.9);
                }
            }
        }

    }
}
