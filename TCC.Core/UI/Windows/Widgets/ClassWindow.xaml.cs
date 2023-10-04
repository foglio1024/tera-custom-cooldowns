using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class ClassWindow 
{
    ClassWindowViewModel VM { get; }

    public ClassWindow(ClassWindowViewModel vm)
    {
        DataContext = vm;
        VM = (ClassWindowViewModel) DataContext;
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
        if (enabled) VM.CurrentClass = Game.Me.Class;
    }
}