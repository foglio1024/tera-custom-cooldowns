using System.Windows;
using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Interop;


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
                Proxy.RequestLfgList();
            }
            Proxy.RequestPartyInfo(((LfgMessage)DataContext).AuthorId);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            var name = ((sender as FrameworkElement).DataContext as User).Name;
            Proxy.AskInteractive(SessionManager.Server.ServerId, name);

        }
    }
}
