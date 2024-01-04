using TCC.Utils;
using TeraDataLite;

namespace TCC.Data.Chat;

public class ApplyMessage : ChatMessage
{
    public bool Handled = false;
    public Class UserClass { get; }
    public uint PlayerId { get; }
    public short PlayerLevel { get; }
    public uint ServerId { get; }

    public ApplyMessage(uint playerId, Class c, short level, string name, uint serverId)
    {
        Channel = ChatChannel.Apply;
        Author = name;
        DisplayedAuthor = Author;
        PlayerId = playerId;
        PlayerLevel = level;
        UserClass = c;
        PlainMessage = $"{Author} apply";
        ServerId = serverId;
    }
}