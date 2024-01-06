using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.Interop.Proxy;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class GroupWindow
{
    readonly GroupWindowViewModel _vm;

    public GroupWindow(GroupWindowViewModel vm)
    {
        DataContext = vm;
        _vm = vm;
        InitializeComponent();
        BoundaryRef = Boundary;
        ButtonsRef = Buttons;
        MainContent = WindowContent;
        Init(_vm.Settings!);
    }

    //TODO: to commands in VM
    void DisbandButtonClicked(object sender, RoutedEventArgs e)
    {
        if(!_vm.AmILeader) return;
        StubInterface.Instance.StubClient.DisbandGroup(); //ProxyOld.DisbandParty();
    }

    //TODO: to commands in VM
    void ResetButtonClicked(object sender, RoutedEventArgs e)
    {
        if(!_vm.AmILeader) return;
        StubInterface.Instance.StubClient.ResetInstance(); //ProxyOld.ResetInstance();
    }

    void GroupWindow_OnMouseEnter(object sender, MouseEventArgs e)
    {
        GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
        GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
    }

    void GroupWindow_OnMouseLeave(object sender, MouseEventArgs e)
    {
        GroupButtonsSingle.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
        GroupButtons.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { BeginTime = TimeSpan.FromMilliseconds(500) });
    }

    //TODO: to commands in VM
    void LeaveParty(object sender, RoutedEventArgs e)
    {
        StubInterface.Instance.StubClient.LeaveGroup(); //ProxyOld.LeaveParty();
    }

    //TODO: to commands in VM
    void ShowAbnormalSettings(object sender, RoutedEventArgs e)
    {
        new GroupAbnormalConfigWindow().ShowWindow();
        //WindowManager.GroupAbnormalConfigWindow.ShowWindow();
    }
}