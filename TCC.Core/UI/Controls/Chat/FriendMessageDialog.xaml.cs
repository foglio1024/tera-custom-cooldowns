using System.Windows;
using System.Windows.Controls;
using TCC.Interop.Proxy;

namespace TCC.UI.Controls.Chat;

/// <summary>
/// Logica di interazione per FriendMessageDialog.xaml
/// </summary>
public partial class FriendMessageDialog
{
    public FriendMessageDialog()
    {
        InitializeComponent();
        TargetName = WindowManager.ViewModels.PlayerMenuVM.Name;
    }

    private string _message = "Friend me?";
    public string TargetName { get; set; }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _message = ((TextBox)sender).Text;
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
        _message = "Friend me?";
        //ChatWindowManager.Instance.LockTooltip();
        Close();
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
        StubInterface.Instance.StubClient.FriendUser(WindowManager.ViewModels.PlayerMenuVM.Name, _message);//ProxyOld.FriendRequest(WindowManager.FloatingButton.TooltipInfo.Name, _message);
        //ChatWindowManager.Instance.LockTooltip();
        Close();
        _message = "Friend me?";
        WindowManager.ViewModels.PlayerMenuVM.Close();
    }
}