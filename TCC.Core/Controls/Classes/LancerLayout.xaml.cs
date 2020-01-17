using System;
using System.Windows.Media.Animation;
using FoglioUtils.Controls;
using TCC.ViewModels;

namespace TCC.Controls.Classes
{
    // TODO: refactor dis
    public partial class LancerLayout
    {
        private LancerLayoutVM _dc;
        private DoubleAnimation _lineHeldDurationAn;
        private DoubleAnimation _lineHeldStacksAn;

        public LancerLayout()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _dc = DataContext as LancerLayoutVM;
            _lineHeldDurationAn = new DoubleAnimation { Duration = TimeSpan.FromMilliseconds(150) };
            _lineHeldStacksAn = new DoubleAnimation();
            Timeline.SetDesiredFrameRate(_lineHeldDurationAn, 20);
            Timeline.SetDesiredFrameRate(_lineHeldStacksAn, 30);
            //if (_dc != null) _dc.LH.PropertyChanged += OnLineHeldPropertyChanged;
            if (_dc == null) return;
            _dc.LH.BaseStacksChanged += OnStacksChanged;
            _dc.LH.BaseBuffStarted += OnLineHeldRefreshed;
            _dc.LH.BaseBuffRefreshed += OnLineHeldRefreshed;
            _dc.LH.BuffEnded += OnLineHeldEnded;
        }

        private void OnLineHeldEnded()
        {
            Dispatcher?.Invoke(() =>
            {
                _lineHeldStacksAn.To = 42;
                _lineHeldStacksAn.Duration = TimeSpan.FromMilliseconds(150);
                SecReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldStacksAn);
                MainReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldStacksAn);
            });

        }

        //private void OnLineHeldPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName != nameof(_dc.LH.Val)) return;
        //    Dispatcher.Invoke(() =>
        //    {
        //        for (var i = 0; i < _dc.LH.Max; i++)
        //        {
        //            LineHeldContainer.Children[i].Opacity = i <= _dc.LH.Val - 1 ? 1 : 0;
        //        }
        //    });
        //}
        private void OnStacksChanged(int stacks)
        {
            Dispatcher?.Invoke(() =>
            {
                _lineHeldStacksAn.To = stacks / 5D * 280 + 42;
                _lineHeldStacksAn.Duration = TimeSpan.FromMilliseconds(150);
                SecReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldStacksAn);
            });
        }
        private void OnLineHeldRefreshed(long duration)
        {
            Dispatcher?.Invoke(() =>
            {
                _lineHeldDurationAn.From = 318;
                _lineHeldDurationAn.To = 42;
                _lineHeldDurationAn.Duration = TimeSpan.FromMilliseconds(duration);
                MainReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldDurationAn);
            });
        }
    }
}
