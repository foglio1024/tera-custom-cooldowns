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

        public BrokerChatMessage(uint playerId, uint listing, int item, long amount, long sellerPrice, long offeredPrice, string name)
        {
            ContainsPlayerName = true;
            Channel = ChatChannel.Bargain;
            Author = name;
            ListingId = listing;
            PlayerId = playerId;

            Amount = new MessagePiece("Offer for " + amount, MessagePieceType.Simple, App.Settings.FontSize, false) { Container = this };
            OfferedPrice = new MessagePiece(new Money(offeredPrice)) { Container = this };
            StartingPrice = new MessagePiece(new Money(sellerPrice)) { Container = this };
            Listing = new MessagePiece("") { Container = this };

            Session.DB.ItemsDatabase.Items.TryGetValue((uint)item, out var i);
            if (i != null)
            {
                Listing.Text = "<" + i.Name + ">";
                Listing.ItemId = i.Id;
                Listing.SetColor(ChatUtils.GradeToColorString(i.RareGrade));
            }
            Listing.Type = MessagePieceType.Item;
        }
    }
}
