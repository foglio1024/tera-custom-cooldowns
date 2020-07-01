using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TCC.UI.Controls.Classes.Elements
{
    public partial class EdgeBarLayout
    {

        public EdgeBarLayout()
        {
            InitializeComponent();
        }

        public override List<FrameworkElement> EdgeElements
        {
            get
            {
                var ret = new List<FrameworkElement>();
                Dispatcher.Invoke(() => ret.AddRange(EdgeContainer.Children.Cast<FrameworkElement>()));
                return ret.ToList();

            }
        }

        //private void OnEdgePropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName != nameof(Counter.Val)) return;
        //    for (var i = 0; i < _dc.EdgeCounter.MaxValue; i++)
        //    {
        //        if (i < _dc.EdgeCounter.Val)
        //        {
        //            EdgeContainer.Children[i].Opacity = 1;
        //            ((Shape) EdgeContainer.Children[i]).Fill = i < 8 ? i == 7 ?  Application.Current.FindResource("AquadraxBrush") as SolidColorBrush :
        //                                                                         Application.Current.FindResource("IgnidraxBrush") as SolidColorBrush :
        //                                                                         Application.Current.FindResource("HpBrush") as SolidColorBrush;

        //        }
        //        else
        //        {
        //            EdgeContainer.Children[i].Opacity = .1;
        //            ((Shape) EdgeContainer.Children[i]).Fill = Brushes.White;
        //        }
        //    }
        //}
    }
}