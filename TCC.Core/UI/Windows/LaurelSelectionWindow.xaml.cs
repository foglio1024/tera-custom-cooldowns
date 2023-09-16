using System.Windows;

namespace TCC.UI.Windows;

public partial class LaurelSelectionWindow : TccWindow
{
    public LaurelSelectionWindow() : base(true)
    {
        InitializeComponent();
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
