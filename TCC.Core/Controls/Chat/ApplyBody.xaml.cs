using System.Windows.Input;
using TCC.Data.Chat;

namespace TCC.Controls.Chat
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
            Proxy.Proxy.PartyInvite(dc.Author);
            dc.Handled = true;
        }
        private void InspectBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            Proxy.Proxy.Inspect(dc.Author);
        }
        private void DeclineApplyBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            if (dc.Handled) return;
            Proxy.Proxy.DeclineApply(dc.PlayerId);
            dc.Handled = true;
        }
    }
}
