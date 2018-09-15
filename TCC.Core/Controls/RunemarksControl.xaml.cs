using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per RunemarksControl.xaml
    /// </summary>
    public partial class RunemarksControl : UserControl
    {
        private ValkyrieBarManager _dc;

        public RunemarksControl()
        {
            InitializeComponent();
            this.Loaded += (_, __) =>
            {
                _dc = DataContext as ValkyrieBarManager;
                if (_dc != null) _dc.RunemarksCounter.PropertyChanged += OnEdgePropertyChanged;
                //else Console.WriteLine("[EdgeBarLayout] DataContext is null!");
            };

        }
        private void OnEdgePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Counter.Val)) return;
            for (var i = 0; i < _dc.RunemarksCounter.MaxValue; i++)
            {
                if (i < _dc.RunemarksCounter.Val)
                {
                    RunemarkContainer.Children[i].Opacity = 1;
                    (RunemarkContainer.Children[i] as Shape).Fill = _dc.RunemarksCounter.Val == _dc.RunemarksCounter.MaxValue ?
                                    App.Current.FindResource("RunemarkColorMax") as SolidColorBrush :
                                    App.Current.FindResource("RunemarkColor") as SolidColorBrush;

                }
                else
                {
                    RunemarkContainer.Children[i].Opacity = .1;
                    (RunemarkContainer.Children[i] as Shape).Fill = Brushes.White;
                }
            }
            
            this.Effect = _dc.RunemarksCounter.Val == _dc.RunemarksCounter.MaxValue ? new DropShadowEffect
            {
                BlurRadius = 10,
                ShadowDepth = 0,
                Color= (App.Current.FindResource("RunemarkColorMax") as SolidColorBrush).Color
            } : null; 
        }


        //public RunemarksControl()
        //{
        //    InitializeComponent();
        //    BaseBorder.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x27));

        //}

        //private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Val")
        //    {
        //        SetRunes(_context.Val);
        //    }
        //    else if (e.PropertyName == "Maxed")
        //    {
        //        //baseBorder.Background = new SolidColorBrush(Color.FromRgb(0xff,0x98,0xbb));
        //        MaxBorder.Opacity = 1;
        //    }
        //}
        //private int _currentRunes = 0;

        //private void SetRunes(int newRunes)
        //{
        //    var diff = newRunes - _currentRunes;

        //    if (diff == 0) return;
        //    if (diff > 0)
        //    {
        //        for (var i = 0; i < diff; i++)
        //        {
        //            DotsContainer.Children[_currentRunes + i].Opacity = 1;
        //        }
        //    }
        //    else
        //    {
        //        //baseBorder.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x27));
        //        MaxBorder.Opacity = 0;

        //        for (var i = DotsContainer.Children.Count - 1; i >= 0; i--)
        //        {
        //            DotsContainer.Children[i].Opacity = 0;
        //        }
        //    }
        //    _currentRunes = newRunes;
        //}

        //private Counter _context;

    }
}
