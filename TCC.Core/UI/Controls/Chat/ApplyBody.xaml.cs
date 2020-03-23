using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Interop.Proxy;

namespace TCC.UI.Controls.Chat
{
    /// <summary>
    /// Interaction logic for ApplyBody.xaml
    /// </summary>
    public partial class ApplyBody
    {
        public ApplyBody()
        {
            InitializeComponent();
        }
        private void AcceptApplyBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            if (dc.Handled) return;
            StubInterface.Instance.StubClient.GroupInviteUser(dc.Author); //ProxyOld.PartyInvite(dc.Author);
            dc.Handled = true;
        }
        private void InspectBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            StubInterface.Instance.StubClient.InspectUser(dc.Author); //ProxyOld.Inspect(dc.Author);
        }
        private void DeclineApplyBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            if (dc.Handled) return;
            StubInterface.Instance.StubClient.DeclineUserGroupApply(dc.PlayerId); //ProxyOld.DeclineApply(dc.PlayerId);
            dc.Handled = true;
        }
    }
}
