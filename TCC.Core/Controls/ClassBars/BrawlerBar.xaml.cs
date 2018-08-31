using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ClassBars
{
    /// <summary>
    /// Logica di interazione per BrawlerBar.xaml
    /// </summary>
    public partial class BrawlerBar
    {
        public BrawlerBar()
        {
            InitializeComponent();
        }
        private BrawlerBarManager _dc;
        private DoubleAnimation _an;
        private DoubleAnimation _counter;
        private bool _counterAnimating;

        private void BrawlerBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (BrawlerBarManager)DataContext;
            _an = new DoubleAnimation(_dc.StaminaTracker.Factor * 359.99 + 40, TimeSpan.FromMilliseconds(150));
            _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;
            _dc.Counter.PropertyChanged += AnimateCounter;

            _counter = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            _counter.Completed += (_, __) => _counterAnimating = false;
        }

        

        private void ST_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
            _an.To = _dc.StaminaTracker.Factor*(359.99 - 80) + 40;
            MainReArc.BeginAnimation(Arc.EndAngleProperty,_an);

            //if (_dc.StaminaTracker.Factor == 1)
            //{
            //    MainReArc.Stroke = Brushes.Orange;
            //    ((DropShadowEffect)MainReArcGrid.Effect).Opacity = 1;
            //    ((DropShadowEffect)BgImage.Effect).Opacity = 1;
            //}
            //else
            //{
            //    MainReArc.Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x99, 0x33));
            //    ((DropShadowEffect)MainReArcGrid.Effect).Opacity = 0;
            //    ((DropShadowEffect)BgImage.Effect).Opacity = 0;
            //}
        }

        private void AnimateCounter(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                //_counter.To = _dc.Counter.Factor * 359.9;
                _counterAnimating = true;
                //CounterArc.BeginAnimation(Arc.EndAngleProperty, _counter);
            }
        }

    }
}
