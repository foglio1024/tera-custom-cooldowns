using System.ComponentModel;
using System.Windows;
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
            Loaded += (_, __) =>
            {
                _dc = DataContext as WarriorBarManager;
                if (_dc != null) _dc.EdgeCounter.PropertyChanged += OnEdgePropertyChanged;
                //else Console.WriteLine("[EdgeBarLayout] DataContext is null!");
            };
        }

        private void OnEdgePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Counter.Val)) return;
            for (var i = 0; i < _dc.EdgeCounter.MaxValue; i++)
            {
                if (i < _dc.EdgeCounter.Val)
                {
                    EdgeContainer.Children[i].Opacity = 1;
                    ((Shape) EdgeContainer.Children[i]).Fill = i < 8 ? i == 7 ?  Application.Current.FindResource("AquadraxBrush") as SolidColorBrush :
                                                                                 Application.Current.FindResource("IgnidraxBrush") as SolidColorBrush :
                                                                                 Application.Current.FindResource("HpBrush") as SolidColorBrush;

                }
                else
                {
                    EdgeContainer.Children[i].Opacity = .1;
                    ((Shape) EdgeContainer.Children[i]).Fill = Brushes.White;
                }
            }
        }
    }
}