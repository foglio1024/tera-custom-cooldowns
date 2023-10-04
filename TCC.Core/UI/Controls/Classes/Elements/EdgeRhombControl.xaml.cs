using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TCC.UI.Controls.Classes.Elements;

/// <summary>
/// Logica di interazione per EdgeRhombControl.xaml
/// </summary>
public partial class EdgeRhombControl
{
    public EdgeRhombControl()
    {
        InitializeComponent();
    }

    public override List<FrameworkElement> EdgeElements
    {
        get
        {
            var ret = new List<FrameworkElement>();
            Dispatcher.Invoke(() => ret.AddRange(Container.Children.Cast<FrameworkElement>()));
            return ret;
        }
    }
}