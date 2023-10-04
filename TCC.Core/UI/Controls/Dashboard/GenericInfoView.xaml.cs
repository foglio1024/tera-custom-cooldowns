using Nostrum.WPF.Extensions;
using System.Windows.Controls;

namespace TCC.UI.Controls.Dashboard;

public partial class GenericInfoView
{
    public GenericInfoView()
    {
        InitializeComponent();
    }

    void ContentListOnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        CharNames.FindVisualChild<ScrollViewer>()?.ScrollToVerticalOffset(e.VerticalOffset);
    }

    void CharNames_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        ContentList.FindVisualChild<ScrollViewer>()?.ScrollToVerticalOffset(e.VerticalOffset);
    }
}