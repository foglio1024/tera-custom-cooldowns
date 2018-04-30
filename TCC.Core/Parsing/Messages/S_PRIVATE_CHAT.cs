using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PRIVATE_CHAT : ParsedMessage
    {
        private ushort authorNameOffset, messageOffset;
        private uint ch;
        private ulong authorId;
        private string authorName;
        private string message;

        public uint Channel { get => ch; }
        public ulong AuthorId { get => authorId; }
        public string AuthorName { get => authorName; }
        public string Message { get => message; }

        public S_PRIVATE_CHAT(TeraMessageReader reader) : base(reader)
        {
            //authorNameOffset = reader.ReadUInt16();
            //messageOffset = reader.ReadUInt16();
            reader.Skip(4);
            ch = reader.ReadUInt32();
            authorId = reader.ReadUInt64();
            authorName = reader.ReadTeraString();
            message = reader.ReadTeraString();
        }
    }
}
