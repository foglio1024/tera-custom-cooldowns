using System.Windows;
using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Interop.Proxy;

namespace TCC.UI.Controls.Chat
{
    public partial class LfgBody
    {

        public LfgBody()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (LfgMessage)DataContext;
            if (dc.LinkedListing != null)
            {
                WindowManager.ViewModels.LfgVM.LastClicked = dc.LinkedListing;
                if (WindowManager.LfgListWindow.IsVisible) StubInterface.Instance.StubClient.RequestListings();
                else WindowManager.LfgListWindow.ShowWindow();
            }
            StubInterface.Instance.StubClient.RequestPartyInfo(dc.AuthorId); 
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var name = ((sender as FrameworkElement)?.DataContext as User)?.Name;
            if (name == null) return;
            WindowManager.ViewModels.PlayerMenuVM.Open(name, Game.Server.ServerId);
        }

        private void OnMessageMouseEnter(object sender, MouseEventArgs e)
        {
            Underline.Visibility = Visibility.Visible;
        }

        private void OnMessageMouseLeave(object sender, MouseEventArgs e)
        {
            Underline.Visibility = Visibility.Collapsed;
        }
    }
}
