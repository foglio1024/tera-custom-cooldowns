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
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per BrawlerBar.xaml
    /// </summary>
    public partial class BrawlerBar : UserControl
    {
        public BrawlerBar()
        {
            InitializeComponent();
        }
        private BrawlerBarManager _dc;
        private DoubleAnimation _an;
        private void BrawlerBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (BrawlerBarManager)DataContext;
            _an = new DoubleAnimation(_dc.ST.Factor * 359.99, TimeSpan.FromMilliseconds(150));
            _dc.ST.PropertyChanged += ST_PropertyChanged;
        }

        private void ST_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_dc.ST.Factor))
            {
                _an.To = _dc.ST.Factor*359.99;
                MainReArc.BeginAnimation(Arc.EndAngleProperty,_an);
            }
        }

    }
}
