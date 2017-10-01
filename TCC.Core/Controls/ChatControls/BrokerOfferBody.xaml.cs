using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for BrokerOfferBody.xaml
    /// </summary>
    public partial class BrokerOfferBody : UserControl
    {
        public BrokerOfferBody()
        {
            InitializeComponent();
        }
        private void Accept(object sender, MouseButtonEventArgs e)
        {
            var dc = (BrokerChatMessage)DataContext;
            ProxyInterop.SendTradeBrokerAccept(dc.PlayerId, dc.ListingId);
        }

        private void Decline(object sender, MouseButtonEventArgs e)
        {
            var dc = (BrokerChatMessage)DataContext;
            if (dc.Handled) return;
            ProxyInterop.SendTradeBrokerDecline(dc.PlayerId, dc.ListingId);
            OnHandled();
        }

        private void OnHandled()
        {
            var dc = (BrokerChatMessage)DataContext;
            dc.Handled = true;
            //declineButton.Visibility = Visibility.Collapsed;
            //acceptButton.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ChatWindowViewModel.Instance.Paused = true;
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ChatWindowViewModel.Instance.Paused = false;
        }
    }
}
