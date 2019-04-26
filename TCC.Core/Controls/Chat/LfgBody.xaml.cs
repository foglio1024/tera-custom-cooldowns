using System.Windows;
using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Interop;
using TCC.Interop.Proxy;


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
                ProxyInterface.Instance.Stub.RequestListings(); //ProxyOld.RequestLfgList();
            }
            ProxyInterface.Instance.Stub.RequestPartyInfo(((LfgMessage)DataContext).AuthorId); // ProxyOld.RequestPartyInfo(((LfgMessage)DataContext).AuthorId);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            var name = ((sender as FrameworkElement).DataContext as User).Name;
            ProxyInterface.Instance.Stub.AskInteractive(SessionManager.Server.ServerId, name); //ProxyOld.AskInteractive(SessionManager.Server.ServerId, name);

        }
    }
}
