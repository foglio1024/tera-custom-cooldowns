namespace TCC.UI.Windows.Widgets;

public partial class NotificationAreaWindow
{
    public NotificationAreaWindow(NotificationAreaViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        ButtonsRef = Buttons;
        MainContent = WindowContent;
        BoundaryRef = Boundary;
        Init(vm.Settings!);
        MainContent.Opacity = 1;
    }
}