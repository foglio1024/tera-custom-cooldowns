using System.Windows.Input;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

/// <summary>
/// Logica di interazione per CivilUnrestWindow.xaml
/// </summary>
public partial class CivilUnrestWindow 
{
    private readonly CivilUnrestViewModel _vm;
    

    public CivilUnrestWindow(CivilUnrestViewModel vm)
    {
        InitializeComponent();
        MainContent = WindowContent;
        BoundaryRef = Boundary;
        DataContext = vm;
        _vm = (CivilUnrestViewModel) DataContext;

        Init(App.Settings.CivilUnrestWindowSettings);
    }

    private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _vm.CopyToClipboard();
    }
}