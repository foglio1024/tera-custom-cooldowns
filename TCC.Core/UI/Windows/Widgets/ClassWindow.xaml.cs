using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class ClassWindow 
{
    readonly ClassWindowViewModel _vm;

    public ClassWindow(ClassWindowViewModel vm)
    {
        DataContext = vm;
        _vm = (ClassWindowViewModel) DataContext;
        InitializeComponent();
        ButtonsRef = Buttons;
        BoundaryRef = Boundary;
        MainContent = WindowContent;
        Init(App.Settings.ClassWindowSettings);

        //if (!Game.Logged) return;
        //if (VM.CurrentManager != null) return;
        //VM.CurrentClass = Game.Me.Class;

    }

    protected override void OnEnabledChanged(bool enabled)
    {
        base.OnEnabledChanged(enabled);
        if (enabled) _vm.CurrentClass = Game.Me.Class;
    }
}