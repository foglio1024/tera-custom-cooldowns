using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Interop.Proxy;

namespace TCC.UI.Controls.Chat;

/// <summary>
/// Interaction logic for ApplyBody.xaml
/// </summary>
public partial class ApplyBody
{
    public ApplyBody()
    {
        InitializeComponent();
    }

    void AcceptApplyBtn(object sender, MouseButtonEventArgs e)
    {
        var dc = (ApplyMessage)DataContext;
        if (dc.Handled) return;
        StubInterface.Instance.StubClient.GroupInviteUser(dc.Author, Game.Group.IsRaid);
        dc.Handled = true;
    }

    void InspectBtn(object sender, MouseButtonEventArgs e)
    {
        var dc = (ApplyMessage)DataContext;
        StubInterface.Instance.StubClient.InspectUser(dc.Author, dc.ServerId);
    }

    void DeclineApplyBtn(object sender, MouseButtonEventArgs e)
    {
        var dc = (ApplyMessage)DataContext;
        if (dc.Handled) return;
        StubInterface.Instance.StubClient.DeclineUserGroupApply(dc.PlayerId, dc.ServerId);
        dc.Handled = true;
    }
}