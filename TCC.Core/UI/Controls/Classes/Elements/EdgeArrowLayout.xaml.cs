using System.Collections.Generic;
using System.Windows;

namespace TCC.UI.Controls.Classes.Elements;

/// <summary>
/// Logica di interazione per EdgeArrowLayout.xaml
/// </summary>
public partial class EdgeArrowLayout
{
    public EdgeArrowLayout()
    {
        InitializeComponent();
    }

    public override List<FrameworkElement> EdgeElements
    {
        get
        {
            var ret = new List<FrameworkElement>();
            Dispatcher.Invoke(() =>
            {

                for (var i = 4; i >= 0; i--)
                {
                    ret.Add((FrameworkElement) Edge5To1.Children[i]);
                }

                for (var i = 4; i >= 0; i--)
                {
                    ret.Add((FrameworkElement) Edge10To6.Children[i]);
                }
            });
            return ret;
        }
    }
    //private void OnEdgePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    switch (e.PropertyName)
    //    {
    //        case nameof(Counter.Val):
    //            var rects = GetSortedEdge();
    //            for (int i = 0; i < 10; i++)
    //            {
    //                if (i < _dc.Val)
    //                {
    //                    rects[i].Opacity = 1;
    //                    //((Rectangle) rects[i]).Fill = i < 8 ? i == 7 ? Application.Current.FindResource("AquadraxBrush") as SolidColorBrush :
    //                    //                                                Application.Current.FindResource("IgnidraxBrush") as SolidColorBrush :
    //                    //                                                Application.Current.FindResource("HpBrush") as SolidColorBrush;
    //                }
    //                else
    //                {
    //                    rects[i].Opacity = 0.1;
    //                    //((Rectangle) rects[i]).Fill = Brushes.White;
    //                }
    //            }
    //            //if (_dc.Val == 8 || _dc.Val == 10)
    //            //{
    //            //    if (_dc.Val == 10)
    //            //    {
    //            //        MainEdgeGrid.Effect =
    //            //            new DropShadowEffect { BlurRadius = 15, ShadowDepth = 0, Color = Colors.White };
    //            //    }
    //            //}
    //            //else
    //            //{
    //            //    MainEdgeGrid.Effect = Application.Current.FindResource("DropShadow") as DropShadowEffect;
    //            //}
    //            break;
    //    }

    //}
}