using System.Windows;
using TCC.ViewModels.Configuration.Abnormalities;

namespace TCC.UI.Windows.Configuration.Abnormalities;

public partial class AbnormalityConfigWindow
{
    public AbnormalityConfigWindow() : base(true)
    {
        DataContext = new AbnormalityConfigViewModel();
        InitializeComponent();
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void FilterButton_OnClick(object sender, RoutedEventArgs e)
    {
        FilterPopup.IsOpen = !FilterPopup.IsOpen;
    }
}
