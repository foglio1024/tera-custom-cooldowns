using TCC.Parsing.Messages;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TCC.Data
{
    public class ApplyMessage : ChatMessage
    {
        public Class UserClass { get; }
        public uint PlayerId { get; }
        public short PlayerLevel { get; }

        public bool Handled = false;
        public ApplyMessage(S_OTHER_USER_APPLY_PARTY x)
        {
            Channel = ChatChannel.Apply;
            Author = x.Name;
            PlayerId = x.PlayerId;
            PlayerLevel = x.Level;
            UserClass = x.Class;

        }
    }
}
