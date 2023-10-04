using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class BossWindow
{
    public BossWindow(NpcWindowViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        ButtonsRef = Buttons;
        BoundaryRef = Boundary;
        MainContent = WindowContent;
        Init(App.Settings.NpcWindowSettings);
        MouseDoubleClick += (_, _) => vm.CopyToClipboard();
    }
}