using System.Windows.Input;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

/// <summary>
/// Logica di interazione per CivilUnrestWindow.xaml
/// </summary>
public partial class CivilUnrestWindow 
{
    CivilUnrestViewModel VM { get; }
    

    public CivilUnrestWindow(CivilUnrestViewModel vm)
    {
        InitializeComponent();
        MainContent = WindowContent;
        BoundaryRef = Boundary;
        DataContext = vm;
        VM = (CivilUnrestViewModel) DataContext;

        Init(App.Settings.CivilUnrestWindowSettings);
    }

    void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        VM.CopyToClipboard();
    }
}