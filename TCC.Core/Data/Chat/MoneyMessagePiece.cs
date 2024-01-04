namespace TCC.Data.Chat;

public class MoneyMessagePiece : MessagePieceBase
{
    public Money Money { get; }

    public MoneyMessagePiece(Money money)
    {
        Money = money;
        Text = Money.ToString();
    }
}
