using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ClassBars
{
    public partial class EdgeBarLayout
    {
        private WarriorBarManager _dc;

        public EdgeBarLayout()
        {
            InitializeComponent();
            this.Loaded += (_, __) =>
            {
                _dc = DataContext as WarriorBarManager;
                if (_dc != null) _dc.EdgeCounter.PropertyChanged += OnEdgePropertyChanged;
                else Console.WriteLine("[EdgeBarLayout] DataContext is null!");
            };
        }

        private void OnEdgePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Counter.Val)) return;
            for (var i = 0; i < 10; i++)
            {
                if (i < _dc.EdgeCounter.Val)
                {
                    EdgeContainer.Children[i].Opacity = 1;
                    (EdgeContainer.Children[i] as Shape).Fill = i < 8 ? i == 7 ? App.Current.FindResource("AquadraxColor") as SolidColorBrush :
                                                                                 App.Current.FindResource("IgnidraxColor") as SolidColorBrush :
                                                                                 App.Current.FindResource("HpColor") as SolidColorBrush;

                }
                else
                {
                    EdgeContainer.Children[i].Opacity = .1;
                    (EdgeContainer.Children[i] as Shape).Fill = Brushes.White;
                }
            }
        }
    }
}