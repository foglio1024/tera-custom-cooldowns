using System.Windows.Input;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class PerfMonitorWindow 
{
    public PerfMonitorWindow(PerfMonitorViewModel vm)
    {
        DataContext = vm;

        InitializeComponent();

        BoundaryRef = Boundary;
        MainContent = WindowContent;

        Init(vm.Settings!);
    }

    private void PerfMonitorWindow_OnMouseEnter(object sender, MouseEventArgs e)
    {
        ((PerfMonitorViewModel) DataContext).ShowDumpButton = true;
    }

    private void PerfMonitorWindow_OnMouseLeave(object sender, MouseEventArgs e)
    {
        ((PerfMonitorViewModel) DataContext).ShowDumpButton = false;
    }
}