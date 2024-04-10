using System.Windows;
using System.Windows.Controls;
using TCC.ViewModels;

namespace TCC.UI.Windows;

public partial class DashboardWindow
{
    private readonly DashboardViewModel _vm;

    public DashboardWindow(DashboardViewModel vm) : base(false)
    {
        InitializeComponent();
        DataContext = vm;
        _vm = vm;
        Showed += _vm.UpdateBuffs;
        Hidden += Game.DB!.DungeonDatabase.SaveCustomDefs;
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        HideWindow();
        _vm.SaveCharacters();
    }

    private void OnTabChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] is not TabItem) return;
        _vm.ShowDetails = false;
    }
}