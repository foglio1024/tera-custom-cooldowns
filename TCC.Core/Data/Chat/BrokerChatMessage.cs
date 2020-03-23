using TCC.Utilities;

namespace TCC.Data.Chat
{
    public class BrokerChatMessage : ChatMessage
    {
        private MessagePieceBase _startingPrice;
        public MessagePieceBase StartingPrice
        {
            get => _startingPrice;
            set
            {
                _startingPrice = value;
                N();
            }
        }

        private MessagePieceBase _offeredPrice;
        public MessagePieceBase OfferedPrice
        {
            get => _offeredPrice;
            set
            {
                _offeredPrice = value;
                N();
            }
        }

        private MessagePieceBase _listing;
        public MessagePieceBase Listing
        {
            get => _listing;
            set
            {
                _listing = value;
                N();
            }
        }

        private MessagePieceBase _amount;
        public MessagePieceBase Amount
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

            Amount = new SimpleMessagePiece("Offer for " + amount, App.Settings.FontSize, false) { Container = this };
            OfferedPrice = new MoneyMessagePiece(new Money(offeredPrice)) { Container = this };
            StartingPrice = new MoneyMessagePiece(new Money(sellerPrice)) { Container = this };
            Listing = new SimpleMessagePiece("") { Container = this };

            Game.DB.ItemsDatabase.Items.TryGetValue((uint)item, out var i);
            if (i == null) return;
            Listing.Text = "<" + i.Name + ">";
            //TODO: //Listing.ItemId = i.Id;
            Listing.Color = TccUtils.GradeToColorString(i.RareGrade);
            //Listing.Type = MessagePieceType.Item;
        }
    }
}
