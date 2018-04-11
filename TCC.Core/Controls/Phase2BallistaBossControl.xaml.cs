using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
