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
    /// Interaction logic for BrokerOfferBody.xaml
    /// </summary>
    public partial class BrokerOfferBody : UserControl
    {
        public BrokerOfferBody()
        {
            InitializeComponent();
        }
        bool handled = false;
        private void Accept(object sender, MouseButtonEventArgs e)
        {
            if (handled) return;
            var dc = (BrokerChatMessage)DataContext;
            ProxyInterop.SendTradeBrokerAccept(dc.PlayerId, dc.ListingId);
            OnHandled();
        }

        private void Decline(object sender, MouseButtonEventArgs e)
        {
            if (handled) return;
            var dc = (BrokerChatMessage)DataContext;
            ProxyInterop.SendTradeBrokerDecline(dc.PlayerId, dc.ListingId);
            OnHandled();

        }

        private void OnHandled()
        {
            handled = true;
            declineButton.Visibility = Visibility.Collapsed;
            acceptButton.Visibility = Visibility.Collapsed;
        }
    }
}
