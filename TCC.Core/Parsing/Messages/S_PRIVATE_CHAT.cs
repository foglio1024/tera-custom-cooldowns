using TCC.Annotations;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PRIVATE_CHAT : ParsedMessage
    {
        public uint Channel { get; }

        public ulong AuthorId { [UsedImplicitly] get; }

        public string AuthorName { get; }

        public string Message { get; }

        public S_PRIVATE_CHAT(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);
            Channel = reader.ReadUInt32();
            AuthorId = reader.ReadUInt64();
            AuthorName = reader.ReadTeraString();
            Message = reader.ReadTeraString();
        }
    }
}
