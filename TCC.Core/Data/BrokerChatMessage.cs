using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data.Databases;
using TCC.Parsing.Messages;

namespace TCC.Data
{
    public class BrokerChatMessage : ChatMessage
    {
        MessagePiece startingPrice;
        public MessagePiece StartingPrice
        {
            get => startingPrice;
            set
            {
                startingPrice = value;
                NotifyPropertyChanged(nameof(StartingPrice));
            }
        }

        MessagePiece offeredPrice;
        public MessagePiece OfferedPrice
        {
            get => offeredPrice;
            set
            {
                offeredPrice = value;
                NotifyPropertyChanged(nameof(OfferedPrice));
            }
        }

        MessagePiece listing;
        public MessagePiece Listing
        {
            get => listing;
            set
            {
                listing = value;
                NotifyPropertyChanged(nameof(Listing));
            }
        }

        MessagePiece amount;
        public MessagePiece Amount
        {
            get => amount;
            set
            {
                amount = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }

        public bool Handled = false;
        public uint PlayerId, ListingId;

        public BrokerChatMessage(S_TRADE_BROKER_DEAL_SUGGESTED p) : base()
        {
            ContainsPlayerName = true;
            Channel = ChatChannel.Bargain;
            Author = p.Name;
            ListingId = p.Listing;
            PlayerId = p.PlayerId;

            Amount = new MessagePiece("Offer for " + p.Amount.ToString(), MessagePieceType.Simple, Channel);
            OfferedPrice = new MessagePiece(new Money(p.OfferedPrice), Channel);
            StartingPrice = new MessagePiece(new Money(p.SellerPrice), Channel);
            Listing = new MessagePiece("");
            
            ItemsDatabase.Items.TryGetValue((uint)p.Item, out Item i);
            if(i != null)
            {
                Listing.Text = "<"+ i.Name + ">";
                Listing.ItemId = i.Id;
                Listing.SetColor(GetItemColor(i));
            }
            Listing.Type = MessagePieceType.Item;
        }
    }
}
