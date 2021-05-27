using TCC.Utils;
using TeraDataLite;

namespace TCC.Data.Chat
{
    public class ApplyMessage : ChatMessage
    {
        public Class UserClass { get; }
        public uint PlayerId { get; }
        public short PlayerLevel { get; }

        public bool Handled = false;
        public ApplyMessage(uint playerId, Class c, short level, string name)
        {
            Channel = ChatChannel.Apply;
            Author = name;
            PlayerId = playerId;
            PlayerLevel = level;
            UserClass = c;
            PlainMessage = $"{Author} apply";

        }
    }
}
