using System.Runtime;
using System.Windows;
using TCC.ViewModels;

namespace TCC.UI.Windows;
public partial class LootDistributionWindow
{
    readonly LootDistributionViewModel _vm;
    public LootDistributionWindow(LootDistributionViewModel vm)
    {
        _vm = vm;
        DataContext = vm;


        SizeChanged += OnSizeChanged;
        InitializeComponent();

        BoundaryRef = Boundary;
        MainContent = WindowContent;
        Init(_vm.Settings!);

        _vm.Settings!.Visible = false;
    }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        base.OnLoaded(sender, e);
        FocusManager.HideFromToolBar(Handle);
        FocusManager.MakeUnfocusable(Handle);
    }

    void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        _vm.Settings.Visible = false;
    }



    void OnShowListButtonClick(object sender, RoutedEventArgs e)
    {
        SizeToContent = SizeToContent.Width;
    }

    void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_vm.Settings == null) return;
        if (_vm.Settings.IgnoreSize) return;
        if (_vm.Settings.W == ActualWidth && _vm.Settings.H == ActualHeight) return;
        _vm.Settings.W = ActualWidth;
        _vm.Settings.H = ActualHeight;
        if (!App.Loading) App.Settings.Save();
    }
}
