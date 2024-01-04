namespace TCC.Data.Chat;

public class SimpleMessagePiece : MessagePieceBase
{
    public SimpleMessagePiece(string text)
    {
        Text = text;
    }

    public SimpleMessagePiece(string text, int fontSize, bool customSize, string col = "") : this(text)
    {
        _fontSize = fontSize;
        _customSize = customSize;
        Color = col;
    }
}