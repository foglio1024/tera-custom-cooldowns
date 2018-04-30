using System;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per GearItemControl.xaml
    /// </summary>
    public partial class GearItemControl
    {
        private readonly DoubleAnimation _anim;
        public GearItemControl()
        {
            InitializeComponent();
            _anim = new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(350))
            {
                EasingFunction = new QuadraticEase(),
                BeginTime = TimeSpan.FromMilliseconds(200)
            };
        }

        private void GearItemControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (GearItem)DataContext;
            if (dc == null) return;
            _anim.To = dc.LevelFactor * 359.9;
            //MainArc.BeginAnimation(Arc.EndAngleProperty, _anim);

        }
    }
}
