using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.ViewModels;
using TCC.Windows;


namespace TCC.Controls.Chat
{
    public partial class LfgBody
    {

        public LfgBody()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((LfgMessage)DataContext).LinkedListing != null)
            {
                WindowManager.LfgListWindow.VM.LastClicked = ((LfgMessage)DataContext).LinkedListing;
                ProxyInterop.Proxy.RequestLfgList();
            }
            ProxyInterop.Proxy.RequestPartyInfo(((LfgMessage)DataContext).AuthorId);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            var name = ((sender as FrameworkElement).DataContext as User).Name;
            ProxyInterop.Proxy.AskInteractive(SessionManager.Server.ServerId, name);

        }
    }
}
