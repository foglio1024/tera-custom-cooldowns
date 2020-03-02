using System;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.Controls;
using TCC.ViewModels;

namespace TCC.UI.Controls.Classes
{
    /// <summary>
    /// Logica di interazione per GunnerLayout.xaml
    /// </summary>
    public partial class GunnerLayout
    {
        public GunnerLayout()
        {
            InitializeComponent();
        }

        private GunnerLayoutVM _dc;
        private DoubleAnimation _an;
        private void GunnerLayout_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (GunnerLayoutVM) DataContext;
            _an = new DoubleAnimation(_dc.StaminaTracker.Factor * 359.99 + 40, TimeSpan.FromMilliseconds(150));
            _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;

        }

        private void ST_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
            _an.To = _dc.StaminaTracker.Factor*(359.99 - MainReArc.StartAngle*2) + MainReArc.StartAngle;
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
        }
    }
}
