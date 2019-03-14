using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls.Classes
{
    /// <summary>
    /// Logica di interazione per BrawlerLayout.xaml
    /// </summary>
    public partial class BrawlerLayout
    {
        public BrawlerLayout()
        {
            InitializeComponent();
        }
        private BrawlerLayoutVM _dc;
        private DoubleAnimation _an;

        private void BrawlerLayout_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (BrawlerLayoutVM)DataContext;
            _an = new DoubleAnimation(_dc.StaminaTracker.Factor * 359.99 + 40, TimeSpan.FromMilliseconds(150));
            _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;
        }

        private void ST_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
            _an.To = _dc.StaminaTracker.Factor*(359.99 - 2*MainReArc.StartAngle) + MainReArc.StartAngle;
            MainReArc.BeginAnimation(Arc.EndAngleProperty,_an);
        }
    }
}
