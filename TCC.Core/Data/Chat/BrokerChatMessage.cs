using TCC.Parsing.Messages;

namespace TCC.Data.Chat
{
    public class BrokerChatMessage : ChatMessage
    {
        private MessagePiece _startingPrice;
        public MessagePiece StartingPrice
        {
            get => _startingPrice;
            set
            {
                _startingPrice = value;
                N();
            }
        }

        private MessagePiece _offeredPrice;
        public MessagePiece OfferedPrice
        {
            get => _offeredPrice;
            set
            {
                _offeredPrice = value;
                N();
            }
        }

        private MessagePiece _listing;
        public MessagePiece Listing
        {
            get => _listing;
            set
            {
                _listing = value;
                N();
            }
        }

        private MessagePiece _amount;
        public MessagePiece Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                N();
            }
        }

        public bool Handled = false;
        public readonly uint PlayerId;
        public readonly uint ListingId;

        public BrokerChatMessage(S_TRADE_BROKER_DEAL_SUGGESTED p)
        {
            ContainsPlayerName = true;
            Channel = ChatChannel.Bargain;
            Author = p.Name;
            ListingId = p.Listing;
            PlayerId = p.PlayerId;

            Amount = new MessagePiece("Offer for " + p.Amount, MessagePieceType.Simple, Settings.SettingsHolder.FontSize, false) {Container = this};
            OfferedPrice = new MessagePiece(new Money(p.OfferedPrice)){ Container = this };
            StartingPrice = new MessagePiece(new Money(p.SellerPrice)) { Container = this };
            Listing = new MessagePiece("") { Container = this };
            
            SessionManager.CurrentDatabase.ItemsDatabase.Items.TryGetValue((uint)p.Item, out var i);
            if(i != null)
            {
                Listing.Text = "<"+ i.Name + ">";
                Listing.ItemId = i.Id;
                Listing.SetColor(ChatUtils.GetItemColor(i));
            }
            Listing.Type = MessagePieceType.Item;
        }
    }
}
