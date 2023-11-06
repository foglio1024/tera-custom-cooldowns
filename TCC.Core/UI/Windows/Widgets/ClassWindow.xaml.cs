using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class ClassWindow 
{
    public ClassWindow(ClassWindowViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        ButtonsRef = Buttons;
        BoundaryRef = Boundary;
        MainContent = WindowContent;
        Init(App.Settings.ClassWindowSettings);
    }
}