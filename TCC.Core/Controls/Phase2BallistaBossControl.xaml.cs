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
    /// Interaction logic for Phase2BallistaBossControl.xaml
    /// </summary>
    public partial class Phase2BallistaBossControl : UserControl
    {
        public Phase2BallistaBossControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            enrageEll.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(240, 0, TimeSpan.FromSeconds(240)));
        }
    }
}
