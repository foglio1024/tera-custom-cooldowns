using System;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class GroupWindow
{
    public GroupWindow(GroupWindowViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        BoundaryRef = Boundary;
        ButtonsRef = Buttons;
        MainContent = WindowContent;
        Init(vm.Settings!);
    }

    private void GroupWindow_OnMouseEnter(object sender, MouseEventArgs e)
    {
        GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
    }

    private void GroupWindow_OnMouseLeave(object sender, MouseEventArgs e)
    {
        GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
    }
}