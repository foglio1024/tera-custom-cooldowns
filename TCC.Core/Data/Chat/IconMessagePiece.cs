namespace TCC.Data.Chat;

public class IconMessagePiece : MessagePieceBase
{
    public IconMessagePiece(string source, int fontSize, bool customSize)
    {
        _fontSize = fontSize;
        _customSize = customSize;
        Text = source;
    }
}