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
    /// Logica di interazione per GearItemControl.xaml
    /// </summary>
    public partial class GearItemControl : UserControl
    {
        private DoubleAnimation _anim;
        public GearItemControl()
        {
            InitializeComponent();
            _anim = new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(350)) { EasingFunction = new QuadraticEase() };
            _anim.BeginTime = TimeSpan.FromMilliseconds(200);
        }

        private void GearItemControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (GearItem)DataContext;
            if (dc == null) return;
            _anim.To = dc.LevelFactor * 359.9;
            MainArc.BeginAnimation(Arc.EndAngleProperty, _anim);

        }
    }
}
