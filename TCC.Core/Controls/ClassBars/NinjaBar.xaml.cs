using System;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls.ClassBars
{
    /// <summary>
    /// Logica di interazione per NinjaBar.xaml
    /// </summary>
    public partial class NinjaBar
    {
        private NinjaBarManager _dc;
        private DoubleAnimation _an;

        public NinjaBar()
        {
            InitializeComponent();
        }

        private void NinjaBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (NinjaBarManager)DataContext;
            _an = new DoubleAnimation(_dc.ST.Factor * 359.99, TimeSpan.FromMilliseconds(150));

            _dc.ST.PropertyChanged += ST_PropertyChanged;
        }

        private void ST_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.ST.Factor)) return;
            _an.To = _dc.ST.Factor * 359.99;
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
        }
    }
}
