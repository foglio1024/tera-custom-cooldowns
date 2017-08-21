using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;

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
    }
}
