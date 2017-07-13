using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for ApplyBody.xaml
    /// </summary>
    public partial class ApplyBody : UserControl
    {
        public ApplyBody()
        {
            InitializeComponent();
        }
        private void AcceptApplyBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            if (dc.Handled) return;
            ProxyInterop.SendPartyInvite(dc.Author);
            dc.Handled = true;
        }
        private void InspectBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            ProxyInterop.SendInspect(dc.Author);
        }
        private void DeclineApplyBtn(object sender, MouseButtonEventArgs e)
        {
            var dc = (ApplyMessage)DataContext;
            if (dc.Handled) return;
            ProxyInterop.SendDeclineApply(dc.PlayerId);
            dc.Handled = true;
        }
    }
}
