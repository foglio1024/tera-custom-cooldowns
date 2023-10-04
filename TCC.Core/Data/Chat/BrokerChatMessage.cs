using TCC.Utilities;
using TCC.Utils;

namespace TCC.Data.Chat;

public class BrokerChatMessage : ChatMessage
{

    public bool Handled = false;
    public uint PlayerId { get; }
    public uint ListingId { get; }
        
    public MessagePieceBase StartingPrice { get; set; }
    public MessagePieceBase OfferedPrice { get; set; }
    public MessagePieceBase Listing { get; set; }
    public MessagePieceBase Amount { get; set; }

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

        Game.DB!.ItemsDatabase.Items.TryGetValue((uint)item, out var i);
        if (i == null) return;
        Listing.Text = "<" + i.Name + ">";
        //TODO: //Listing.ItemId = i.Id;
        Listing.Color = TccUtils.GradeToColorString(i.RareGrade);

        PlainMessage = Listing.Text;
        //Listing.Type = MessagePieceType.Item;
    }
}