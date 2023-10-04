using System.Windows;

namespace TCC.UI.Windows;

public partial class LaurelSelectionWindow
{
    public LaurelSelectionWindow() : base(true)
    {
        InitializeComponent();
    }

    void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
